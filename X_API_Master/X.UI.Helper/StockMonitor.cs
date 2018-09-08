using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using X.UI.Entities;
using X.Util.Core.Log;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockMonitor
    {
        #region 静态文件数据

        public static List<StockBase> StockBase = GetStockBase();
        //public static List<string> CodeList1 = GetCodeList("code_1.txt");
        //public static List<string> CodeList2 = GetCodeList("code_2.txt");
        //public static List<string> CodeList3 = GetCodeList("code_3.txt");
        //public static List<string> CodeList4 = GetCodeList("code_4.txt");
        //public static List<string> CodeList5 = GetCodeList("code_5.txt");
        //public static List<string> CodeList6 = GetCodeList("code_6.txt");

        public static List<string> K005List = GetCodeList("K005.EBK");
        public static List<string> K010List = GetCodeList("K010.EBK");
        public static List<string> K015List = GetCodeList("K015.EBK");
        public static List<string> K110List = GetCodeList("K110.EBK");
        public static List<string> K122List = GetCodeList("K122.EBK");
        public static List<string> K132List = GetCodeList("K132.EBK");

        private static decimal GetDecimal(string str)
        {
            if (str.Contains("亿"))
            {
                return decimal.Parse(str.Split('亿')[0])*100000000;
            }
            if (str.Contains("万"))
            {
                return decimal.Parse(str.Split('万')[0])*10000;
            }
            decimal ret;
            decimal.TryParse(str, out ret);
            return ret;
        }

        private static List<StockBase> GetStockBase()
        {
            var content =
                FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "stockbase.txt"),
                    "gb2312");
            var list = content.Trim().Split('\n');
            return list.Select(item => item.Trim().Split('\t')).Select(arr => new StockBase
            {
                StockCode = arr[0].Trim(),
                StockName = arr[1].Trim(),
                Industry = arr[2].Trim(),
                GeneralCapital = GetDecimal(arr[3].Trim()),
                NegotiableCapital = GetDecimal(arr[4].Trim())
            }).ToList();
        }

        private static List<string> GetCodeList(string file)
        {
            var content = FileBase.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", file),
                "gb2312");
            var list = content.Trim().Split('\n');
            return list.Where(item => !string.IsNullOrEmpty(item.Trim()))
                .Select(item => item.Trim().Substring(item.Trim().Length - 6)).ToList();
        }

        #endregion

        public static StockPrice GetStockPrice(string code)
        {
            var uri = code.StartsWith("6") ? "http://hq.sinajs.cn/list=sh" + code : "http://hq.sinajs.cn/list=sz" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            if (string.IsNullOrEmpty(ret.Content)) return default(StockPrice);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            if (string.IsNullOrEmpty(content) || arr.Length == 1) return default(StockPrice);
            var result = new StockPrice
            {
                StockCode = code,
                StockName = arr[0],
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                LastClosePrice = decimal.Parse(arr[2]),
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice/result.LastClosePrice*100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice/result.LastClosePrice*100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice/result.LastClosePrice*100 - 100;
            if (K110List.Contains(code)) result.Indexs.Add("393110");
            if (K005List.Contains(code)) result.Indexs.Add("393005");
            if (K122List.Contains(code)) result.Indexs.Add("393122");
            if (K010List.Contains(code)) result.Indexs.Add("393010");
            if (K132List.Contains(code)) result.Indexs.Add("393132");
            if (K015List.Contains(code)) result.Indexs.Add("393015");
            return result;
        }

        public static Dictionary<string, StockPrice> GetStockPrice(List<string> list)
        {
            var ret = new Dictionary<string, StockPrice>();
            foreach (var code in list.Where(code => !ret.ContainsKey(code) && !string.IsNullOrEmpty(code)))
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
            return GetStockPrice(K110List.
                Union(K005List).
                Union(K122List).
                Union(K010List).
                Union(K132List).
                Union(K015List).ToList());
        }


        public static decimal Monitor(Dictionary<string, StockPrice> stockPrice, List<string> list)
        {
            var ret = default(decimal);
            var toatlCapital = default(decimal);
            foreach (var code in list.Where(p=>!string.IsNullOrEmpty(p)))
            {
                try
                {
                    var item = stockPrice[code];
                    var capital = StockBase.First(p => p.StockCode == code).NegotiableCapital;
                    toatlCapital += capital;
                    ret += capital * (item != null ? item.Inc : 0);
                }
                catch(Exception e)
                {
                    //Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { code }), e, LogDomain.Ui);
                    continue;
                }
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
                    StockCode = "393110",
                    Inc = Monitor(dictionary, K010List),
                },
                new StockPrice
                {
                    StockCode = "393005",
                    Inc = Monitor(dictionary, K005List),
                },
                new StockPrice
                {
                    StockCode = "393122",
                    Inc = Monitor(dictionary, K122List)
                },
                new StockPrice
                {
                    StockCode = "393010",
                    Inc = Monitor(dictionary, K010List)
                },
                new StockPrice
                {
                    StockCode = "393132",
                    Inc = Monitor(dictionary, K132List)
                },
                new StockPrice
                {
                    StockCode = "393015",
                    Inc = Monitor(dictionary, K015List)
                },
            };
            return ret;
        }

        public static void SetConsoleColor(decimal value, string format)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = value > 0
                ? ConsoleColor.Red
                : value < 0
                    ? ConsoleColor.Green
                    : ConsoleColor.Black;
            Console.WriteLine(format, value);
        }

        public static void Monitor()
        {
            var d1 = DateTime.Now.Date.AddHours(9).AddMinutes(25);
            var d2 = DateTime.Now.Date.AddHours(11).AddMinutes(30);
            var d3 = DateTime.Now.Date.AddHours(13);
            var d4 = DateTime.Now.Date.AddHours(15);
            var d5 = DateTime.Now.Date.AddDays(1);
            while (true)
            {
                var dic = GetStockPrice();
                if ((DateTime.Now >= d1 && DateTime.Now <= d2) || (DateTime.Now >= d3 && DateTime.Now <= d4))
                {
                    SetConsoleColor(Monitor(dic, K110List), "393110:{0}");
                    SetConsoleColor(Monitor(dic, K005List), "393005:{0}");
                    SetConsoleColor(Monitor(dic, K122List), "393122:{0}");
                    SetConsoleColor(Monitor(dic, K010List), "393010:{0}");
                    SetConsoleColor(Monitor(dic, K132List), "393132:{0}");
                    SetConsoleColor(Monitor(dic, K015List), "393015:{0}");
                    foreach (
                        var item in
                            dic.Where(p => p.Value != null && (p.Value.Inc >= 4 || p.Value.Inc <= -4))
                                .OrderByDescending(p => p.Value.Inc))
                    {
                        SetConsoleColor(item.Value.Inc,
                            item.Value.StockName + "(" + item.Value.StockCode + ")(" +
                            string.Join("|", item.Value.Indexs) + ")" + ":{0}");
                    }
                    Thread.Sleep(2000);
                }
                else
                {
                    var ts = DateTime.Now < d1
                        ? d1 - DateTime.Now
                        : DateTime.Now < d2
                            ? d2 - DateTime.Now
                            : DateTime.Now < d3
                                ? d3 - DateTime.Now
                                : DateTime.Now < d4 ? d4 - DateTime.Now : d5 - DateTime.Now;
                    Console.WriteLine("非交易时间,行情不更新,{0}毫秒后刷新行情", Math.Floor(ts.TotalMilliseconds / 2));
                    Thread.Sleep(Math.Max((int) Math.Floor(ts.TotalMilliseconds/2), 2000));
                }
            }
        }
    }
}
