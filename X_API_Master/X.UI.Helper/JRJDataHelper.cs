using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Extend.Mongo;
using X.Util.Other;

namespace X.UI.Helper
{
    public enum EnumTab
    {
        ZT = 0,
        LB = 1,
        FAST = 2,
        FAST_LB = 3,
        FAST_MIN = 4,
        ZT_MIN = 5
    }

    public class JRJDataHelper
    {
        public static List<JRJDataItem> GetJRJData(DateTime dt)
        {
            var ret = HttpRequestBase.GetHttpInfo("http://home.flashdata2.jrj.com.cn/limitStatistic/ztForce/" + dt.ToString("yyyyMMdd") + ".js?_=1543031229126",
    "gb2312", "appication/json", null, string.Empty);
            if (!ret.Success) return null;
            var index = ret.Content.IndexOf('{');
            var c = ret.Content.Substring(index, ret.Content.Length - index - 1).FromJson<JRJData>();
            var list = new List<JRJDataItem>();
            if (c.Data != null && c.Data.Count > 0)
            {
                foreach (var item in c.Data)
                {
                    list.Add(new JRJDataItem
                    {
                        Id = string.Format("{0}_{1}", dt.ToString("yyyyMMdd"), item[0]),
                        DateTime = dt,
                        StockCode = item[0],
                        StockName = item[1],
                        Price = item[2].Convert2Decimal(0),
                        PriceLimit = item[3].Convert2Decimal(0),
                        FCB = item[4].Convert2Decimal(0),
                        FLB = item[5].Convert2Decimal(0),
                        FDMoney = item[6].Convert2Decimal(0),
                        FirstZtTime = (dt.ToString("yyyy-MM-dd ") + item[7]).Convert2DateTime(DateTime.MinValue),
                        LastZtTime = (dt.ToString("yyyy-MM-dd ") + item[8]).Convert2DateTime(DateTime.MinValue),
                        OpenTime = item[9].Convert2Int32(0),
                        ZF = item[10].Convert2Decimal(0),
                        Force = item[11].Convert2Double(0),
                    });
                }
            }
            return list;
        }


        public static void DealData(DateTime start, DateTime end)
        {
            while(start<= end)
            {
                Console.WriteLine("query jrj start.{0}", start);
                var list = GetJRJData(start);
                if (list == null)
                {
                    start = start.AddDays(1);
                    continue;
                }
                Console.WriteLine("query db end and insert mongodb start.");
                foreach(var item in list)
                {
                    MongoDbBase<JRJDataItem>.Default.SaveMongo(item, "Stock", "JRJ");
                    Console.WriteLine("DealDone Code {1}({0}), save done mongodb", item.StockCode, item.StockName);
                }
                Console.WriteLine("insert mongodb end! deal done!{0}", start);
                start = start.AddDays(1);
            }
            Console.WriteLine("Task end!");
        }

        public static List<JRJDataItem> GetDataFromMongo(DateTime dt)
        {
            return MongoDbBase<JRJDataItem>.Default.Find("Stock", "JRJ", Query.EQ("DateTime", dt.Date), Fields.Null, SortBy.Descending("DateTime", "Force")).ToList();
        }

        public static List<JRJDataItem> GetTab(DateTime dt, EnumTab tab)
        {
            var timeOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 30, 0).TimeOfDay;
            var data = GetDataFromMongo(dt);
            switch (tab)
            {
                case EnumTab.ZT:
                    return data;
                case EnumTab.LB:
                    return data.Where(p => p.OpenTime > 0).ToList();
                case EnumTab.FAST:
                    return data.Where(p => p.FirstZtTime.TimeOfDay<= timeOfDay).ToList();
                case EnumTab.FAST_LB:
                    return data.Where(p => p.FirstZtTime.TimeOfDay <= timeOfDay && p.OpenTime > 0).ToList();
                case EnumTab.FAST_MIN:
                    return data.Where(p => p.FirstZtTime.TimeOfDay <= timeOfDay && p.Amount <= 3e8M).ToList();
                case EnumTab.ZT_MIN:
                    return data.Where(p => p.Amount <= 3e8M).ToList();
            }
            return data;
        }
    }
}
