﻿using MongoDB.Driver.Builders;
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
            if (c != null && c.Data != null && c.Data.Count > 0)
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
                var a0 = list.Count(p => p.LastZtTime.TimeOfDay <= new TimeSpan(10, 30, 0));
                var a1 = list.Count(p => p.LastZtTime.TimeOfDay <= new TimeSpan(11, 30, 0));
                var a2 = list.Count(p => p.LastZtTime.TimeOfDay <= new TimeSpan(14, 00, 0));
                var a3 = list.Count(p => p.LastZtTime.TimeOfDay <= new TimeSpan(15, 00, 0));

                var b0 = list.Count(p => p.Amount <= 1e8M);
                var b1 = list.Count(p => p.Amount <= 2e8M);
                var b2 = list.Count(p => p.Amount <= 3e8M);
                var b3 = list.Count(p => p.Amount <= 4e8M);
                var b4 = list.Count(p => p.Amount <= 5e8M);
                var b5 = list.Count(p => p.Amount <= 8e8M);
                var b6 = list.Count(p => p.Amount > 8e8M);

                Console.WriteLine("9:30-10:30:<{0}>,10:30-11:30:<{1}>,13:00-14:00:<{2}>,14:00-15:00:<{3}>", a0, a1 - a0, a2 - a1, a3 - a2);

                Console.WriteLine("0-1:<{0}>,1-2:<{1}>,2-3:<{2}>,3-4:<{3}>,4-5:<{4}>,5-8:<{5}>，>8:<{6}>", b0, b1 - b0, b2 - b1, b3 - b2, b4 - b3, b5 - b4, b6);

            }
            Console.WriteLine("Task end!");
        }

        public static List<JRJDataItem> GetDataFromMongo(DateTime dt)
        {
            return MongoDbBase<JRJDataItem>.Default.Find("Stock", "JRJ", Query.EQ("DateTime", dt.Date), Fields.Null, SortBy.Descending("DateTime", "Force")).ToList();
        }

        public static List<JRJDataItem> GetDataFromMongo(DateTime dt, TimeSpan ts)
        {
            return MongoDbBase<JRJDataItem>.Default.Find("Stock", "JRJ", Query.GTE("DateTime", dt), Fields.Null, SortBy.Descending("DateTime", "Force"))
                .Where(p => p.FirstZtTime.TimeOfDay < ts).ToList();
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
                    return data.Where(p => p.LastZtTime.TimeOfDay<= timeOfDay).ToList();
                case EnumTab.FAST_LB:
                    return data.Where(p => p.LastZtTime.TimeOfDay <= timeOfDay && p.OpenTime > 0).ToList();
                case EnumTab.FAST_MIN:
                    return data.Where(p => p.LastZtTime.TimeOfDay <= timeOfDay && p.Amount <= 2e8M).ToList();
                case EnumTab.ZT_MIN:
                    return data.Where(p => p.Amount <= 3e8M).ToList();
            }
            return data;
        }

        public static List<JRJDataItem> WebUI(DateTime start, TimeSpan ts)
        {
            var data = GetDataFromMongo(start, ts)
                .Where(p => p.PriceLimit > 7 && p.PriceLimit < 11)
                .OrderBy(p => p.DateTime);

            var dic = new Dictionary<DateTime, List<string>>();
            foreach (var item in data)
            {
                if (dic.ContainsKey(item.DateTime))
                {
                    dic[item.DateTime].Add(item.StockCode);
                }
                else
                {
                    dic[item.DateTime] = new List<string>() { item.StockCode };
                }
            }
            var idata = new List<JRJDataItem>();
            var LastDate = DateTime.MinValue;
            var CurrentDate = DateTime.MinValue;
            foreach (var item in data)
            {
                if (dic.ContainsKey(LastDate) && !dic[LastDate].Contains(item.StockCode))
                {
                    continue;
                }
                bool a = false;
                if (item.FirstZtTime.TimeOfDay <= new TimeSpan(9, 45, 0) && item.LastZtTime.TimeOfDay >= new TimeSpan(9, 26, 0))
                {
                    a = true;
                }
                if (item.LastZtTime.TimeOfDay <= new TimeSpan(9, 26, 0) && item.FirstZtTime.TimeOfDay >= new TimeSpan(9, 30, 0))
                {
                    a = true;
                }
                if (a)
                {
                    idata.Add(item);
                }
                if (CurrentDate != item.DateTime)
                {
                    if (CurrentDate != DateTime.MinValue)
                    {
                        LastDate = CurrentDate;
                    }
                    CurrentDate = item.DateTime;
                }
            }
            return idata;
        }


        public static List<Continue> Continue(DateTime start, DateTime end, int count = 2)
        {
            var query = Query.And(Query.GTE("DateTime", start), Query.LTE("DateTime", end));
            var list  = MongoDbBase<JRJDataItem>.Default.Find("Stock", "JRJ", query, Fields.Null, SortBy.Ascending("DateTime", "Force")).ToList();
            var _list = (from item in list group item by item.DateTime).ToList();
            var ret = new Dictionary<string, Continue>();
            var LastTime = DateTime.MinValue;
            for (var i = 0; i < _list.Count; i++)
            {
                foreach(var k in _list[i])
                {
                    var lastKey = string.Format("{0}_{1}", k.StockCode, LastTime.ToString("yyyyMMdd"));
                    var key = string.Format("{0}_{1}", k.StockCode, k.DateTime.ToString("yyyyMMdd"));
                    if (!ret.ContainsKey(lastKey))
                    {
                        ret[key] = new Continue
                        {
                            StockCode = k.StockCode,
                            StockName = k.StockName,
                            Start = k.DateTime,
                            End = k.DateTime,
                            Count = 1,
                            Enable = true
                        };
                    }
                    else
                    {
                        ret[lastKey].Enable = false;
                        ret[key] = ret[lastKey];
                        ret[key].Count++;
                        ret[key].End = k.DateTime;
                        ret[key].Enable = true;
                    }
                }
                LastTime = _list[i].Key;
            }
            return ret.Where(p => p.Value.Count >= count
            && p.Value.Enable
            && p.Value.StockName.IndexOf("ST", StringComparison.OrdinalIgnoreCase) == -1).Select(p => p.Value).Distinct().ToList();
        }

        public static void ContinueInFile(List<Continue> list)
        {
            Dictionary<int, bool> tmp = new Dictionary<int, bool>();
            foreach (var item in list)
            {
                var content = string.Format("{0}（{1}）【{2}-{3}】长度{4}", item.StockName, item.StockCode, item.Start, item.End, item.Count);
                if (!tmp.ContainsKey(item.Start.Year))
                {
                    tmp[item.Start.Year] = true;
                    FileBase.WriteFile("./", "test-"+ item.Start.Year + ".txt", "-------------", "utf-8", Util.Entities.Enums.FileBaseMode.Create);
                }
                FileBase.WriteFile("./", "test-" + item.Start.Year + ".txt", content, "utf-8", Util.Entities.Enums.FileBaseMode.Append);
            }
            Console.WriteLine("Write File End!");
        }

        private static List<JRJDataItem> GetLastTest(DateTime dt, int times = 30)
        {
            var query = Query.EQ("DateTime", dt);
            var list = MongoDbBase<JRJDataItem>.Default.Find("Stock", "JRJ", query, Fields.Null, SortBy.Ascending("DateTime", "Force")).ToList();
            if (list.Count == 0 && times > 0)
            {
                return GetLastTest(dt.AddDays(-1), times - 1);
            }
            return list;
        }


        public static void TestSingle(DateTime dt)
        {
            var query = Query.EQ("DateTime", dt);
            var list = MongoDbBase<JRJDataItem>.Default.Find("Stock", "JRJ", query, Fields.Null, SortBy.Ascending("DateTime", "Force")).ToList();
            if (list.Count == 0) return;
            var last = GetLastTest(dt.AddDays(-1));
            FileBase.WriteFile("../f", dt.ToString("yyyyMMdd") + "-1.txt", "-------------", "utf-8", Util.Entities.Enums.FileBaseMode.Create);
            FileBase.WriteFile("../f", dt.ToString("yyyyMMdd") + "-2.txt", "-------------", "utf-8", Util.Entities.Enums.FileBaseMode.Create);
            foreach (var item in list.Where(p => p.StockName.IndexOf("ST", StringComparison.OrdinalIgnoreCase) == -1))
            {
                if (last.Exists(p => p.StockCode == item.StockCode))
                {
                    FileBase.WriteFile("./", dt.ToString("yyyyMMdd") + "-2.txt", item.StockName, "utf-8", Util.Entities.Enums.FileBaseMode.Append);
                }
                else
                {
                    FileBase.WriteFile("./", dt.ToString("yyyyMMdd") + "-1.txt", item.StockName, "utf-8", Util.Entities.Enums.FileBaseMode.Append);
                }
            }
        }

        public static void Test()
        {
            var start = new DateTime(2016, 1, 1);
            while (start < DateTime.Now.Date)
            {
                TestSingle(start);
                start = start.AddDays(1);
            }
        }

    }
}
