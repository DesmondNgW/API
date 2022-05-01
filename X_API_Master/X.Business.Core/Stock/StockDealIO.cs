using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using X.Business.Entities.Enums;
using X.Business.Entities.Stock;
using X.Business.Helper.Stock;
using X.Util.Core;
using X.Util.Core.Log;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.Business.Core.Stock
{
    public class StockDealIO
    {
        #region 复盘逻辑
        /// <summary>
        /// 输入文件导入
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<MyStock> GetMyStock(MyStockMode mode)
        {
            var file = mode == MyStockMode.JX ? StockConstHelper.JXPATH :
                 mode == MyStockMode.AQS ? StockConstHelper.AQSPATH :
                 mode == MyStockMode.JX2 ? StockConstHelper.JXPATH2 :
                 mode == MyStockMode.Kernel ? StockConstHelper.KERNELJXPATH :
                 mode == MyStockMode.Wave ? StockConstHelper.WAVEPATH : StockConstHelper.AQSPATH;
            var content = FileBase.ReadFile(file, StockConstHelper.GB2312);
            var list = Regex.Split(content, StockConstHelper.RN, RegexOptions.IgnoreCase);
            var ret = new List<MyStock>();
            foreach (var item in list)
            {
                var t = item.Split(StockConstHelper.T);
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
            return ret;
        }

        /// <summary>
        /// 处理股票输出
        /// </summary>
        public static void Deal(List<MyStock> AQS, List<MyStock> Wave, string encode = "utf-8")
        {
            var tmp = StockDealBase.Union(AQS, Wave);
            //原始数据处理
            var list = tmp.Where(p => !p.Code.StartsWith("8"));

            var bk = tmp.Where(p => p.Code.StartsWith("8"));
            var t = list.Sum(p => p.Amount) / 400 * 0.382;
            Logger.Client.Debug(string.Format("{0}亿", (t / 1e8).ToString("0.00")), LogDomain.Business);
            string dir = StockConstHelper.DIRPATH, dirK = StockConstHelper.DIRKPATH, dirA = StockConstHelper.DIRAPATH,
                dirBk = StockConstHelper.DIRBKPATH;
            var KContent = new List<Tuple<double, double>>();
            var AContent = new List<Tuple<double, double>>();
            var BkContent = new List<Tuple<double, double>>();

            Func<MyStock, bool> _F5 = StockDealBase.F3;

            for (var i = 25; i <= 400; i += 25)
            {
                //K系列
                var kContent = StockDealBusiness.GetStockName(list, i, StockDealBase.F1, StockDealBase.O1);
                KContent.Add(new Tuple<double, double>(kContent[0].Convert2Double(-10000), (kContent.Length - 1.0) / i));
                FileBase.WriteFile(dirK, "K" + i + ".txt", string.Join(StockConstHelper.TN, kContent), encode, FileBaseMode.Create);
                //A系列
                var aContent = StockDealBusiness.GetStockName(list, i, p => StockDealBase.F4(p, t), StockDealBase.O2);
                AContent.Add(new Tuple<double, double>(aContent[0].Convert2Double(-10000), (aContent.Length - 1.0) / i));
                FileBase.WriteFile(dirA, "A" + i + ".txt", string.Join(StockConstHelper.TN, aContent), encode, FileBaseMode.Create);
            }

            var bkc = bk.Count();
            for (var i = 3; i <= bkc; i += 3)
            {
                //BK系列
                var kContent = StockDealBusiness.GetStockName(bk, i, StockDealBase.F1, StockDealBase.O2);
                BkContent.Add(new Tuple<double, double>(kContent[0].Convert2Double(-10000), (kContent.Length - 1.0) / i));
                FileBase.WriteFile(dirBk, "BK" + i + ".txt", string.Join(StockConstHelper.TN, kContent), encode, FileBaseMode.Create);
            }

            //K系列
            FileBase.WriteFile(dirK, "K500.txt", string.Join(StockConstHelper.TN, StockDealBusiness.GetStockName(list, 500, StockDealBase.F1, StockDealBase.O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirK, "K825.txt", string.Join(StockConstHelper.TN, StockDealBusiness.GetStockName(list, 825, StockDealBase.F1, StockDealBase.O1)), encode, FileBaseMode.Create);
            //A系列
            FileBase.WriteFile(dirA, "A500.txt", string.Join(StockConstHelper.TN, StockDealBusiness.GetStockName(list, 500, p => StockDealBase.F4(p, t), StockDealBase.O2)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirA, "A825.txt", string.Join(StockConstHelper.TN, StockDealBusiness.GetStockName(list, 825, p => StockDealBase.F4(p, t), StockDealBase.O2)), encode, FileBaseMode.Create);

            var KConsole = StockDealBase.GetAnswer(KContent);
            var AConsole = StockDealBase.GetAnswer(AContent);
            Logger.Client.Debug(string.Format("KContent:价格高低点:{0};比例高低点:{1}", string.Join("-", KConsole.Item1), string.Join("-", KConsole.Item2)), LogDomain.Business);
            Logger.Client.Debug(string.Format("AContent:价格高低点:{0};比例高低点:{1}", string.Join("-", AConsole.Item1), string.Join("-", AConsole.Item2)), LogDomain.Business);
            FileBase.WriteFile(dir, "K.txt", string.Join(StockConstHelper.TN, KContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "A.txt", string.Join(StockConstHelper.TN, AContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Bk.txt", string.Join(StockConstHelper.TN, BkContent.Select((p, index) => (index + 1) * 3 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
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
            List<StockPrice> Core3, List<StockPrice> LDX, List<StockPrice> DDXList, List<MyStock> AQS, List<MyStock> All, int TopCount)
        {
            List<StockPrice> OP = Kernel;
            List<StockPrice> Top = ZT.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();

            List<StockPrice> _Continue = OP;
            List<StockPrice> _Trend = OP.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();

            var des = GetStockDes(StockDesType.DateBase);

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
                        var ldx = LDX.Exists(p => p.StockCode == item.StockCode) ? "Y" : "N";

                        List<string> bks = new List<string>();
                        if (!ZT.Exists(p => p.StockCode == item.StockCode) && !First.Exists(p => p.StockCode == item.StockCode))
                        {
                            var firstDes = des.FirstOrDefault(p => p.Code == item.StockCode);
                            if (firstDes != null)
                            {
                                bks = firstDes.Bk;
                            }
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
                            OrderRemark3 = orderremark3,
                            BK = bks,
                            LDX = ldx
                        });
                    }
                }
                var j = 0;
                foreach (var item in listDebug.OrderByDescending(p => p.SLevel).ThenByDescending(p => p.Inc))
                {
                    if (i > 0 && j < TopCount || i == 0)
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
            FileBase.WriteFile("./", "dest.txt", string.Join(StockConstHelper.TN, list.OrderByDescending(p => p.Value.SLevel)
                .ThenByDescending(p => p.Value.Inc).
                Select(p => p.Value.StockCode + " " + p.Value.StockName + " " + string.Join("+", p.Value.BK) + " " + p.Value.Remark + " " + p.Value.OrderRemark2
                + " " + p.Value.OrderRemark + " " + p.Value.OrderRemark3 + " " + p.Value.LDX + " " + p.Value.KLL + " " + p.Value.NF)), "utf-8", FileBaseMode.Create);
            #endregion
        }
        #endregion

        #region 复盘数据加工
        /// <summary>
        /// 资金流文件
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetDDXList()
        {
            var file = StockConstHelper.TOOLPATH;
            var list1 = Regex.Split(FileBase.ReadFile(file, StockConstHelper.GB2312), StockConstHelper.RN, RegexOptions.IgnoreCase);
            var ret = new List<StockPrice>();
            foreach (var item in list1)
            {
                string[] t = StockDealBase.SplitFiledByRegex(item, 21, 2);
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
        /// 复盘数据映射
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetMyMonitorStock(MyStockType mode)
        {
            var file = mode == MyStockType.First ? StockConstHelper.FIRSTPATH :
                mode == MyStockType.ZT ? StockConstHelper.ZTPATH :
                mode == MyStockType.CoreT ? StockConstHelper.CORETPATH :
                mode == MyStockType.LDX ? StockConstHelper.LDXPATH :
                mode == MyStockType.CoreT2 ? StockConstHelper.CORETPATH2 :
                mode == MyStockType.CoreT3 ? StockConstHelper.CORETPATH3 :
                mode == MyStockType.Kernel ? StockConstHelper.KERNELPATH :
                mode == MyStockType.KernelH ? StockConstHelper.KERNELHPATH :
                mode == MyStockType.KernelL ? StockConstHelper.KERNELLPATH : StockConstHelper.JLPATH;
            var list1 = Regex.Split(FileBase.ReadFile(file, StockConstHelper.GB2312), StockConstHelper.RN, RegexOptions.IgnoreCase);
            var ret = new List<StockPrice>();
            foreach (var item in list1)
            {
                var t = item.Split(StockConstHelper.T);
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

        #endregion

        #region 题材数据处理
        /// <summary>
        /// 获取题材数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<StockDes> GetStockDes(StockDesType type)
        {
            var file = StockConstHelper.BAKPATH;
            if (type == StockDesType.DateBase)
            {
                file = StockConstHelper.DATABASEPATH;
            }
            var ret = new List<StockDes>();
            var content = FileBase.ReadFile(file, StockConstHelper.UTF8);
            var list = Regex.Split(content, StockConstHelper.RN, RegexOptions.IgnoreCase);
            foreach (var item in list.Where(p => !string.IsNullOrEmpty(p)))
            {
                var t = item.Split(StockConstHelper.T);
                var des = new StockDes()
                {
                    Code = t[0],
                    Name = t[1]
                };
                var tmp = t[2].Split('+');
                if (tmp.Length == 1)
                {
                    tmp = t[2].Split('-');
                }
                des.Bk = tmp.ToList();
                ret.Add(des);
            }
            return ret;
        }

        /// <summary>
        /// 保存题材
        /// </summary>
        public static void SaveDataBase(List<StockDes> bak)
        {
            var data = GetStockDes(StockDesType.DateBase);
            foreach (var item in data)
            {
                if (!bak.Exists(p => p.Code == item.Code))
                {
                    bak.Add(item);
                }
            }

            FileBase.WriteFile("./src/data", "database.txt", string.Join("\r\n", bak.
                Select(p => p.Code.Convert2Int32(0).ToString("000000") + "\t" + p.Name + "\t" + string.Join("+", p.Bk) + "\t")), "utf-8", FileBaseMode.Create);
        }
        #endregion
    }
}
