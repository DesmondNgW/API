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
        #region 静态文件数据

        public static List<StockBase> StockBase = GetStockBase();
        public static List<string> CodeList1 = GetCodeList("code_1.txt");
        public static List<string> CodeList2 = GetCodeList("code_2.txt");
        public static List<string> CodeList3 = GetCodeList("code_3.txt");
        public static List<string> CodeList4 = GetCodeList("code_4.txt");
        public static List<string> CodeList5 = GetCodeList("code_5.txt");
        public static List<string> CodeList6 = GetCodeList("code_6.txt");

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
            return list.Select(item => item.Trim()).ToList();
        }

        #endregion

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
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice/result.LastClosePrice*100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice/result.LastClosePrice*100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice/result.LastClosePrice*100 - 100;
            //if (CodeList1.Contains(code)) result.Indexs.Add("Code_1");
            //if (CodeList2.Contains(code)) result.Indexs.Add("Code_2");
            if (CodeList3.Contains(code)) result.Indexs.Add("Code_3");
            if (CodeList4.Contains(code)) result.Indexs.Add("Code_4");
            if (CodeList5.Contains(code)) result.Indexs.Add("Code_5");
            if (CodeList6.Contains(code)) result.Indexs.Add("Code_6");
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
            return GetStockPrice(CodeList1.
                Union(CodeList2).
                Union(CodeList3).
                Union(CodeList4).
                Union(CodeList5).
                Union(CodeList6).ToList());
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
                    Inc = Monitor(dictionary, CodeList1),
                },
                new StockPrice
                {
                    StockCode = "393702",
                    Inc = Monitor(dictionary, CodeList2),
                },
                new StockPrice
                {
                    StockCode = "393703",
                    Inc = Monitor(dictionary, CodeList3)
                },
                new StockPrice
                {
                    StockCode = "393704",
                    Inc = Monitor(dictionary, CodeList4)
                },
                new StockPrice
                {
                    StockCode = "393705",
                    Inc = Monitor(dictionary, CodeList5)
                },
                new StockPrice
                {
                    StockCode = "393706",
                    Inc = Monitor(dictionary, CodeList6)
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
                    SetConsoleColor(Monitor(dic, CodeList1), "code_1:{0}");
                    SetConsoleColor(Monitor(dic, CodeList2), "code_2:{0}");
                    SetConsoleColor(Monitor(dic, CodeList3), "code_3:{0}");
                    SetConsoleColor(Monitor(dic, CodeList4), "code_4:{0}");
                    SetConsoleColor(Monitor(dic, CodeList5), "code_5:{0}");
                    SetConsoleColor(Monitor(dic, CodeList6), "code_6:{0}");
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
