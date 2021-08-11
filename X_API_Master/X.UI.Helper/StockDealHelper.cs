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
            var file = mode == MyStockMode.JX ? "./src/dp/精选.txt" :
                 mode == MyStockMode.AQS ? "./src/fp/AQS.txt" :
                 mode == MyStockMode.JX2 ? "./src/dp/精选2.txt" :
                 mode == MyStockMode.Kernel ? "./src/dp/Kernel精选.txt" :
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
                        NF = t[15].Convert2Double(0),
                        KLL = t[16].Convert2Double(-100),
                        SP = t[17].Convert2Double(-100),
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
        public static void Deal(List<MyStock> AQS, List<MyStock> Wave, string encode = "utf-8")
        {
            var tmp = Union(AQS, Wave);
            //原始数据处理
            var list = tmp.Where(p => !p.Code.StartsWith("8"));

            var bk = tmp.Where(p => p.Code.StartsWith("8"));
            var t = list.Sum(p => p.Amount) / 400 * 0.382;
            Console.WriteLine("{0}亿", (t / 1e8).ToString("0.00"));
            string dir = "./dest", dirK = "./dest/K", dirA = "./dest/A", dirBk = "./dest/Bk";
            var KContent = new List<Tuple<double, double>>();
            var AContent = new List<Tuple<double, double>>();
            var BkContent = new List<Tuple<double, double>>();

            Func<MyStock, bool> _F5 = F3;

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
        #endregion

        #region 选股逻辑
        /// <summary>
        /// FilterStock 选股
        /// </summary>
        /// <param name="First"></param>
        /// <param name="ZT"></param>
        /// <param name="Kernel"></param>
        /// <param name="KernelH"></param>
        /// <param name="KernelL"></param>
        /// <param name="Core"></param>
        /// <param name="Core2"></param>
        /// <param name="Core3"></param>
        /// <param name="DDXList"></param>
        /// <param name="AQS"></param>
        /// <param name="All"></param>
        public static void FilterStock(List<StockPrice> First, List<StockPrice> ZT, List<StockPrice> Kernel, 
            List<StockPrice> KernelH, List<StockPrice> KernelL, List<StockPrice> Core, List<StockPrice> Core2, 
            List<StockPrice> Core3, List<StockPrice> DDXList, List<MyStock> AQS, List<MyStock> All)
        {
            var topCount = ConfigurationHelper.GetAppSettingByName("topCount", 15);
            List<StockPrice> OP = Kernel;
            List<StockPrice> Top = ZT.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();

            List<StockPrice> _Continue = OP;
            List<StockPrice> _Trend = OP.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();

            #region 过滤器
            var _top = Top;
            Func<StockPrice, bool> filter = p => true;
            //强势过滤
            bool top(StockPrice p) => _top.ToList().Exists(q => q.StockCode == p.StockCode);
            //趋势过滤
            bool trend(StockPrice p) => _Trend.Exists(q => q.StockCode == p.StockCode);
            //首板过滤
            bool first(StockPrice p) => First.Exists(q => q.StockCode == p.StockCode);
            //半路过滤
            bool zt(StockPrice p) => ZT.All(q => q.StockCode != p.StockCode);
            //连板过滤
            bool lb(StockPrice p) => ZT.Exists(q => q.StockCode == p.StockCode) && First.All(q => q.StockCode != p.StockCode);

            _top = new List<StockPrice>();
            #endregion
            Console.WriteLine("开始选股");
            #region 选股
            var list = new Dictionary<string, MyStockMonitor>();
            for (var i = 0; i <= 6; i++)
            {
                Func<StockPrice, bool> debugFilter = p => true;
                List<StockPrice> debugTop = Top;
                var remark = "|TOP";
                switch (i)
                {
                    case 1:
                        debugFilter = top;
                        remark = "|龙头";
                        break;
                    case 2:
                        debugFilter = trend;
                        remark = "|趋势";
                        break;
                    case 3:
                        debugFilter = lb;
                        debugTop = new List<StockPrice>();
                        remark = "|连板";
                        break;
                    case 4:
                        debugFilter = first;
                        debugTop = new List<StockPrice>();
                        remark = "|首板";
                        break;
                    case 5:
                        debugFilter = zt;
                        debugTop = new List<StockPrice>();
                        remark = "|半路";
                        break;
                    case 6:
                        debugFilter = p => first(p) || zt(p);
                        debugTop = new List<StockPrice>();
                        remark = "|低位";
                        break;
                }
                var mainDebug = _Continue.Union(debugTop).Where(p => p.CurrentPrice > 0 && debugFilter(p));
                var listDebug = new List<MyStockMonitor>();
                foreach (var item in mainDebug)
                {
                    var last = All.FirstOrDefault(p => p.Code == item.StockCode);
                    if (!listDebug.Exists(p => p.StockCode == item.StockCode))
                    {
                        var orderremark = "MIDDLE";
                        if (KernelH.Exists(p => p.StockCode == item.StockCode))
                        {
                            orderremark = "HIGH";
                        }
                        else if (KernelL.Exists(p => p.StockCode == item.StockCode))
                        {
                            orderremark = "LOW";
                        }

                        var orderremark2 = DDXList.Exists(p => p.StockCode == item.StockCode) ? "ddx" : "0";
                        var orderremark3 = string.Empty;
                        if (Core.Exists(p => p.StockCode == item.StockCode))
                        {
                            orderremark3 = "7";
                        }
                        else if (Core2.Exists(p => p.StockCode == item.StockCode))
                        {
                            orderremark3 = "3";
                        }
                        else if (Core3.Exists(p => p.StockCode == item.StockCode))
                        {
                            orderremark3 = "1";
                        }

                        listDebug.Add(new MyStockMonitor()
                        {
                            MyStockType = item.MyStockType,
                            StockCode = item.StockCode,
                            StockName = item.StockName,
                            Inc = item.Inc,
                            Price = item.CurrentPrice,
                            S = last != null ? last.S1 : 0,
                            NF = last != null ? last.NF.ToString("0") : "DEFAULT",
                            KLL = last != null ? last.KLL.ToString("0") : "DEFAULT",
                            Amount = item.Amount,
                            OrderRemark = orderremark,
                            OrderRemark2 = orderremark2,
                            OrderRemark3 = orderremark3
                        });
                    }
                }
                var j = 0;
                foreach (var item in listDebug.OrderByDescending(p => p.SLevel).ThenByDescending(p => p.Inc))
                {
                    if (i > 0 && j < topCount || i == 0)
                    {
                        if (OP.Exists(p => p.StockCode == item.StockCode))
                        {
                            if (!list.ContainsKey(item.StockCode))
                            {
                                list[item.StockCode] = item;
                            }
                            list[item.StockCode].Remark += remark + (j + 1);
                        }
                    }
                    j++;
                }
            }
            FileBase.WriteFile("./", "dest.txt", string.Join("\t\n", list.OrderByDescending(p => p.Value.SLevel)
                .ThenByDescending(p => p.Value.Inc).
                Select(p => p.Value.StockCode + " " + p.Value.StockName + " " + p.Value.Remark + " " + p.Value.OrderRemark2
                + " " + p.Value.OrderRemark + " " + p.Value.OrderRemark3 + " " + p.Value.KLL + " " + p.Value.NF)), "utf-8", FileBaseMode.Create);
            #endregion
            Console.WriteLine("选股完成");
        }
        #endregion

        #region 复盘数据加工
        /// <summary>
        /// 复盘数据映射
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetMyMonitorStock(MyStockType mode)
        {
            var file = mode == MyStockType.Continie ? "./src/dp/接力.txt" :
                mode == MyStockType.ShortContinie ? "./src/dp/短线接力.txt" :
                mode == MyStockType.First ? "./src/dp/首板.txt" :
                mode == MyStockType.ZT ? "./src/dp/涨停.txt" :
                mode == MyStockType.CoreT ? "./src/dp/CORET.txt" :
                mode == MyStockType.CoreT2 ? "./src/dp/CORET2.txt" :
                mode == MyStockType.CoreT3 ? "./src/dp/CORET3.txt" :
                mode == MyStockType.Kernel ? "./src/dp/Kernel.txt" :
                mode == MyStockType.KernelH ? "./src/dp/KernelH.txt" :
                mode == MyStockType.KernelL ? "./src/dp/KernelL.txt" : "./src/dp/接力.txt";
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
        /// 资金流文件
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetDDXList()
        {
            var file = "./src/dp/tool.txt";
            var list1 = Regex.Split(FileBase.ReadFile(file, "gb2312"), "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<StockPrice>();
            foreach (var item in list1)
            {
                var t = item.Split('\t');
                if (t.Length >= 6)
                {
                    var ddx = t[5].Convert2Decimal(-1);
                    if (ddx > 0)
                    {
                        ret.Add(new StockPrice()
                        {
                            StockCode = t[1].Trim(),
                            StockName = t[2],
                        });
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 资金流文件
        /// </summary>
        /// <returns></returns>
        public static List<StockCompare> GetDDXList2()
        {
            var file = "./src/dp/tool.txt";
            var list1 = Regex.Split(FileBase.ReadFile(file, "gb2312"), "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<StockCompare>();
            foreach (var item in list1)
            {
                var t = item.Split('\t');
                if (t.Length >= 6)
                {
                    var ddx = t[5].Convert2Decimal(-1);
                    ret.Add(new StockCompare()
                    {
                        Code = t[1].Trim(),
                        Name = t[2],
                        DDX = ddx,
                        Inc = t[4].Convert2Decimal(-1)
                    });
                }
            }
            return ret;
        }

        /// <summary>
        /// 对数据进行模式自动分类
        /// </summary>
        /// <param name="ddxList"></param>
        /// <param name="JX"></param>
        /// <param name="Core"></param>
        /// <returns></returns>
        public static Dictionary<string, ModeCompare> GetModeCompareAuto(List<StockCompare> ddxList, List<MyStock> JX, List<StockPrice> Core)
        {
            Dictionary<string, ModeCompare> ret = new Dictionary<string, ModeCompare>();
            foreach (var item in JX.Where(p => !p.Name.Contains("转债")))
            {
                if (item.SP > 0)
                {
                    string key = item.SP.ToString();
                    if (Core.Exists(p => p.StockCode == item.Code))
                    {
                        key = (item.SP + 3).ToString();
                    }
                    if (!ret.ContainsKey(key))
                    {
                        ret[key] = new ModeCompare()
                        {
                            CodeList = new List<StockCompare>()
                        };
                    }
                    var ddx = ddxList.FirstOrDefault(p => p.Code == item.Code);
                    if (ddx == null)
                    {
                        Console.WriteLine(item.Name + "找不到ddx");
                        continue;
                    }
                    ddx.Mode = key;
                    ret[key].CodeList.Add(ddx);
                    ret[key].Name = key;
                }
            }
            return ret;
        }

        /// <summary>
        /// 手动输入外置模式
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<string, string[]>> GetFilterListFromFile()
        {
            var file = "./src/dp/bk.txt";
            var content = FileBase.ReadFile(file, "utf-8");
            var list = Regex.Matches(content, "(.+)：【(.+)】");
            var ret = new List<Tuple<string, string[]>>();
            foreach (Match item in list)
            {
                ret.Add(new Tuple<string, string[]>(item.Groups[1].Value, item.Groups[2].Value.Split('-')));
            }
            return ret;
        }

        /// <summary>
        /// 对外置模式数据统一格式
        /// </summary>
        /// <param name="ddxList"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Dictionary<string, ModeCompare> GetModeCompare(List<StockCompare> ddxList, List<Tuple<string, string[]>> filter)
        {
            Dictionary<string, ModeCompare> ret = new Dictionary<string, ModeCompare>();
            for (var i = 0; i < filter.Count; i++)
            {
                if (filter[i].Item2.Length > 0)
                {
                    var mode = new ModeCompare()
                    {
                        CodeList = new List<StockCompare>()
                    };
                    foreach (var item in filter[i].Item2)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var select = ddxList.FirstOrDefault(p => p.Name == item);
                            select.Mode = filter[i].Item1;
                            mode.CodeList.Add(select);
                            mode.Name = filter[i].Item1;
                        }
                    }
                    ret[filter[i].Item1] = mode;
                }
            }
            return ret;
        }

        /// <summary>
        /// 输出模式结果
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, ModeCompare>> GetModeCompareWithOrder(Dictionary<string, ModeCompare> mode, string remark)
        {
            var newMode = new Dictionary<string, ModeCompare>();
            var list = new List<StockCompare>();
            foreach (var item in mode)
            {
                list = list.Concat(item.Value.CodeList).ToList();
            }
            list = list.OrderByDescending(p => p.DDX).ToList();

            for (var i = 0; i < list.Count; i++)
            {
                list[i].DDXOrder = i + 1;
                if (!newMode.ContainsKey(list[i].Mode))
                {
                    newMode[list[i].Mode] = new ModeCompare()
                    {
                        Name = list[i].Mode,
                        CodeList = new List<StockCompare>()
                        {
                            list[i]
                        }
                    };
                }
                else
                {
                    newMode[list[i].Mode].CodeList.Add(list[i]);
                }
            }
            var stdOrder = 0.5 * list.Count;
            var stdInc = list.Average(p => p.Inc);
            var stdDDX = list.Average(p => p.DDX);

            var ret = newMode.Where(p => p.Value.DDX >= stdDDX && p.Value.Inc >= stdInc && p.Value.DDXOrder <= stdOrder);
            Console.WriteLine(remark);
            foreach (var item in ret)
            {
                Console.WriteLine("key:{0},value:{1}", item.Key, string.Join("-", item.Value.CodeList.Select(p => p.Name)));
                Console.WriteLine("----------");
            }
            Console.WriteLine("当前模块结束");
            return ret;
        }

        /// <summary>
        /// GetStockResult 收盘统计
        /// </summary>
        /// <param name="JX"></param>
        /// <param name="JX2"></param>
        /// <param name="Core"></param>
        public static void GetStockResult(List<MyStock> JX, List<MyStock> JX2, List<StockPrice> Core)
        {
            var list = JX.Union(JX2);
            var tmp = new string[] { "试错突破", "试错加速", "试错回踩", "低位突破", "低位加速", "低位回踩", "高位突破", "高位加速", "高位回踩" };
            Dictionary<double, List<string>> ret = new Dictionary<double, List<string>>();
            foreach (var item in list)
            {
                var t = StockDataHelper.GetStockPrice(item.Code);
                if (t != null)
                {
                    if (t.Inc < 5) continue;
                    var sp = item.SP;
                    if (Core.Exists(p => p.StockCode == item.Code))
                    {
                        sp += 3;
                    }
                    if (!ret.ContainsKey(sp))
                    {
                        ret[sp] = new List<string>() { item.Name };
                    }
                    else
                    {
                        ret[sp].Add(item.Name);
                    }
                }
            }
            foreach (var item in ret)
            {
                Console.WriteLine("{0}:{1}:{2}", DateTime.Now.ToString("HH:mm:ss.fff"), tmp[(int)item.Key - 1], string.Join("-", item.Value));
            }
            
        }


        #endregion

        #region 盯盘监控
        /// <summary>
        /// 成交金额集合
        /// </summary>
        private static Dictionary<DateTime, double> TradeAmount = new Dictionary<DateTime, double>();

        /// <summary>
        /// 预估成交金额
        /// </summary>
        public static void CalcAmount()
        {
            if (TradeAmount.Count <= 0) return;
            var max = TradeAmount.Max(p => p.Key);
            var list = TradeAmount.Where(p => p.Key >= max.AddSeconds(-1000)).OrderByDescending(p => p.Key);
            if (list.Count() >= 15)
            {
                var first = default(KeyValuePair<DateTime, double>);
                double k = 0.5, calc = 0.0;
                DateTime dt = DateTime.Now, end = new DateTime(dt.Year, dt.Month, dt.Day, 15, 0, 0);
                foreach (var item in list.OrderByDescending(p => p.Key))
                {
                    if (first.Equals(default(KeyValuePair<DateTime, double>))) first = item;
                    else
                    {
                        calc += k * (first.Value - item.Value) / (first.Key - item.Key).TotalSeconds;
                        k *= 0.5;
                    }
                }
                var y = (end - first.Key).TotalSeconds * calc + first.Value;
                Console.WriteLine("两市预估成交金额：{0}亿", y.ToString("0.00"));
            }
        }

        /// <summary>
        /// 盘中风控指数监控
        /// </summary>
        public static void MonitorIndex()
        {
            var a = StockDataHelper.GetIndexPrice("sh000001") ?? new StockPrice() { Inc = -10 };
            var b = StockDataHelper.GetIndexPrice("sz399001") ?? new StockPrice() { Inc = -10 };
            var c = StockDataHelper.GetIndexPrice("sz399005") ?? new StockPrice() { Inc = -10 };
            var d = StockDataHelper.GetIndexPrice("sz399006") ?? new StockPrice() { Inc = -10 };
            if (a.Inc > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine("上证:{0}%,深圳:{1}%,中小:{2}%,创业:{3}%", a.Inc.ToString("0.00"), b.Inc.ToString("0.00"), c.Inc.ToString("0.00"), d.Inc.ToString("0.00"));
            if (!TradeAmount.ContainsKey(a.Datetime) && a.Amount >= 100)
            {
                TradeAmount.Add(a.Datetime, (double)(a.Amount + b.Amount));
            }
            CalcAmount();
        }

        /// <summary>
        /// MonitorStock
        /// </summary>
        /// <param name="AQS"></param>
        /// <param name="All"></param>
        /// <param name="SP"></param>
        public static void MonitorStock(List<MyStock> AQS, List<MyStock> All, List<MyStock> Jx, List<MyStock> Jx2, List<string> SP)
        {
            //开盘时间
            var tradeStart = ConfigurationHelper.GetAppSettingByName("TradeStart", new DateTime(2099, 1, 1, 9, 15, 0));
            //收盘时间
            var tradeEnd = ConfigurationHelper.GetAppSettingByName("TradeEnd", new DateTime(2099, 1, 1, 15, 0, 0));
            List<MyStock> OP = All;
            List<MyStock> _Continue = All;
            List<MyStock> _Trend = OP.Where(p => AQS.Exists(q => q.Code == p.Code)).ToList();

            #region 建模提取数据
            var m1 = new List<MyStockMonitor>();
            var _mainLoop = All.Where(p => p.Close > 0);
            foreach (var item in _mainLoop.Where(p => SP.Exists(q => q == p.SP.ToString())))
            {
                var t = StockDataHelper.GetStockPrice(item.Code);
                if (t == null) continue;
                if (!m1.Exists(p => p.StockCode == item.Code))
                {
                    var last = All.FirstOrDefault(p => p.Code == item.Code);
                    var isHigh = Jx.Union(Jx2).ToList().Exists(p => p.Code == item.Code);
                    if (last != null)
                    {
                        m1.Add(new MyStockMonitor
                        {
                            StockCode = t.StockCode,
                            StockName = t.StockName,
                            Inc = t.Inc,
                            Price = t.CurrentPrice,
                            S = last.S1,
                            NF = last.NF.ToString("0"),
                            KLL = last.KLL.ToString("0"),
                            Amount = t.Amount,
                            AmountRate = (double)t.Amount * 1e8 / last.Amount * 100,
                            VolRate = (double)t.Vol / last.Vol * 100,
                            Buy1 = t.Buy1 * t.CurrentPrice / 100000000,
                            Sell1 = t.Sell1 * t.CurrentPrice / 100000000,
                            IsHigh = isHigh
                        });
                    }
                }
            }
            #endregion
            var topCount = ConfigurationHelper.GetAppSettingByName("topCount", 15);
            IEnumerable<MyStockMonitor> m2 = m1.OrderByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            if (m2 != null && m2.Count() > 0)
            {
                var i = 1;
                foreach (var item in m2.Where(p => p.Inc >= 3.82M))
                {
                    if (item.IsHigh)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.WriteLine("{10}-{0}-{1}({2}):涨跌幅{3}%，日成交{4}亿，买一{5}亿，卖一{6}亿，放量比例{7}%，S:{8},股价{9}",
                        DateTime.Now.ToString("HH:mm:ss"), item.StockName, item.StockCode, item.Inc.ToString("0.00"),
                        item.Amount.ToString("0.00"), item.Buy1.ToString("0.00"), item.Sell1.ToString("0.00"),
                        item.AmountRate.ToString("0.00"), item.S.ToString("0"), item.Price.ToString("0.00"), i);
                    i++;
                }
            }
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
                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                var ddx = GetDDXList2();
                var jx = GetMyStock(MyStockMode.JX);
                var jx2 = GetMyStock(MyStockMode.JX2);
                var core = GetMyMonitorStock(MyStockType.CoreT);
                var sp1 = GetModeCompareWithOrder(GetModeCompareAuto(ddx, jx, core), "精选-开始");
                var sp2 = GetModeCompareWithOrder(GetModeCompareAuto(ddx, jx2, core), "精选2-开始");
                var sp = sp1.Select(p => p.Key).Union(sp2.Select(p => p.Key)).ToList();
                var all = Union(AQS, Wave);
                while (dt.TimeOfDay >= tradeStart.TimeOfDay && dt.TimeOfDay <= tradeEnd.TimeOfDay)
                {
                    MonitorIndex();
                    MonitorStock(AQS, all, jx, jx2, sp);
                    GetStockResult(jx, jx2, core);
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
            }
            else if (mode == 3)
            {
                //首板
                var first = GetMyMonitorStock(MyStockType.First);
                //涨停
                var zt = GetMyMonitorStock(MyStockType.ZT);
                var kernel = GetMyMonitorStock(MyStockType.Kernel);
                var kernelH = GetMyMonitorStock(MyStockType.KernelH);
                var kernelL = GetMyMonitorStock(MyStockType.KernelL);
                var coreT = GetMyMonitorStock(MyStockType.CoreT);
                var coreT2 = GetMyMonitorStock(MyStockType.CoreT2);
                var coreT3 = GetMyMonitorStock(MyStockType.CoreT3);
                var ddx = GetDDXList();
                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                var all = Union(AQS, Wave);
                FilterStock(first, zt, kernel, kernelH, kernelL, coreT, coreT2, coreT3, ddx, AQS, all);
            }
            else if (mode == 4)
            {
                var jx = GetMyStock(MyStockMode.JX);
                var jx2 = GetMyStock(MyStockMode.JX2);
                var kernel = GetMyStock(MyStockMode.Kernel);
                var core = GetMyMonitorStock(MyStockType.CoreT);

                var a2 = GetFilterListFromFile();
                var b = GetDDXList2();
                GetModeCompareWithOrder(GetModeCompareAuto(b, jx, core), "精选-开始");
                GetModeCompareWithOrder(GetModeCompareAuto(b, jx2, core), "精选2-开始");
                GetModeCompareWithOrder(GetModeCompareAuto(b, kernel, core), "Kernel-开始");
                GetModeCompareWithOrder(GetModeCompare(b, a2), "板块-开始");
                GetStockResult(jx, jx2, core);
            }

            Console.WriteLine("Program End! Press Any Key!");
            Console.ReadKey();
        }

        /// <summary>
        /// 测试阈值
        /// </summary>
        public static void Test(int mode = 1)
        {
            var content = FileBase.ReadFile(@"D:\stock\股票工具\Debug\src\" + mode + ".txt", "gb2312");
            var list = Regex.Split(content, "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<decimal>();
            foreach (string item in list)
            {
                if (!string.IsNullOrEmpty(item))
                    ret.Add(item.Convert2Decimal(0));
            }
            List<int> H = new List<int>();
            List<int> L = new List<int>();
            List<decimal> HC = new List<decimal>();
            List<decimal> LC = new List<decimal>();
            List<decimal> secHC = new List<decimal>();
            List<decimal> secLC = new List<decimal>();

            List<int> LL = new List<int>();//两个低点之间的距离

            int lastH = 0;
            int lastL = 0;
            for (var i = 1; i < ret.Count - 1; i++)
            {
                if (ret[i] > ret[i + 1] && ret[i] > ret[i - 1])
                {
                    HC.Add(ret[i]);
                    secHC.Add(ret[i + 1]);
                    lastH = i;
                    if (lastL != 0)
                    {
                        L.Add(lastH - lastL + 1);
                    }
                }
                if (ret[i] < ret[i + 1] && ret[i] < ret[i - 1])
                {
                    LC.Add(ret[i]);
                    secLC.Add(ret[i + 1]);
                    if (lastL != 0)
                    {
                        LL.Add(i - lastL + 1);
                    }
                    lastL = i;
                    if (lastH != 0)
                    {
                        H.Add(lastL - lastH + 1);
                    }
                }
            }

            Console.WriteLine("L:" + L.Average());
            Console.WriteLine("H:" + H.Average());
            Console.WriteLine("Concat:" + L.Concat(H).Average());
            Console.WriteLine("LC:" + LC.Average());
            Console.WriteLine("HC:" + HC.Average());
            Console.WriteLine("Concat-C:" + LC.Concat(HC).Average());

            Console.WriteLine("secLC:" + secLC.Average());
            Console.WriteLine("secHC:" + secHC.Average());
            Console.WriteLine("Concat-secC:" + secLC.Concat(secHC).Average());

            Console.WriteLine("LL:" + LL.Average());

            Console.WriteLine("pp3:" + (LL.Count(p => p == 3) + 0.0) / LL.Count);
            Console.WriteLine("pp4:" + (LL.Count(p => p == 4) + 0.0) / LL.Count);
            Console.WriteLine("pp5:" + (LL.Count(p => p == 5) + 0.0) / LL.Count);
            Console.WriteLine("pp6:" + (LL.Count(p => p == 6) + 0.0) / LL.Count);
            Console.WriteLine("pp7:" + (LL.Count(p => p == 7) + 0.0) / LL.Count);
        }
    }
}
