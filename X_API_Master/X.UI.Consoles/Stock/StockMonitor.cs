using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using X.Util.Other;

namespace X.UI.Consoles.Stock
{
    public class StockMonitor
    {
        public static List<StockBase> StockBase = GetStockBase();

        private static decimal GetDecimal(string str)
        {
            if (str.Contains("亿"))
            {
                return decimal.Parse(str.Split('亿')[0])*100000000;
            }
            if (str.Contains("万"))
            {
                return decimal.Parse(str.Split('万')[0]) * 10000;
            }
            decimal ret;
            decimal.TryParse(str, out ret);
            return ret;
        }

        public static List<StockBase> GetStockBase()
        {
            var content =
                FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "stockbase.txt"),
                    "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim().Split('\t')).Select(arr => new StockBase
            {
                StockCode = arr[0].Trim(), StockName = arr[1].Trim(), Industry = arr[2].Trim(), GeneralCapital = GetDecimal(arr[3].Trim()), NegotiableCapital = GetDecimal(arr[4].Trim())
            }).ToList();
        }

        public static List<string> GetCodeList_1()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_1.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_2()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_1.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_3()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_1.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_4()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_1.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_5()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_1.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_6()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_1.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }

        public static StockPrice GetStockPrice(string code)
        {
            var uri = code.StartsWith("6") ? "http://hq.sinajs.cn/list=sh" + code : "http://hq.sinajs.cn/list=sz" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            return new StockPrice
            {
                StockCode = code,
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                LastClosePrice = decimal.Parse(arr[2])
            };
        }

        public static decimal Monitor(List<string> list)
        {
            var ret = default(decimal);
            var toatlCapital = default(decimal);
            foreach (var code in list)
            {
                var item = GetStockPrice(code);
                var capital = StockBase.First(p => p.StockCode == code).NegotiableCapital;
                toatlCapital += capital;
                ret += capital * item.Inc;
            }
            ret /= toatlCapital;
            return ret;
        }

        public static void Monitor()
        {
            var d1 = DateTime.Now.Date.AddHours(9).AddMinutes(30);
            var d2 = DateTime.Now.Date.AddHours(11).AddMinutes(30);
            var d3 = DateTime.Now.Date.AddHours(13);
            var d4 = DateTime.Now.Date.AddHours(15);
            while (true)
            {
                if ((DateTime.Now >= d1 && DateTime.Now <= d2) || (DateTime.Now >= d3 && DateTime.Now <= d4))
                {
                    Console.WriteLine("code_1:" + Monitor(GetCodeList_1()));
                    Console.WriteLine("code_2:" + Monitor(GetCodeList_2()));
                    Console.WriteLine("code_3:" + Monitor(GetCodeList_3()));
                    Console.WriteLine("code_4:" + Monitor(GetCodeList_4()));
                    Console.WriteLine("code_5:" + Monitor(GetCodeList_5()));
                    Console.WriteLine("code_6:" + Monitor(GetCodeList_6()));
                }
                Thread.Sleep(6000);
            }
        }
    }
}
