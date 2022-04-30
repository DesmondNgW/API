using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using X.Business.Entities.Enums;
using X.Business.Entities.Stock;
using X.Business.Helper.Stock;
using X.Util.Core;
using X.Util.Core.Configuration;
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
            List<StockPrice> Core3, List<StockPrice> LDX, List<StockPrice> DDXList, List<MyStock> AQS, List<MyStock> All)
        {
            var topCount = ConfigurationHelper.GetAppSettingByName("topCount", 15);
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
            FileBase.WriteFile("./", "dest.txt", string.Join(StockConstHelper.TN, list.OrderByDescending(p => p.Value.SLevel)
                .ThenByDescending(p => p.Value.Inc).
                Select(p => p.Value.StockCode + " " + p.Value.StockName + " " + string.Join("+", p.Value.BK) + " " + p.Value.Remark + " " + p.Value.OrderRemark2
                + " " + p.Value.OrderRemark + " " + p.Value.OrderRemark3 + " " + p.Value.LDX + " " + p.Value.KLL + " " + p.Value.NF)), "utf-8", FileBaseMode.Create);
            #endregion
        }
        #endregion

        #region 复盘数据加工
        /// <summary>
        /// GetDDXListFromExcel
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetDDXListFromExcel()
        {
            var ret = new List<StockPrice>();
            var file = StockConstHelper.TOOLPATH;
            FileBase.ReadExcel(file, (sheet) =>
            {
                IRow row = sheet.GetRow(0);
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        var ddx = row.GetCell(5).ToString().Convert2Decimal(-1);
                        if (ddx > 0)
                        {
                            ret.Add(new StockPrice()
                            {
                                StockCode = row.GetCell(1).ToString().Trim(),
                                StockName = row.GetCell(2).ToString(),
                            });
                        }
                    }
                }
            });
            return ret;
        }

        /// <summary>
        /// GetDDXList2FromExcel
        /// </summary>
        /// <returns></returns>
        public static List<StockCompare> GetDDXList2FromExcel()
        {
            var ret = new List<StockCompare>();
            var file = StockConstHelper.TOOLPATH;
            FileBase.ReadExcel(file, (sheet) =>
            {
                IRow row = sheet.GetRow(0);
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row != null)
                    {
                        var ddx = row.GetCell(5).ToString().Convert2Decimal(-1);
                        ret.Add(new StockCompare()
                        {
                            Code = row.GetCell(1).ToString().Trim(),
                            Name = row.GetCell(2).ToString(),
                            DDX = ddx,
                            DDXWeek = row.GetCell(8).ToString().Convert2Decimal(-1),
                            Inc = row.GetCell(4).ToString().Convert2Decimal(-1)
                        });
                    }
                }
            });
            return ret;
        }

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
        /// 资金流文件
        /// </summary>
        /// <returns></returns>
        public static List<StockCompare> GetDDXList2()
        {
            var file = StockConstHelper.TOOLPATH;
            var list1 = Regex.Split(FileBase.ReadFile(file, StockConstHelper.GB2312), StockConstHelper.RN, RegexOptions.IgnoreCase);
            var ret = new List<StockCompare>();
            foreach (var item in list1)
            {
                if (string.IsNullOrEmpty(item)) continue;
                string[] t = StockDealBase.SplitFiledByRegex(item, 21, 2);
                if (t.Length >= 6)
                {
                    var ddx = t[5].Convert2Decimal(-1);
                    ret.Add(new StockCompare()
                    {
                        Code = t[1].Trim(),
                        Name = t[2],
                        DDX = ddx,
                        DDXWeek = t[8].Convert2Decimal(-1),
                        Inc = t[4].Convert2Decimal(-1)
                    });
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

        /// <summary>
        /// 手动输入外置模式
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<string, string[]>> GetFilterListFromFile()
        {
            var file = StockConstHelper.DPBKPATH;
            var content = FileBase.ReadFile(file, StockConstHelper.UTF8);
            var list = Regex.Matches(content, StockConstHelper.REGEXBK);
            var ret = new List<Tuple<string, string[]>>();
            foreach (Match item in list)
            {
                ret.Add(new Tuple<string, string[]>(item.Groups[1].Value, item.Groups[2].Value.Split('-')));
            }
            return ret;
        }

        /// <summary>
        /// 输出模式结果
        /// </summary>
        /// <param name="iret"></param>
        /// <param name="Des"></param>
        public static void GetResultFromMode(IEnumerable<KeyValuePair<string, ModeCompare>> iret, List<StockDes> Des)
        {
            var list = new List<StockCompare>();
            foreach (var item in iret)
            {
                foreach (var cl in item.Value.CodeList)
                {
                    if (!list.Exists(p => p.Code == cl.Code))
                    {
                        list.Add(cl);
                    }
                }
            }
            foreach (var item in list)
            {
                var desItem = Des.FirstOrDefault(p => p.Code == item.Code);
                Logger.Client.Debug(string.Format("{0}({1})：{2};", item.Name, item.Code, desItem != null ? string.Join("+", desItem.Bk) : ""), LogDomain.Business);
            }
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
                Logger.Client.Debug(string.Format("{0}:{1}:{2}", DateTime.Now.ToString("HH:mm:ss.fff"), tmp[(int)item.Key - 1], string.Join("-", item.Value)), LogDomain.Business);
            }
        }

        /// <summary>
        /// 输出模式结果
        /// </summary>
        public static IEnumerable<KeyValuePair<string, ModeCompare>> GetModeCompareWithOrder(Dictionary<string, ModeCompare> mode, string remark, string weekddx, string ddxmode)
        {
            var newMode = new Dictionary<string, ModeCompare>();
            var list = new List<StockCompare>();
            foreach (var item in mode)
            {
                list = list.Concat(item.Value.CodeList).ToList();
            }
            if (ddxmode == "week")
            {
                list = list.OrderByDescending(p => p.DDXWeek).ToList();
            }
            else
            {
                list = list.OrderByDescending(p => p.DDX).ToList();
            }
            if (list.Count == 0) return null;
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
            var sumAmount = list.Sum(p => p.Amount);
            var stdInc = list.Sum(p => p.Inc * p.Amount / sumAmount);
            var stdDDX = list.Average(p => p.DDX * p.Amount / sumAmount);
            var stdDDXWeek = weekddx == StockConstHelper.AUTO ? list.Average(p => p.DDXWeek * p.Amount / sumAmount) : decimal.MinValue;
            var ret = newMode.Count > 1 ? newMode.Where(p => p.Value.DDX >= stdDDX && p.Value.Inc >= stdInc &&
                p.Value.DDXOrder <= stdOrder && p.Value.DDXWeek >= stdDDXWeek) :
                newMode;
            if (ddxmode == "week")
            {
                ret = newMode.Count > 1 ? newMode.Where(p => p.Value.Inc >= stdInc && p.Value.DDXOrder <= stdOrder && p.Value.DDXWeek >= stdDDXWeek) :
                newMode;
            }
            foreach (var item in ret)
            {
                Logger.Client.Debug(string.Format("key:{0},value:{1}", item.Key, string.Join("-", item.Value.CodeList.Select(p => p.Name))), LogDomain.Business);
            }
            return ret;
        }


        /// <summary>
        /// 输出模式结果；个股做key
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="remark"></param>
        /// <param name="weekddx"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, ModeCompare>> GetModeCompareWithOrder(IEnumerable<KeyValuePair<string, ModeCompare>> mode, string remark, string weekddx, string ddxmode)
        {
            var newMode = new Dictionary<string, ModeCompare>();
            var list = new List<StockCompare>();
            foreach (var item in mode)
            {
                list = list.Concat(item.Value.CodeList).ToList();
            }
            if (ddxmode == "week")
            {
                list = list.OrderByDescending(p => p.DDXWeek).ToList();
            }
            else
            {
                list = list.OrderByDescending(p => p.DDX).ToList();
            }
            if (list.Count == 0) return null;
            for (var i = 0; i < list.Count; i++)
            {
                list[i].DDXOrder = i + 1;
                if (!newMode.ContainsKey(list[i].Code))
                {
                    newMode[list[i].Code] = new ModeCompare()
                    {
                        Name = list[i].Code,
                        CodeList = new List<StockCompare>()
                        {
                            list[i]
                        }
                    };
                }
                else
                {
                    newMode[list[i].Code].CodeList.Add(list[i]);
                }
            }
            var stdOrder = 0.5 * list.Count;
            var sumAmount = list.Sum(p => p.Amount);
            var stdInc = list.Sum(p => p.Inc * p.Amount / sumAmount);
            var stdDDX = list.Average(p => p.DDX * p.Amount / sumAmount);
            var stdDDXWeek = weekddx == StockConstHelper.AUTO ? list.Average(p => p.DDXWeek * p.Amount / sumAmount) : decimal.MinValue;
            var ret = newMode.Count > 1 ? newMode.Where(p => p.Value.DDX >= stdDDX && p.Value.Inc >= stdInc &&
                p.Value.DDXOrder <= stdOrder && p.Value.DDXWeek >= stdDDXWeek) :
                newMode;
            if (ddxmode == "week")
            {
                ret = newMode.Count > 1 ? newMode.Where(p => p.Value.Inc >= stdInc && p.Value.DDXOrder <= stdOrder &&
                p.Value.DDXWeek >= stdDDXWeek) :
                newMode;
            }
            foreach (var item in ret)
            {
                Logger.Client.Debug(string.Format("key:{0},value:{1}", item.Key, string.Join("-", item.Value.CodeList.Select(p => p.Name))), LogDomain.Business);
            }
            return ret;
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
                Logger.Client.Debug(string.Format("两市预估成交金额：{0}亿", y.ToString("0.00")), LogDomain.Business);
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
            Logger.Client.Debug(string.Format("上证:{0}%,深圳:{1}%,中小:{2}%,创业:{3}%", a.Inc.ToString("0.00"), b.Inc.ToString("0.00"), c.Inc.ToString("0.00"), d.Inc.ToString("0.00")), LogDomain.Business);
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
        public static void MonitorStock(List<MyStock> AQS, List<MyStock> All, List<MyStock> Jx, List<MyStock> Jx2, List<string> SP, List<StockDes> Des)
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
                    var desItem = Des.FirstOrDefault(p => p.Code == item.StockCode);
                    Logger.Client.Debug(string.Format("{10}-{0}-{1}({2}):涨跌幅{3}%，所属板块：{11}，日成交{4}亿，买一{5}亿，卖一{6}亿，放量比例{7}%，S:{8},股价{9}",
                        DateTime.Now.ToString("HH:mm:ss"), item.StockName, item.StockCode, item.Inc.ToString("0.00"),
                        item.Amount.ToString("0.00"), item.Buy1.ToString("0.00"), item.Sell1.ToString("0.00"),
                        item.AmountRate.ToString("0.00"), item.S.ToString("0"), item.Price.ToString("0.00"), i
                        , string.Join("+", desItem != null ? desItem.Bk : new List<string>() { "未知板块" })), LogDomain.Business);
                    i++;
                }
            }
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
        public static void SaveDataBase()
        {
            var bak = GetStockDes(StockDesType.Bak);
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
