using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Core.Configuration;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockDealHelper
    {
        #region 工具算法
        /// <summary>
        /// 非ST的有效股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool F1(MyStock p)
        {
            return p.Inc > -100 && !p.Name.Contains("ST");
        }

        /// <summary>
        /// 非ST，中轨上方, 量价齐增的有效股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool F3(MyStock p)
        {
            return p.Inc > -100 && !p.Name.Contains("ST") && (p.K1 + p.K2 + p.K3 + p.K4) / 4 > 7.5 && p.K4 > 0;
        }

        /// <summary>
        /// 非ST，成交在3.82亿以上的股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool F4(MyStock p, double t)
        {
            return p.Inc > -100 && !p.Name.Contains("ST") && p.Amount >= t;// 3.82 * 1e8;
        }

        /// <summary>
        /// 输出代码和名称
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string O1(MyStock p)
        {
            return p.Code + " " + p.Name;
        }

        /// <summary>
        /// 输出代码、名称和量价排序
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string O2(MyStock p)
        {
            return p.Code + " " + p.Name + " " + (p.K1 + p.K2 + p.K3 + p.K4) / 4 + " " + (p.MyStockMode == MyStockMode.AQS ? "AQS" : "Wave");
        }

        /// <summary>
        /// 加权均值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double Calc(IEnumerable<MyStock> list)
        {
            double b = 0;
            double c = 0;
            foreach (var item in list)
            {
                b += item.Cap * item.LastClose;
                c += item.Cap * item.Close;
            }
            return c / b * 100 - 100;
        }

        /// <summary>
        /// GetAnswer
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Tuple<List<int>, List<int>> GetAnswer(List<Tuple<double, double>> list)
        {
            var ret = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());
            if (list.Count >= 5)
            {
                for (var i = 2; i < list.Count; i++)
                {
                    //高低点
                    if ((list[i - 1].Item1 >= list[i - 2].Item1 && list[i - 1].Item1 >= list[i].Item1) ||
                        (list[i - 1].Item1 <= list[i - 2].Item1 && list[i - 1].Item1 <= list[i].Item1))
                    {
                        ret.Item1.Add(i * 25);
                    }
                    //高低点
                    if ((list[i - 1].Item2 >= list[i - 2].Item2 && list[i - 1].Item2 >= list[i].Item2) ||
                        (list[i - 1].Item2 <= list[i - 2].Item2 && list[i - 1].Item2 <= list[i].Item2))
                    {
                        ret.Item2.Add(i * 25);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 合并多个List
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<MyStock> Union(params List<MyStock>[] list)
        {
            var ret = new List<MyStock>();
            foreach(List<MyStock> item in list)
            {
                foreach (var current in item.Where(p => !ret.Exists(q => q.Code == p.Code)))
                {
                    ret.Add(current);
                }
            }
            return ret;
        }

        /// <summary>
        /// 二进制数据位解析
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static List<int> MT(int a)
        {
            var ret = new List<int>();
            var t1 = a % 2;
            ret.Add(t1);

            var a1 = (a - t1) / 2;
            var t2 = a1 % 2;
            ret.Add(t2);

            var a2 = (a1 - t2) / 2;
            var t3 = a2 % 2;
            ret.Add(t3);

            var a3 = (a2 - t3) / 2;
            var t4 = a3 % 2;
            ret.Add(t4);

            var a4 = (a3 - t4) / 2;
            var t5 = a4 % 2;
            ret.Add(t5);

            var a5 = (a4 - t5) / 2;
            var t6 = a5 % 2;
            ret.Add(t6);

            var a6 = (a5 - t6) / 2;
            var t7 = a6 % 2;
            ret.Add(t7);
            return ret;
        }

        /// <summary>
        /// 匹配值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool MtMatch(int a, int b)
        {
            var ret = 0;
            var r1 = MT(a);
            var r2 = MT(b);
            for (var i = 0; i < r1.Count; i++)
            {
                if (r1[i] == r2[i])
                {
                    ret++;
                }
            }
            return ret >= 5;
        }

        #endregion

        #region 复盘逻辑
        /// <summary>
        /// 输入文件导入
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<MyStock> GetMyStock(MyStockMode mode)
        {
            var file = mode == MyStockMode.Index ? "./src/fp/板块.txt" :
                 mode == MyStockMode.AQS ? "./src/fp/AQS.txt" :
                 mode == MyStockMode.Wave ? "./src/fp/Wave.txt" : "./src/fp/AQS.txt";
            var content = FileBase.ReadFile(file, "gb2312");
            var list = Regex.Split(content, "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<MyStock>();
            foreach (var item in list)
            {
                var t = item.Split('\t');
                if (t.Length >= 16)
                {
                    var myStock = new MyStock()
                    {
                        Code = t[0],
                        Name = t[1],
                        Inc = t[2].Convert2Double(-10000),
                        Close = t[3].Convert2Double(0),
                        Vol = t[4].Convert2Double(0),
                        Amount = t[5].Convert2Double(0),
                        S1 = t[6].Convert2Double(-10000),
                        S2 = t[7].Convert2Double(-10000),
                        S3 = t[8].Convert2Double(-10000),
                        S4 = t[9].Convert2Double(-10000),
                        K1 = t[10].Convert2Double(-100),
                        K2 = t[11].Convert2Double(-100),
                        K3 = t[12].Convert2Double(-100),
                        K4 = t[13].Convert2Double(-100),
                        Cap = t[14].Convert2Double(0),
                        MT = t[15].Convert2Int32(0),
                        MyStockMode = mode
                    };
                    ret.Add(myStock);
                }
            }
            //SetOrder(ret);
            return ret;
        }

        /// <summary>
        /// 前排股票
        /// </summary>
        /// <param name="list"></param>
        /// <param name="top"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string[] GetStockName(IEnumerable<MyStock> list, int top, Func<MyStock, bool> f, Func<MyStock, string> o)
        {
            var a1 = list.Where(f).OrderByDescending(p => p.S1).Take(top);        
            var a2 = list.Where(f).OrderByDescending(p => p.S2).Take(top);
            var a3 = list.Where(f).OrderByDescending(p => p.S3).Take(top);
            var a4 = list.Where(f).OrderByDescending(p => p.S4).Take(top);
            var b1 = a1.Union(a2);
            var b2 = a3.Union(a4);
            var b = b1.Where(p => b2.Count(q => q.Code == p.Code) > 0).Distinct();
            var ret = new List<string>() { Calc(b).ToString("0.00") };
            return ret.Union(b.OrderByDescending(p => p.K1+ p.K2+ p.K3+ p.K4).Select(o)).ToArray();
        }

        /// <summary>
        /// GetS
        /// </summary>
        /// <param name="list"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static IEnumerable<MyStock> GetS(List<MyStock> list, int top)
        {
            var list1 = list.Where(F1).OrderByDescending(p => p.S1).Take(top);
            var list2 = list.Where(F1).OrderByDescending(p => p.S2).Take(top);
            var list3 = list.Where(F1).OrderByDescending(p => p.S3).Take(top);
            var list4 = list.Where(F1).OrderByDescending(p => p.S4).Take(top);
            return list1.Union(list2).Union(list3).Union(list4).Distinct();
        }


        /// <summary>
        /// 处理股票输出
        /// </summary>
        /// <param name="AQS"></param>
        /// <param name="Wave"></param>
        /// <param name="encode"></param>
        public static void Deal(List<MyStock> AQS, List<MyStock> Wave, string encode = "utf-8")
        {
            var tmp = Union(AQS, Wave);
            var list = tmp.Where(p => !p.Code.StartsWith("8"));
            var bk = tmp.Where(p => p.Code.StartsWith("8"));
            var t = list.Sum(p => p.Amount) / 400 * 0.382;
            Console.WriteLine("{0}亿", (t / 1e8).ToString("0.00"));
            string dir = "./dest", dirK = "./dest/K", dirA = "./dest/A", dirBk = "./dest/Bk";
            var KContent = new List<Tuple<double, double>>();
            var AContent = new List<Tuple<double, double>>();
            var BkContent = new List<Tuple<double, double>>();
            for (var i = 25; i <= 400; i += 25)
            {
                //K系列
                var kContent = GetStockName(list, i, F1, O1);
                KContent.Add(new Tuple<double, double>(kContent[0].Convert2Double(-10000), (kContent.Length - 1.0) / i));
                FileBase.WriteFile(dirK, "K" + i + ".txt", string.Join("\t\n", kContent), encode, FileBaseMode.Create);
                //A系列
                var aContent = GetStockName(list, i, p => F4(p, t), O2);
                AContent.Add(new Tuple<double, double>(aContent[0].Convert2Double(-10000), (aContent.Length - 1.0) / i));
                FileBase.WriteFile(dirA, "A" + i + ".txt", string.Join("\t\n", aContent), encode, FileBaseMode.Create);
            }

            var bkc = bk.Count();
            for (var i = 3; i <= bkc; i += 3)
            {
                //BK系列
                var kContent = GetStockName(bk, i, F1, O2);
                BkContent.Add(new Tuple<double, double>(kContent[0].Convert2Double(-10000), (kContent.Length - 1.0) / i));
                FileBase.WriteFile(dirBk, "BK" + i + ".txt", string.Join("\t\n", kContent), encode, FileBaseMode.Create);
            }

            //K系列
            FileBase.WriteFile(dirK, "K500.txt", string.Join("\t\n", GetStockName(list, 500, F1, O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirK, "K825.txt", string.Join("\t\n", GetStockName(list, 825, F1, O1)), encode, FileBaseMode.Create);
            //A系列
            FileBase.WriteFile(dirA, "A500.txt", string.Join("\t\n", GetStockName(list, 500, p => F4(p, t), O2)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirA, "A825.txt", string.Join("\t\n", GetStockName(list, 825, p => F4(p, t), O2)), encode, FileBaseMode.Create);

            var KConsole = GetAnswer(KContent);
            var AConsole = GetAnswer(AContent);

            Console.WriteLine("KContent:价格高低点:{0};比例高低点:{1}", string.Join("-", KConsole.Item1), string.Join("-", KConsole.Item2));
            Console.WriteLine("AContent:价格高低点:{0};比例高低点:{1}", string.Join("-", AConsole.Item1), string.Join("-", AConsole.Item2));
            
            FileBase.WriteFile(dir, "K.txt", string.Join("\t\n", KContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "A.txt", string.Join("\t\n", AContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Bk.txt", string.Join("\t\n", BkContent.Select((p, index) => (index + 1) * 3 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
        }

        /// <summary>
        /// 处理板块输出
        /// </summary>
        /// <param name="list"></param>
        /// <param name="encode"></param>
        public static void Deal2(List<MyStock> list, string encode = "utf-8")
        {
            var dirB = "./dest/B";
            for (var i = 3; i <= 48; i += 3)
            {
                var bContent = GetStockName(list, i, F3, O2);
                FileBase.WriteFile(dirB, "B" + i + ".txt", string.Join("\t\n", bContent), encode, FileBaseMode.Create);
            }
        }
        #endregion

        #region 盯盘监控

        /// <summary>
        /// 盘中风控指数监控
        /// </summary>
        public static decimal MonitorIndex()
        {
            var a = StockDataHelper.GetIndexPrice("sh000001");
            var b = StockDataHelper.GetIndexPrice("sz399001");
            var c = StockDataHelper.GetIndexPrice("sz399005");
            var d = StockDataHelper.GetIndexPrice("sz399006");
            var count = (a != null ? 1 : 0) + (b != null ? 1 : 0) + (c != null ? 1 : 0) + (d != null ? 1 : 0);
            var inc = a?.Inc + b?.Inc + c?.Inc + d?.Inc;
            var e = count > 0 ? inc.Value / count : 0;
            if (a.Inc > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine("上证:{0}%,深圳:{1}%,中小:{2}%,创业:{3}%,综合:{4}%", a.Inc.ToString("0.00"), b.Inc.ToString("0.00"), c.Inc.ToString("0.00"), d.Inc.ToString("0.00"), e.ToString("0.00"));
            return e;
        }

        /// <summary>
        /// 自选板块数据
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetMyMonitorStock(MyStockType mode)
        {
            var file = mode == MyStockType.Top ? "./src/dp/龙头.txt" :
                mode == MyStockType.Continie ? "./src/dp/接力.txt" :
                mode == MyStockType.ShortContinie ? "./src/dp/短线接力.txt" :
                mode == MyStockType.ShortTrend ? "./src/dp/短线趋势接力.txt" :
                 mode == MyStockType.Trend ? "./src/dp/趋势接力.txt" :
                 mode == MyStockType.AR ? "./src/dp/分时指标.txt" :
                 mode == MyStockType.R1 ? "./src/dp/R1.txt" : "./src/dp/接力.txt";
            var list1 = Regex.Split(FileBase.ReadFile(file, "gb2312"), "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<StockPrice>();
            foreach (var item in list1)
            {
                var t = item.Split('\t');
                if (t.Length >= 15)
                {
                    ret.Add(new StockPrice()
                    {
                        StockCode = t[0].Trim(),
                        StockName = t[1],
                        CurrentPrice = t[3].Convert2Decimal(0),
                        OpenPrice = t[11].Convert2Decimal(0),
                        MaxPrice = t[12].Convert2Decimal(0),
                        MinPrice = t[13].Convert2Decimal(0),
                        LastClosePrice = t[14].Convert2Decimal(0),
                        MyStockType = mode,
                    });
                }
            }
            return ret;
        }

        /// <summary>
        /// 盯盘
        /// </summary>
        /// <param name="Top">龙头</param>
        /// <param name="Continue">接力</param>
        /// <param name="Trend">趋势</param>
        /// <param name="ShortContinue">短线接力</param>
        /// <param name="ShortTrend">短线趋势</param>
        /// <param name="R1">短线分时</param>
        /// <param name="AR">短线分时-All</param> 
        /// <param name="AQS"></param>
        /// <param name="Wave"></param>
        /// <param name="e"></param>
        public static void MonitorStock(List<StockPrice> Top, List<StockPrice> Continue, List<StockPrice> Trend,
            List<StockPrice> ShortContinue, List<StockPrice> ShortTrend, List<StockPrice> R1, List<StockPrice> AR, 
            List<MyStock> AQS, List<MyStock> Wave, decimal e)
        {
            //开盘时间
            var tradeStart = ConfigurationHelper.GetAppSettingByName("TradeStart", new DateTime(2099, 1, 1, 9, 15, 0));
            //收盘时间
            var tradeEnd = ConfigurationHelper.GetAppSettingByName("TradeEnd", new DateTime(2099, 1, 1, 15, 0, 0)); 
            //盯盘模式，
            var dpmode = ConfigurationHelper.GetAppSettingByName("DpMode", 1);
            //盯盘过滤器
            var dpFilter = ConfigurationHelper.GetAppSettingByName("filter", 3);

            #region 盯盘模式
            List<StockPrice> _Continue = null;
            List<StockPrice> _Trend = null;
            if (dpmode == 1)
            {
                _Continue = Continue;
                _Trend = Trend;
            }
            else if (dpmode == 2)
            {
                _Continue = ShortContinue;
                _Trend = ShortTrend;
            }
            else if (dpmode == 3)
            {
                _Continue = Continue.Intersect(ShortContinue).ToList();
                _Trend = Trend.Intersect(ShortTrend).ToList();
            }
            else if (dpmode == 4)
            {
                _Continue = Continue.Union(ShortContinue).ToList();
                _Trend = Trend.Union(ShortTrend).ToList();
            }
            else if (dpmode == 0)
            {
                _Continue = AR;
                _Trend = AR.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            #endregion

            #region 盯盘过滤器
            var _top = Top;
            Func<StockPrice, bool> filter = p => true;
            //强势过滤
            bool top(StockPrice p) => _top.ToList().Exists(q => q.StockCode == p.StockCode);
            //趋势过滤
            bool trend(StockPrice p) => _Trend.Exists(q => q.StockCode == p.StockCode);
            //不过滤
            bool none(StockPrice p) => _top.Union(_Trend).ToList().Exists(q => q.StockCode == p.StockCode);

            if (dpFilter == 1)
            {
                filter = top;
            }
            else if (dpFilter == 2)
            {
                filter = trend;
            }
            else if (dpFilter == 3)
            {
                filter = none;
            }
            #endregion

            #region 建模提取数据
            var m1 = new List<MyStockMonitor>();
            foreach (var item in _Continue.Union(_top).Where(p => p.CurrentPrice > 0 && filter(p)))
            {
                var t = StockDataHelper.GetStockPrice(item.StockCode);
                decimal a = 0.01M, b = 0.01M;
                try
                {
                    a = t.MaxPrice / item.MaxPrice * t.MinPrice / item.MinPrice * 61.8M + 38.2M * t.CurrentPrice / item.CurrentPrice - 100;
                    b = t.MaxPrice / item.MaxPrice * t.MinPrice / item.MinPrice * 61.8M + 38.2M * t.OpenPrice / t.LastClosePrice - 100;
                    if (t.MinPrice < item.MinPrice)
                    {
                        b = t.CurrentPrice / t.MinPrice * t.CurrentPrice / t.MaxPrice * 61.8M + 38.2M * t.OpenPrice / t.LastClosePrice - 100;
                    }
                }
                catch { }
                if (!m1.Exists(p => p.StockCode == item.StockCode))
                {
                    var last = Union(AQS, Wave).FirstOrDefault(p => p.Code == t.StockCode);
                    if (last != null)
                    {
                        m1.Add(new MyStockMonitor
                        {
                            MyStockType = item.MyStockType,
                            StockCode = t.StockCode,
                            StockName = t.StockName,
                            Inc = t.Inc,
                            Price = t.CurrentPrice,
                            S = last.S1,
                            K = a,
                            L = b,
                            Amount = t.Amount,
                            AmountRate = (double)t.Amount * 1e8 / last.Amount * 100,
                            VolRate = (double)t.Vol / last.Vol * 100,
                            Buy1 = t.Buy1 * t.CurrentPrice / 100000000,
                            Sell1 = t.Sell1 * t.CurrentPrice / 100000000,
                        });
                    }
                }
            }
            #endregion

            #region 输出配置
            var topCount = ConfigurationHelper.GetAppSettingByName("topCount", 15);
            topCount = Math.Min(Math.Max(9, topCount), 39);
            //是否大成交模式
            var isBig = ConfigurationHelper.GetAppSettingByName("isBig", false);
            //大成交金额阈值
            var bigAmount = isBig ? ConfigurationHelper.GetAppSettingByName("bigAmount", 20M) : 0;
            bool bigFilter(MyStockMonitor p) => p.Amount >= bigAmount;
            #endregion

            #region 输出
            IEnumerable<MyStockMonitor> m2 = null;
            var dt = DateTime.Now;
            if (dt.TimeOfDay <= tradeEnd.AddMinutes(-15).TimeOfDay && dt.TimeOfDay >= tradeStart.TimeOfDay)
            {
                m2 = m1.Where(p => p.KLevel >= 7 && bigFilter(p)).OrderByDescending(p => p.KLevel).ThenByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (dt.TimeOfDay > tradeEnd.AddMinutes(-15).TimeOfDay)
            {
                m2 = m1.Where(p => p.LLevel >= 4 && bigFilter(p)).OrderByDescending(p => p.LLevel).ThenByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else
            {
                m2 = m1.OrderByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            if (m2 != null && m2.Count() > 0)
            {
                foreach (var t in m2)
                {
                    //龙头
                    var __top = Top.Exists(p => p.StockCode == t.StockCode);
                    //趋势
                    var __trend = _Trend.Exists(p => p.StockCode == t.StockCode);
                    //分时指标
                    var __r1 = R1.Exists(p => p.StockCode == t.StockCode);
                    var tip = "套利股";
                    if (t.Inc > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (__top)
                    {
                        Console.BackgroundColor = __r1 ? ConsoleColor.Gray : ConsoleColor.White;// : ConsoleColor.Cyan;
                        tip = __top ? "龙头强势股" : "强势股";
                    }
                    else if (__trend)
                    {
                        Console.BackgroundColor = __r1 ? ConsoleColor.Yellow : ConsoleColor.White;
                        tip = "趋势股";
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    var tip1 = string.Empty;
                    if (t.Buy1 >= 0.1M)
                    {
                        tip1 += "买一:" + t.Buy1.ToString("0.00") + "亿;";
                    }
                    if (t.Sell1 >= 0.1M)
                    {
                        tip1 += "卖一:" + t.Sell1.ToString("0.00") + "亿;";
                    }
                    if (__r1)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        tip1 += "分时达标";
                    }

                    Console.WriteLine("{0}-{1}:{2}({3})涨幅;{4}%,价格;{5},金额比例;{12}%,量能比例;{13}%,K;{6},KLevel;{7},S;{8},SLevel;{9},指数风控:{10}%,成交金额:{11}亿,{14}",
                        DateTime.Now.ToString("MM-dd HH:mm:ss"), tip,
                        t.StockName, t.StockCode, t.Inc.ToString("0.00"), t.Price, t.K.ToString("0.00"), t.KLevel,
                        t.S.ToString("0.00"), t.SLevel, e.ToString("0.00"), t.Amount.ToString("0.00"),
                        t.AmountRate.ToString("0.00"), t.VolRate.ToString("0.00"), tip1);
                }
            }
            #endregion
        }
        #endregion

        public static void Program()
        {
            //开盘时间
            var tradeStart = ConfigurationHelper.GetAppSettingByName("TradeStart", new DateTime(2099, 1, 1, 9, 15, 0));
            //收盘时间
            var tradeEnd = ConfigurationHelper.GetAppSettingByName("TradeEnd", new DateTime(2099, 1, 1, 15, 0, 0));
            //运行模式
            var mode = ConfigurationHelper.GetAppSettingByName("mode", 0);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            var dt = DateTime.Now;
            //盯盘
            if (mode == 1 || (mode == 0 && dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && dt.TimeOfDay <= tradeEnd.TimeOfDay))
            {
                //龙头
                var top = GetMyMonitorStock(MyStockType.Top);
                //接力
                var Continue = GetMyMonitorStock(MyStockType.Continie);
                //趋势
                var trend = GetMyMonitorStock(MyStockType.Trend);
                //短线接力
                var shortContinue = GetMyMonitorStock(MyStockType.ShortContinie);
                //短线趋势
                var shortTrend = GetMyMonitorStock(MyStockType.ShortTrend);
                //短线分时
                var r1 = GetMyMonitorStock(MyStockType.R1);
                //短线分时-All
                var ar = GetMyMonitorStock(MyStockType.AR);

                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                while (dt.TimeOfDay >= tradeStart.TimeOfDay && dt.TimeOfDay <= tradeEnd.TimeOfDay)
                {
                    var e = MonitorIndex();
                    MonitorStock(top, Continue, trend, shortContinue, shortTrend, r1, ar, AQS, Wave, e);
                    Thread.Sleep(6000);
                    dt = DateTime.Now;
                }
            }
            //复盘
            if (mode == 2 || (mode == 0 && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday ||
                dt.TimeOfDay > tradeEnd.TimeOfDay || dt.AddMinutes(30).TimeOfDay <= tradeStart.TimeOfDay)))
            {
                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                Deal(AQS, Wave);
                var t2 = GetMyStock(MyStockMode.Index);
                Deal2(t2);
            }
            else if (mode == 3)
            {
                //龙头
                var top = GetMyMonitorStock(MyStockType.Top);
                //接力
                var Continue = GetMyMonitorStock(MyStockType.Continie);
                //趋势
                var trend = GetMyMonitorStock(MyStockType.Trend);
                //短线接力
                var shortContinue = GetMyMonitorStock(MyStockType.ShortContinie);
                //短线趋势
                var shortTrend = GetMyMonitorStock(MyStockType.ShortTrend);
                //短线分时
                var r1 = GetMyMonitorStock(MyStockType.R1);
                //短线分时-All
                var ar = GetMyMonitorStock(MyStockType.AR);
                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                var e = MonitorIndex();
                MonitorStock(top, Continue, trend, shortContinue, shortTrend, r1, ar, AQS, Wave, e);
            }
            Console.WriteLine("Program End! Press Any Key!");
            Console.ReadKey();
        }
    }
}
