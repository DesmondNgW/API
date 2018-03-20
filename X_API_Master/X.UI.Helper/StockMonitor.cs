using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using X.UI.Entities;
using X.Util.Other;

namespace X.UI.Helper
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
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_2.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_3()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_3.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_4()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_4.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_5()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_5.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }
        public static List<string> GetCodeList_6()
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "code_6.txt"), "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim()).ToList();
        }

        public static StockPrice GetStockPrice(string code)
        {
            var uri = code.StartsWith("6") ? "http://hq.sinajs.cn/list=sh" + code : "http://hq.sinajs.cn/list=sz" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            if (string.IsNullOrEmpty(ret.Content)) return default(StockPrice);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            var result = new StockPrice
            {
                StockCode = code,
                StockName = arr[0],
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                LastClosePrice = decimal.Parse(arr[2]),
            };
            result.Inc = result.CurrentPrice / result.LastClosePrice * 100 - 100;
            result.MaxInc = result.MaxPrice / result.LastClosePrice * 100 - 100;
            result.MinInc = result.MinPrice / result.LastClosePrice * 100 - 100;
            return result;
        }

        public static Dictionary<string, StockPrice> GetStockPrice(List<string> list)
        {
            var ret = new Dictionary<string, StockPrice>();
            foreach (var code in list.Where(code => !ret.ContainsKey(code)))
            {
                ret[code] = GetStockPrice(code);
            }
            return ret;
        }

        /// <summary>
        /// 所有行情数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, StockPrice> GetStockPrice()
        {
            return GetStockPrice(GetCodeList_1().
                Union(GetCodeList_2()).
                Union(GetCodeList_3()).
                Union(GetCodeList_4()).
                Union(GetCodeList_5()).
                Union(GetCodeList_6()).ToList());
        }


        public static decimal Monitor(Dictionary<string, StockPrice> stockPrice, List<string> list)
        {
            var ret = default(decimal);
            var toatlCapital = default(decimal);
            foreach (var code in list)
            {
                var item = stockPrice[code];
                var capital = StockBase.First(p => p.StockCode == code).NegotiableCapital;
                toatlCapital += capital;
                ret += capital*(item != null ? item.Inc : 0);
            }
            ret /= toatlCapital;
            return ret;
        }

        public static List<StockPrice> GetSpecialStockPrice(Dictionary<string, StockPrice> dictionary)
        {
            var ret = new List<StockPrice>
            {
                new StockPrice
                {
                    StockCode = "393701",
                    Inc = Monitor(dictionary, GetCodeList_1()),
                },
                new StockPrice
                {
                    StockCode = "393702",
                    Inc = Monitor(dictionary, GetCodeList_2()),
                },
                new StockPrice
                {
                    StockCode = "393703",
                    Inc = Monitor(dictionary, GetCodeList_3())
                },
                new StockPrice
                {
                    StockCode = "393704",
                    Inc = Monitor(dictionary, GetCodeList_4())
                },
                new StockPrice
                {
                    StockCode = "393705",
                    Inc = Monitor(dictionary, GetCodeList_5())
                },
                new StockPrice
                {
                    StockCode = "393706",
                    Inc = Monitor(dictionary, GetCodeList_6())
                },
            };
            return ret;
        }

        public static void Monitor()
        {
            var d1 = DateTime.Now.Date.AddHours(9).AddMinutes(30);
            var d2 = DateTime.Now.Date.AddHours(11).AddMinutes(30);
            var d3 = DateTime.Now.Date.AddHours(13);
            var d4 = DateTime.Now.Date.AddHours(19);
            while (true)
            {
                var dic = GetStockPrice();
                if ((DateTime.Now >= d1 && DateTime.Now <= d2) || (DateTime.Now >= d3 && DateTime.Now <= d4))
                {
                    var monitor1 = Monitor(dic, GetCodeList_1());
                    var monitor2 = Monitor(dic, GetCodeList_2());
                    var monitor3 = Monitor(dic, GetCodeList_3());
                    var monitor4 = Monitor(dic, GetCodeList_4());
                    var monitor5 = Monitor(dic, GetCodeList_5());
                    var monitor6 = Monitor(dic, GetCodeList_6());
                    if (monitor1 >= 0.5M) Console.WriteLine("code_1:" + monitor1);
                    if (monitor2 >= 0.5M) Console.WriteLine("code_2:" + monitor2);
                    if (monitor3 >= 0.5M) Console.WriteLine("code_3:" + monitor3);
                    if (monitor4 >= 0.5M) Console.WriteLine("code_4:" + monitor4);
                    if (monitor5 >= 0.5M) Console.WriteLine("code_5:" + monitor5);
                    if (monitor6 >= 0.5M) Console.WriteLine("code_6:" + monitor6);
                    foreach (var item in dic.Where(p => p.Value != null && (p.Value.Inc >= 4 || p.Value.Inc <= -6)).OrderByDescending(p => p.Value.Inc))
                    {
                        Console.WriteLine(item.Value.StockName + "(" + item.Value.StockCode + ")" + ":" + item.Value.Inc);
                    }
                }
                Thread.Sleep(6000);
            }
        }
    }
}
