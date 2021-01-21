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

        private static bool F5(MyStock p, List<MyStock> list)
        {
            return p.Inc > -100 && !p.Name.Contains("ST") && list.Exists(q => q.Code == p.Code);
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
                 mode == MyStockMode.IndexWave ? "./src/fp/BKWave.txt" :
                 mode == MyStockMode.MS ? "./src/fp/MS.txt" :
                 mode == MyStockMode.AR ? "./src/fp/AR.txt" :
                 mode == MyStockMode.HHS ? "./src/fp/HHS.txt" :
                 mode == MyStockMode.HS ? "./src/fp/HS.txt" :
                 mode == MyStockMode.THS ? "./src/fp/2HS.txt" :
                 mode == MyStockMode.DS ? "./src/fp/DS.txt" :
                 mode == MyStockMode.TDS ? "./src/fp/3DS.txt" :
                 mode == MyStockMode.WS ? "./src/fp/WS.txt" :
                 mode == MyStockMode.TWS ? "./src/fp/TWS.txt" :
                 mode == MyStockMode.HB ? "./src/fp/HB.txt" :
                 mode == MyStockMode.THB ? "./src/fp/THB.txt" :
                 mode == MyStockMode.DB ? "./src/fp/DB.txt" :
                 mode == MyStockMode.TDB ? "./src/fp/TDB.txt" :
                 mode == MyStockMode.WB ? "./src/fp/WB.txt" :
                 mode == MyStockMode.TWB ? "./src/fp/TWB.txt" :
                 mode == MyStockMode.MB ? "./src/fp/MB.txt" :
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
        public static void Deal(List<MyStock> AQS, List<MyStock> Wave, List<MyStock> AR, List<MyStock> HHS, 
            List<MyStock> HS, List<MyStock> THS, List<MyStock> DS, List<MyStock> TDS, List<MyStock> WS, List<MyStock> TWS, 
            List<MyStock> HB, List<MyStock> THB, List<MyStock> DB, List<MyStock> TDB, List<MyStock> WB, List<MyStock> TWB,
            List<MyStock> MB, List<MyStock> MS, string encode = "utf-8")
        {
            var tmp = Union(AQS, Wave);
            //原始数据处理
            var list = tmp.Where(p => !p.Code.StartsWith("8"));
            var arList = AR.Where(p => !p.Code.StartsWith("8"));
            var hhsList = HHS.Where(p => !p.Code.StartsWith("8"));
            var hsList = HS.Where(p => !p.Code.StartsWith("8"));
            var thsList = THS.Where(p => !p.Code.StartsWith("8"));
            var dsList = DS.Where(p => !p.Code.StartsWith("8"));
            var tdsList = TDS.Where(p => !p.Code.StartsWith("8"));
            var wsList = WS.Where(p => !p.Code.StartsWith("8"));
            var twsList = TWS.Where(p => !p.Code.StartsWith("8"));

            var hbList = HB.Where(p => !p.Code.StartsWith("8"));
            var thbList = THB.Where(p => !p.Code.StartsWith("8"));
            var dbList = DB.Where(p => !p.Code.StartsWith("8"));
            var tdbList = TDB.Where(p => !p.Code.StartsWith("8"));
            var wbList = WB.Where(p => !p.Code.StartsWith("8"));
            var twbList = TWB.Where(p => !p.Code.StartsWith("8"));
            var mbList = MB.Where(p => !p.Code.StartsWith("8"));

            var bk = tmp.Where(p => p.Code.StartsWith("8"));
            var t = list.Sum(p => p.Amount) / 400 * 0.382;
            Console.WriteLine("{0}亿", (t / 1e8).ToString("0.00"));
            string dir = "./dest", dirK = "./dest/K", dirA = "./dest/A", dirBk = "./dest/Bk", dirAr = "./dest/Ar", 
                dirHHs = "./dest/Hhs", dirHs = "./dest/Hs", dirThs = "./dest/Ths", dirDs = "./dest/Ds",
                dirTDs = "./dest/TDs", dirWs = "./dest/Ws", dirTws = "./dest/Tws", dirHb = "./dest/Hb", 
                dirThb = "./dest/Thb", dirDb = "./dest/Db", dirTdb = "./dest/Tdb", dirWb = "./dest/Wb",
                dirTwb = "./dest/Twb", dirMb = "./dest/Mb";
            var KContent = new List<Tuple<double, double>>();
            var AContent = new List<Tuple<double, double>>();
            var BkContent = new List<Tuple<double, double>>();
            var ArContent = new List<Tuple<double, double>>();
            var HhsContent = new List<Tuple<double, double>>();
            var HsContent = new List<Tuple<double, double>>();
            var ThsContent = new List<Tuple<double, double>>();
            var DsContent = new List<Tuple<double, double>>();
            var TDsContent = new List<Tuple<double, double>>();
            var WsContent = new List<Tuple<double, double>>();
            var TwsContent = new List<Tuple<double, double>>();

            var HbContent = new List<Tuple<double, double>>();
            var ThbContent = new List<Tuple<double, double>>();
            var DbContent = new List<Tuple<double, double>>();
            var TDbContent = new List<Tuple<double, double>>();
            var WbContent = new List<Tuple<double, double>>();
            var TwbContent = new List<Tuple<double, double>>();
            var MbContent = new List<Tuple<double, double>>();

            Func<MyStock, bool> _F5 = p => F5(p, Wave);

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
                //Ar系列
                var arContent = GetStockName(arList, i, F1, O2);
                ArContent.Add(new Tuple<double, double>(arContent[0].Convert2Double(-10000), (arContent.Length - 1.0) / i));
                FileBase.WriteFile(dirAr, "Ar" + i + ".txt", string.Join("\t\n", arContent), encode, FileBaseMode.Create);
                //Hhs系列
                var hhsContent = GetStockName(hhsList, i, _F5, O2);
                var _hhsOutput = hhsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                HhsContent.Add(new Tuple<double, double>(hhsContent[0].Convert2Double(-10000), (hhsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirHHs, "Hhs" + i + ".txt", string.Join("\t\n", _hhsOutput), encode, FileBaseMode.Create);
                //HS系列
                var hsContent = GetStockName(hsList, i, _F5, O2);
                var _hsOutput = hsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                HsContent.Add(new Tuple<double, double>(hsContent[0].Convert2Double(-10000), (hsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirHs, "Hs" + i + ".txt", string.Join("\t\n", _hsOutput), encode, FileBaseMode.Create);
                //THS系列
                var thsContent = GetStockName(thsList, i, _F5, O2);
                var _thsOutput = thsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                ThsContent.Add(new Tuple<double, double>(thsContent[0].Convert2Double(-10000), (thsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirThs, "Ths" + i + ".txt", string.Join("\t\n", _thsOutput), encode, FileBaseMode.Create);
                //DS系列
                var dsContent = GetStockName(dsList, i, _F5, O2);
                var _dsOutput = dsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                DsContent.Add(new Tuple<double, double>(dsContent[0].Convert2Double(-10000), (dsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirDs, "Ds" + i + ".txt", string.Join("\t\n", _dsOutput), encode, FileBaseMode.Create);
                //TDS系列
                var tdsContent = GetStockName(tdsList, i, _F5, O2);
                var _tdsOutput = tdsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                TDsContent.Add(new Tuple<double, double>(tdsContent[0].Convert2Double(-10000), (tdsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirTDs, "TDs" + i + ".txt", string.Join("\t\n", _tdsOutput), encode, FileBaseMode.Create);
                //WS系列
                var wsContent = GetStockName(wsList, i, _F5, O2);
                var _wsOutput = wsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                WsContent.Add(new Tuple<double, double>(wsContent[0].Convert2Double(-10000), (wsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirWs, "Ws" + i + ".txt", string.Join("\t\n", _wsOutput), encode, FileBaseMode.Create);
                //TwS系列
                var twsContent = GetStockName(twsList, i, _F5, O2);
                var _twsOutput = twsContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                TwsContent.Add(new Tuple<double, double>(twsContent[0].Convert2Double(-10000), (twsContent.Length - 1.0) / i));
                FileBase.WriteFile(dirTws, "Tws" + i + ".txt", string.Join("\t\n", _twsOutput), encode, FileBaseMode.Create);

                //Hb系列
                var hbContent = GetStockName(hbList, i, _F5, O2);
                var _hbOutput = hbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                HbContent.Add(new Tuple<double, double>(hbContent[0].Convert2Double(-10000), (hbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirHb, "Hb" + i + ".txt", string.Join("\t\n", _hbOutput), encode, FileBaseMode.Create);
                //Thb系列
                var thbContent = GetStockName(thbList, i, _F5, O2);
                var _thbOutput = thbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                ThbContent.Add(new Tuple<double, double>(thbContent[0].Convert2Double(-10000), (thbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirThb, "Thb" + i + ".txt", string.Join("\t\n", _thbOutput), encode, FileBaseMode.Create);
                //Db系列
                var dbContent = GetStockName(dbList, i, _F5, O2);
                var _dbOutput = dbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                DbContent.Add(new Tuple<double, double>(dbContent[0].Convert2Double(-10000), (dbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirDb, "Db" + i + ".txt", string.Join("\t\n", _dbOutput), encode, FileBaseMode.Create);
                //Tdb系列
                var tdbContent = GetStockName(tdbList, i, _F5, O2);
                var _tdbOutput = tdbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                TDbContent.Add(new Tuple<double, double>(tdbContent[0].Convert2Double(-10000), (tdbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirTdb, "Tdb" + i + ".txt", string.Join("\t\n", _tdbOutput), encode, FileBaseMode.Create);
                //Wb系列
                var wbContent = GetStockName(wbList, i, _F5, O2);
                var _wbOutput = wbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                WbContent.Add(new Tuple<double, double>(wbContent[0].Convert2Double(-10000), (wbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirWb, "Wb" + i + ".txt", string.Join("\t\n", _wbOutput), encode, FileBaseMode.Create);

                //Twb系列
                var twbContent = GetStockName(twbList, i, _F5, O2);
                var _twbOutput = twbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                TwbContent.Add(new Tuple<double, double>(twbContent[0].Convert2Double(-10000), (twbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirTwb, "Twb" + i + ".txt", string.Join("\t\n", _twbOutput), encode, FileBaseMode.Create);

                //Mb系列
                var mbContent = GetStockName(mbList, i, _F5, O2);
                var _mbOutput = mbContent.Where(p => MS.Exists(m => p.Contains(m.Code)));
                MbContent.Add(new Tuple<double, double>(mbContent[0].Convert2Double(-10000), (mbContent.Length - 1.0) / i));
                FileBase.WriteFile(dirMb, "Mb" + i + ".txt", string.Join("\t\n", _mbOutput), encode, FileBaseMode.Create);
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
            //Ar系列
            FileBase.WriteFile(dirAr, "Ar500.txt", string.Join("\t\n", GetStockName(arList, 500, F1, O2)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirAr, "Ar825.txt", string.Join("\t\n", GetStockName(arList, 825, F1, O2)), encode, FileBaseMode.Create);
            //HHs系列
            FileBase.WriteFile(dirHHs, "Hhs500.txt", string.Join("\t\n", GetStockName(hhsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirHHs, "Hhs825.txt", string.Join("\t\n", GetStockName(hhsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Hs系列
            FileBase.WriteFile(dirHs, "Hs500.txt", string.Join("\t\n", GetStockName(hsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirHs, "Hs825.txt", string.Join("\t\n", GetStockName(hsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Ths系列
            FileBase.WriteFile(dirThs, "Ths500.txt", string.Join("\t\n", GetStockName(thsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirThs, "Ths825.txt", string.Join("\t\n", GetStockName(thsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Ds系列
            FileBase.WriteFile(dirDs, "Ds500.txt", string.Join("\t\n", GetStockName(dsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirDs, "Ds825.txt", string.Join("\t\n", GetStockName(dsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //TDs系列
            FileBase.WriteFile(dirTDs, "TDs500.txt", string.Join("\t\n", GetStockName(tdsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirTDs, "TDs825.txt", string.Join("\t\n", GetStockName(tdsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Ws系列
            FileBase.WriteFile(dirWs, "Ws500.txt", string.Join("\t\n", GetStockName(wsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirWs, "Ws825.txt", string.Join("\t\n", GetStockName(wsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //TWs系列
            FileBase.WriteFile(dirTws, "Tws500.txt", string.Join("\t\n", GetStockName(twsList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirTws, "Tws825.txt", string.Join("\t\n", GetStockName(twsList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Hb系列
            FileBase.WriteFile(dirHb, "Hb500.txt", string.Join("\t\n", GetStockName(hbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirHb, "Hb825.txt", string.Join("\t\n", GetStockName(hbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Thb系列
            FileBase.WriteFile(dirThb, "Thb500.txt", string.Join("\t\n", GetStockName(thbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirThb, "Thb825.txt", string.Join("\t\n", GetStockName(thbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Db系列
            FileBase.WriteFile(dirDb, "Db500.txt", string.Join("\t\n", GetStockName(dbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirDb, "Db825.txt", string.Join("\t\n", GetStockName(dbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Tdb系列
            FileBase.WriteFile(dirTdb, "Tdb500.txt", string.Join("\t\n", GetStockName(tdbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirTdb, "Tdb825.txt", string.Join("\t\n", GetStockName(tdbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Wb系列
            FileBase.WriteFile(dirWb, "Wb500.txt", string.Join("\t\n", GetStockName(wbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirWb, "Wb825.txt", string.Join("\t\n", GetStockName(wbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Twb系列
            FileBase.WriteFile(dirTwb, "Twb500.txt", string.Join("\t\n", GetStockName(twbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirTwb, "Twb825.txt", string.Join("\t\n", GetStockName(twbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            //Mb系列
            FileBase.WriteFile(dirMb, "Mb500.txt", string.Join("\t\n", GetStockName(mbList, 500, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dirMb, "Mb825.txt", string.Join("\t\n", GetStockName(mbList, 825, _F5, O2).Where(p => MS.Exists(m => p.Contains(m.Code)))), encode, FileBaseMode.Create);

            var KConsole = GetAnswer(KContent);
            var AConsole = GetAnswer(AContent);
            var ArConsole = GetAnswer(ArContent);
            var HhsConsole = GetAnswer(HhsContent);
            var HsConsole = GetAnswer(HsContent);
            var ThsConsole = GetAnswer(ThsContent);
            var DsConsole = GetAnswer(DsContent);
            var TDsConsole = GetAnswer(TDsContent);
            var WsConsole = GetAnswer(WsContent);
            var TwsConsole = GetAnswer(TwsContent);

            var HbConsole = GetAnswer(HbContent);
            var ThbConsole = GetAnswer(ThbContent);
            var DbConsole = GetAnswer(DbContent);
            var TDbConsole = GetAnswer(TDbContent);
            var WbConsole = GetAnswer(WbContent);
            var TwbConsole = GetAnswer(TwbContent);
            var MbConsole = GetAnswer(MbContent);

            Console.WriteLine("KContent:价格高低点:{0};比例高低点:{1}", string.Join("-", KConsole.Item1), string.Join("-", KConsole.Item2));
            Console.WriteLine("AContent:价格高低点:{0};比例高低点:{1}", string.Join("-", AConsole.Item1), string.Join("-", AConsole.Item2));
            Console.WriteLine("ArContent:价格高低点:{0};比例高低点:{1}", string.Join("-", ArConsole.Item1), string.Join("-", ArConsole.Item2));
            Console.WriteLine("HHsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", HhsConsole.Item1), string.Join("-", HhsConsole.Item2));
            Console.WriteLine("HsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", HsConsole.Item1), string.Join("-", HsConsole.Item2));
            Console.WriteLine("THsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", ThsConsole.Item1), string.Join("-", ThsConsole.Item2));
            Console.WriteLine("DsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", DsConsole.Item1), string.Join("-", DsConsole.Item2));
            Console.WriteLine("TDsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", TDsConsole.Item1), string.Join("-", TDsConsole.Item2));
            Console.WriteLine("WsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", WsConsole.Item1), string.Join("-", WsConsole.Item2));
            Console.WriteLine("TwsContent:价格高低点:{0};比例高低点:{1}", string.Join("-", TwsConsole.Item1), string.Join("-", TwsConsole.Item2));

            Console.WriteLine("HbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", HbConsole.Item1), string.Join("-", HbConsole.Item2));
            Console.WriteLine("ThbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", ThbConsole.Item1), string.Join("-", ThbConsole.Item2));
            Console.WriteLine("DbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", DbConsole.Item1), string.Join("-", DbConsole.Item2));
            Console.WriteLine("TdbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", TDbConsole.Item1), string.Join("-", TDbConsole.Item2));
            Console.WriteLine("WbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", WbConsole.Item1), string.Join("-", WbConsole.Item2));
            Console.WriteLine("TwbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", TwbConsole.Item1), string.Join("-", TwbConsole.Item2));
            Console.WriteLine("MbContent:价格高低点:{0};比例高低点:{1}", string.Join("-", MbConsole.Item1), string.Join("-", MbConsole.Item2));

            FileBase.WriteFile(dir, "K.txt", string.Join("\t\n", KContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "A.txt", string.Join("\t\n", AContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Ar.txt", string.Join("\t\n", ArContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Hhs.txt", string.Join("\t\n", HhsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Hs.txt", string.Join("\t\n", HsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Ths.txt", string.Join("\t\n", ThsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Ds.txt", string.Join("\t\n", DsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "TDs.txt", string.Join("\t\n", TDsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Ws.txt", string.Join("\t\n", WsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Tws.txt", string.Join("\t\n", TwsContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);

            FileBase.WriteFile(dir, "Hb.txt", string.Join("\t\n", HbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Thb.txt", string.Join("\t\n", ThbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Db.txt", string.Join("\t\n", DbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Tdb.txt", string.Join("\t\n", TDbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Wb.txt", string.Join("\t\n", WbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Twb.txt", string.Join("\t\n", TwbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "Mb.txt", string.Join("\t\n", MbContent.Select((p, index) => (index + 1) * 25 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);

            FileBase.WriteFile(dir, "Bk.txt", string.Join("\t\n", BkContent.Select((p, index) => (index + 1) * 3 + " " + p.Item1.ToString("0.000") + " " + p.Item2.ToString("0.000"))), encode, FileBaseMode.Create);
        }

        /// <summary>
        /// 处理板块输出
        /// </summary>
        /// <param name="list"></param>
        /// <param name="list2"></param>
        /// <param name="encode"></param>
        public static void Deal2(List<MyStock> list, List<MyStock> list2, string encode = "utf-8")
        {
            var dirB = "./dest/B";
            var dirBW = "./dest/BW";
            for (var i = 3; i <= 48; i += 3)
            {
                var bContent = GetStockName(list, i, F3, O2);
                FileBase.WriteFile(dirB, "B" + i + ".txt", string.Join("\t\n", bContent), encode, FileBaseMode.Create);
                var bwContent = GetStockName(list2, i, F3, O2);
                FileBase.WriteFile(dirBW, "BW" + i + ".txt", string.Join("\t\n", bwContent), encode, FileBaseMode.Create);
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
        /// 自选板块数据
        /// </summary>
        /// <returns></returns>
        public static List<StockPrice> GetMyMonitorStock(MyStockType mode)
        {
            var file = mode == MyStockType.Continie ? "./src/dp/接力.txt" :
                mode == MyStockType.ShortContinie ? "./src/dp/短线接力.txt" :
                mode == MyStockType.First ? "./src/dp/首板.txt" :
                mode == MyStockType.ZT ? "./src/dp/涨停.txt" :
                mode == MyStockType.HHS ? "./src/dp/CHHS.txt" :
                mode == MyStockType.HS ? "./src/dp/CHS.txt" :
                mode == MyStockType.THS ? "./src/dp/CTHS.txt" :
                mode == MyStockType.DS ? "./src/dp/CDS.txt" :
                mode == MyStockType.TDS ? "./src/dp/CTDS.txt" :
                mode == MyStockType.WS ? "./src/dp/CWS.txt" :
                mode == MyStockType.TWS ? "./src/dp/CTWS.txt" :
                mode == MyStockType.HB ? "./src/dp/CHB.txt" :
                mode == MyStockType.THB ? "./src/dp/CTHB.txt" :
                mode == MyStockType.DB ? "./src/dp/CDB.txt" :
                mode == MyStockType.TDB ? "./src/dp/CTDB.txt" :
                mode == MyStockType.WB ? "./src/dp/CWB.txt" :
                mode == MyStockType.TWB ? "./src/dp/CTWB.txt" :
                mode == MyStockType.MB ? "./src/dp/CMB.txt" :
                mode == MyStockType.SHEN ? "./src/dp/神.txt" :
                mode == MyStockType.WV1 ? "./src/dp/WV1.txt" :
                mode == MyStockType.WV2 ? "./src/dp/WV2.txt" :
                mode == MyStockType.WV3 ? "./src/dp/WV3.txt" :
                mode == MyStockType.WV4 ? "./src/dp/WV4.txt" :
                 mode == MyStockType.AR ? "./src/dp/AR.txt" : "./src/dp/接力.txt";
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

        public static List<StockPrice> GetRealAR(List<StockPrice> AR, List<MyStock> HHS, List<MyStock> HS, List<MyStock> THS,
            List<MyStock> DS, List<MyStock> TDS, List<MyStock> WS, List<MyStock> TWS, List<MyStock> HB, List<MyStock> THB,
            List<MyStock> DB, List<MyStock> TDB, List<MyStock> WB, List<MyStock> TWB, List<MyStock> MB)
        {
            var ret = new List<StockPrice>();
            foreach (var item in AR)
            {
                if (HHS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "HHS";
                }
                else if (HS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "HS";
                }
                else if (THS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "THS";
                }
                else if (DS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "DS";
                }
                else if (TDS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "TDS";
                }
                else if (WS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "WS";
                }
                else if (TWS.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark = "TWS";
                }
                else
                {
                    item.Remark = string.Empty;
                }

                if (HB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "HB";
                }
                else if (THB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "THB";
                }
                else if (DB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "DB";
                }
                else if (TDB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "TDB";
                }
                else if (WB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "WB";
                }
                else if (TWB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "TWB";
                }
                else if (MB.Exists(p => p.Code == item.StockCode))
                {
                    item.Remark2 = "MB";
                }
                else
                {
                    item.Remark2 = string.Empty;
                }
                if (!string.IsNullOrEmpty(item.Remark2) || !string.IsNullOrEmpty(item.Remark))
                {
                    ret.Add(item);
                }
            }
            return ret;
        }

        /// <summary>
        /// 盯盘
        /// </summary>
        /// <param name="Continue"></param>
        /// <param name="ShortContinue"></param>
        /// <param name="AR"></param>
        /// <param name="Shen"></param>
        /// <param name="HHS"></param>
        /// <param name="HS"></param>
        /// <param name="THS"></param>
        /// <param name="DS"></param>
        /// <param name="TDS"></param>
        /// <param name="HB"></param>
        /// <param name="THB"></param>
        /// <param name="DB"></param>
        /// <param name="TDB"></param>
        /// <param name="WB"></param>
        /// <param name="OP"></param>
        /// <param name="First"></param>
        /// <param name="ZT"></param>
        /// <param name="AQS"></param>
        /// <param name="All"></param>
        /// <param name="debug"></param>
        public static void MonitorStock(List<StockPrice> Continue, List<StockPrice> ShortContinue, 
            List<StockPrice> AR, List<StockPrice> Shen, List<StockPrice> HHS, List<StockPrice> HS, List<StockPrice> THS, 
            List<StockPrice> DS, List<StockPrice> TDS, List<StockPrice> WS, List<StockPrice> TWS, List<StockPrice> HB, 
            List<StockPrice> THB, List<StockPrice> DB, List<StockPrice> TDB, List<StockPrice> WB, List<StockPrice> TWB,
            List<StockPrice> MB, List<StockPrice> First, List<StockPrice> ZT, List<StockPrice> WV1, List<StockPrice> WV2,
            List<StockPrice> WV3, List<StockPrice> WV4, List<MyStock> AQS, List<StockPrice> RealAR, List<MyStock> All, bool debug = false)
        {
            //开盘时间
            var tradeStart = ConfigurationHelper.GetAppSettingByName("TradeStart", new DateTime(2099, 1, 1, 9, 15, 0));
            //收盘时间
            var tradeEnd = ConfigurationHelper.GetAppSettingByName("TradeEnd", new DateTime(2099, 1, 1, 15, 0, 0)); 
            //盯盘模式，
            var dpmode = ConfigurationHelper.GetAppSettingByName("DpMode", 1);
            //盯盘过滤器
            var dpFilter = ConfigurationHelper.GetAppSettingByName("filter", 3);
            //加速模式
            var spmode = ConfigurationHelper.GetAppSettingByName("SpMode", 0);

            List<StockPrice> OP = HHS.Union(HS).Union(THS).Union(DS).Union(TDS).Union(WS).Union(TWS)
                .Union(HB).Union(THB).Union(DB).Union(TDB).Union(WB).Union(TWB).Union(MB).Union(RealAR).ToList();

            List<StockPrice> Top = ZT.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();

            #region 盯盘模式
            List<StockPrice> _Continue = null;
            List<StockPrice> _Trend = null;
            if (dpmode == 1)
            {
                _Continue = Continue;
                _Trend = Continue.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 2)
            {
                _Continue = ShortContinue;
                _Trend = ShortContinue.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 3)
            {
                _Continue = HHS;
                _Trend = HHS.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 4)
            {
                _Continue = HS;
                _Trend = HS.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 5)
            {
                _Continue = THS;
                _Trend = THS.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 6)
            {
                _Continue = DS;
                _Trend = DS.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 7)
            {
                _Continue = TDS;
                _Trend = TDS.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
            }
            else if (dpmode == 8)
            {
                _Continue = OP;
                _Trend = OP.Where(p => AQS.Exists(q => q.Code == p.StockCode)).ToList();
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
            //首板过滤
            bool first(StockPrice p) => First.Exists(q => q.StockCode == p.StockCode);
            //半路过滤
            bool zt(StockPrice p) => ZT.All(q => q.StockCode != p.StockCode);
            //连板过滤
            bool lb(StockPrice p) => ZT.Exists(q => q.StockCode == p.StockCode) && First.All(q => q.StockCode != p.StockCode);

            if (dpFilter == 1)
            {
                filter = top;
            }
            else if (dpFilter == 2)
            {
                filter = trend;
            }
            else if (dpFilter == 3)//连板
            {
                filter = lb;
                _top = new List<StockPrice>();
            }
            else if (dpFilter == 4)//首板
            {
                filter = first;
                _top = new List<StockPrice>();
            }
            else if (dpFilter == 5)//半路
            {
                filter = zt;
                _top = new List<StockPrice>();
            }
            else if (dpFilter == 6)//首板或半路
            {
                filter = p => first(p) || zt(p);
                _top = new List<StockPrice>();
            }
            else if (dpFilter == 7)//无龙头
            {
                _top = new List<StockPrice>();
            }
            #endregion

            #region 建模提取数据
            var m1 = new List<MyStockMonitor>();
            var _mainLoop = _Continue.Union(_top).Where(p => p.CurrentPrice > 0 && filter(p));
            foreach (var item in _mainLoop)
            {
                var t = StockDataHelper.GetStockPrice(item.StockCode);
                if (t == null) continue;
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
                    var last = All.FirstOrDefault(p => p.Code == item.StockCode);
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
            //是否大成交模式
            var isBig = ConfigurationHelper.GetAppSettingByName("isBig", false);
            //大成交金额阈值
            var bigAmount = isBig ? ConfigurationHelper.GetAppSettingByName("bigAmount", 20M) : 0;
            bool bigFilter(MyStockMonitor p) => p.Amount >= bigAmount;
            #endregion

            #region 输出
            Console.WriteLine("上涨7%个股{0}-下跌7%个数{1}", m1.Count(p => p.Inc >= 7), m1.Count(p => p.Inc <= -7));
            Console.WriteLine("上涨5%个股{0}-下跌5%个数{1}", m1.Count(p => p.Inc >= 5), m1.Count(p => p.Inc <= -5));
            Console.WriteLine("上涨3%个股{0}-下跌3%个数{1}", m1.Count(p => p.Inc >= 3), m1.Count(p => p.Inc <= -3));
            Console.WriteLine("上涨2%个股{0}-下跌2%个数{1}", m1.Count(p => p.Inc >= 2), m1.Count(p => p.Inc <= -2));

            IEnumerable<MyStockMonitor> m2 = null;
            var dt = DateTime.Now;
            if (debug)
            {
                m2 = m1.OrderByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 1 || (spmode == 0 && dt.TimeOfDay <= tradeEnd.AddMinutes(-15).TimeOfDay && dt.TimeOfDay >= tradeStart.TimeOfDay))
            {
                m2 = m1.Where(p => p.KLevel >= 7 && bigFilter(p)).OrderByDescending(p => p.KLevel).ThenByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 2 || (spmode == 0 && dt.TimeOfDay > tradeEnd.AddMinutes(-15).TimeOfDay && dt.TimeOfDay <= tradeEnd.AddMinutes(105).TimeOfDay))
            {
                m2 = m1.Where(p => p.LLevel >= 4 && bigFilter(p)).OrderByDescending(p => p.LLevel).ThenByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if(spmode == 3)
            {
                m2 = m1.Where(p => p.KLevel >= 7 && bigFilter(p)).OrderByDescending(p => p.SLevel).ThenByDescending(p => p.KLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 4)
            {
                m2 = m1.Where(p => p.LLevel >= 4 && bigFilter(p)).OrderByDescending(p => p.SLevel).ThenByDescending(p => p.LLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 5)
            {
                m2 = m1.Where(p => p.KLevel >= 7 && bigFilter(p)).OrderByDescending(p => p.S).ThenByDescending(p => p.KLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 6)
            {
                m2 = m1.Where(p => p.LLevel >= 4 && bigFilter(p)).OrderByDescending(p => p.S).ThenByDescending(p => p.LLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 7)
            {
                m2 = m1.OrderByDescending(p => p.KLevel).ThenByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else if (spmode == 8)
            {
                m2 = m1.OrderByDescending(p => p.LLevel).ThenByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            else
            {
                m2 = m1.OrderByDescending(p => p.SLevel).ThenByDescending(p => p.Inc).Take(topCount);
            }
            if (m2 != null && m2.Count() > 0)
            {
                var index = 1;
                foreach (var t in m2)
                {
                    //龙头
                    var __top = Top.Exists(p => p.StockCode == t.StockCode);
                    //趋势
                    var __trend = _Trend.Exists(p => p.StockCode == t.StockCode);
                    //分时指标
                    var __ar = AR.Exists(p => p.StockCode == t.StockCode);
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
                        Console.BackgroundColor = __ar ? ConsoleColor.Gray : ConsoleColor.White;// : ConsoleColor.Cyan;
                        tip = __top ? "龙头强势股" : "强势股";
                    }
                    else if (__trend)
                    {
                        Console.BackgroundColor = __ar ? ConsoleColor.Yellow : ConsoleColor.White;
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
                    if (__ar)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        tip1 += "分时达标";
                    }
                    
                    Console.WriteLine("{0}-{1}:{2}({3})涨幅;{4}%,价格;{5},金额比例;{11}%,量能比例;{12}%,K;{6},KLevel;{7},S;{8},SLevel;{9},成交金额:{10}亿,{13}",
                        DateTime.Now.ToString("MM-dd HH:mm:ss<"+ index + ">"), tip,
                        t.StockName, t.StockCode, t.Inc.ToString("0.00"), t.Price, t.K.ToString("0.00"), t.KLevel,
                        t.S.ToString("0.00"), t.SLevel, t.Amount.ToString("0.00"),
                        t.AmountRate.ToString("0.00"), t.VolRate.ToString("0.00"), tip1);
                    index++;
                }
            }
            #endregion

            #region Debug模式
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
                    if (last != null && !listDebug.Exists(p => p.StockCode == item.StockCode))
                    {
                        listDebug.Add(new MyStockMonitor()
                        {
                            MyStockType = item.MyStockType,
                            StockCode = item.StockCode,
                            StockName = item.StockName,
                            Inc = item.Inc,
                            Price = item.CurrentPrice,
                            S = last.S1,
                            Amount = item.Amount,
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
                            list[item.StockCode].Remark += remark + (j+1);

                            var realArItem = RealAR.FirstOrDefault(p => p.StockCode == item.StockCode);

                            if (TWS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "TWS";
                            }
                            else if (WS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "WS";
                            }
                            else if (TDS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "TDS";
                            }
                            else if (DS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "DS";
                            }
                            else if (THS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "THS";
                            }
                            else if (HS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "HS";
                            }
                            else if (HHS.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark = "HHS";
                            }
                            else if (realArItem != null)
                            {
                                list[item.StockCode].OrderRemark = realArItem.Remark;
                            }
                            else
                            {
                                list[item.StockCode].OrderRemark = "";
                            }
                            
                            if (MB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "MB";
                            }
                            else if (TWB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "TWB";
                            }
                            else if (WB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "WB";
                            }
                            else if (TDB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "TDB";
                            }
                            else if (DB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "DB";
                            }
                            else if (THB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "THB";
                            }
                            else if (HB.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark2 = "HB";
                            }
                            else if (realArItem != null)
                            {
                                list[item.StockCode].OrderRemark2 = realArItem.Remark2;
                            }
                            else
                            {
                                list[item.StockCode].OrderRemark2 = "";
                            }
                            
                            if (Shen.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark3 = "Y";
                            }
                            else
                            {
                                list[item.StockCode].OrderRemark3 = "N";
                            }

                            if (WV1.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark4 = "TH";
                            }
                            else if (WV2.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark4 = "D";
                            }
                            else if (WV3.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark4 = "TD";
                            }
                            else if (WV4.Exists(p => p.StockCode == item.StockCode))
                            {
                                list[item.StockCode].OrderRemark4 = "W";
                            }
                            else
                            {
                                list[item.StockCode].OrderRemark4 = "";
                            }
                        }
                    }
                    j++; 
                }
            }
            FileBase.WriteFile("./", "dest.txt", string.Join("\t\n", list.OrderByDescending(p => p.Value.SLevel)
                .ThenByDescending(p => p.Value.Inc).
                Select(p => p.Value.StockCode + " " + p.Value.StockName + " " + p.Value.Remark + " "
                + p.Value.OrderRemark + " " + p.Value.OrderRemark2 + " " + p.Value.OrderRemark3+ " " + p.Value.OrderRemark4)), "utf-8", FileBaseMode.Create);
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
                //接力
                var Continue = GetMyMonitorStock(MyStockType.Continie);
                //短线接力
                var shortContinue = GetMyMonitorStock(MyStockType.ShortContinie);
                //短线分时-All
                var ar = GetMyMonitorStock(MyStockType.AR);
                //AR精选
                var shen = GetMyMonitorStock(MyStockType.SHEN);
                //30卖点
                var hhs = GetMyMonitorStock(MyStockType.HHS);
                //60卖点
                var hs = GetMyMonitorStock(MyStockType.HS);
                //120卖点
                var ths = GetMyMonitorStock(MyStockType.THS);
                //日卖点
                var ds = GetMyMonitorStock(MyStockType.DS);
                //3日卖点
                var tds = GetMyMonitorStock(MyStockType.TDS);
                //周卖
                var ws = GetMyMonitorStock(MyStockType.WS);
                //2周卖
                var tws = GetMyMonitorStock(MyStockType.TWS);

                //60买点
                var hb = GetMyMonitorStock(MyStockType.HB);
                //120买点
                var thb = GetMyMonitorStock(MyStockType.THB);
                //日买点
                var db = GetMyMonitorStock(MyStockType.DB);
                //3日买点
                var tdb = GetMyMonitorStock(MyStockType.TDB);
                //周买点
                var wb = GetMyMonitorStock(MyStockType.WB);
                //2周买点
                var twb = GetMyMonitorStock(MyStockType.TWB);
                //月买点
                var mb = GetMyMonitorStock(MyStockType.MB);

                //首板
                var first = GetMyMonitorStock(MyStockType.First);
                //涨停
                var zt = GetMyMonitorStock(MyStockType.ZT);

                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                var AR = GetMyStock(MyStockMode.AR);
                var HHS = GetMyStock(MyStockMode.HHS);
                var HS = GetMyStock(MyStockMode.HS);
                var THS = GetMyStock(MyStockMode.THS);
                var DS = GetMyStock(MyStockMode.DS);
                var TDS = GetMyStock(MyStockMode.TDS);
                var WS = GetMyStock(MyStockMode.WS);
                var TWS = GetMyStock(MyStockMode.TWS);

                var HB = GetMyStock(MyStockMode.HB);
                var THB = GetMyStock(MyStockMode.THB);
                var DB = GetMyStock(MyStockMode.DB);
                var TDB = GetMyStock(MyStockMode.TDB);
                var WB = GetMyStock(MyStockMode.WB);
                var TWB = GetMyStock(MyStockMode.TWB);
                var MB = GetMyStock(MyStockMode.MB);

                var wv1 = GetMyMonitorStock(MyStockType.WV1);
                var wv2 = GetMyMonitorStock(MyStockType.WV2);
                var wv3 = GetMyMonitorStock(MyStockType.WV3);
                var wv4 = GetMyMonitorStock(MyStockType.WV4);

                var realAR = GetRealAR(ar, HHS, HS, THS, DS, TDS, WS, TWS, HB, THB, DB, TDB, WB, TWB, MB);

                var all = Union(AQS, Wave, AR, HHS, HS, THS, DS, TDS, WS, TWS, HB, THB, DB, TDB, WB, TWB, MB);
                while (dt.TimeOfDay >= tradeStart.TimeOfDay && dt.TimeOfDay <= tradeEnd.TimeOfDay)
                {
                    MonitorIndex();
                    MonitorStock(Continue, shortContinue, ar, shen, hhs, hs, ths, ds, tds, ws, tws,
                        hb, thb, db, tdb, wb, twb, mb, first, zt, wv1, wv2, wv3, wv4, AQS, realAR, all);
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
                var AR = GetMyStock(MyStockMode.AR);
                var HHS = GetMyStock(MyStockMode.HHS);
                var HS = GetMyStock(MyStockMode.HS);
                var THS = GetMyStock(MyStockMode.THS);
                var DS = GetMyStock(MyStockMode.DS);
                var TDS = GetMyStock(MyStockMode.TDS);
                var WS = GetMyStock(MyStockMode.WS);
                var TWS = GetMyStock(MyStockMode.TWS);
                var HB = GetMyStock(MyStockMode.HB);
                var THB = GetMyStock(MyStockMode.THB);
                var DB = GetMyStock(MyStockMode.DB);
                var TDB = GetMyStock(MyStockMode.TDB);
                var WB = GetMyStock(MyStockMode.WB);
                var TWB = GetMyStock(MyStockMode.TWB);
                var MB = GetMyStock(MyStockMode.MB);
                var MS = GetMyStock(MyStockMode.MS);
                Deal(AQS, Wave, AR, HHS, HS, THS, DS, TDS, WS, TWS, HB, THB, DB, TDB, WB, TWB, MB, MS);
                var t2 = GetMyStock(MyStockMode.Index);
                var t3 = GetMyStock(MyStockMode.IndexWave);
                Deal2(t2, t3);
            }
            else if (mode == 3)
            {
                //接力
                var Continue = GetMyMonitorStock(MyStockType.Continie);
                //短线接力
                var shortContinue = GetMyMonitorStock(MyStockType.ShortContinie);
                //短线分时-All
                var ar = GetMyMonitorStock(MyStockType.AR);
                //AR精选
                var shen = GetMyMonitorStock(MyStockType.SHEN);
                //30卖点
                var hhs = GetMyMonitorStock(MyStockType.HHS);
                //60卖点
                var hs = GetMyMonitorStock(MyStockType.HS);
                //120卖点
                var ths = GetMyMonitorStock(MyStockType.THS);
                //日卖点
                var ds = GetMyMonitorStock(MyStockType.DS);
                //3日卖点
                var tds = GetMyMonitorStock(MyStockType.TDS);
                //周卖
                var ws = GetMyMonitorStock(MyStockType.WS);
                //2周卖
                var tws = GetMyMonitorStock(MyStockType.TWS);

                //60买点
                var hb = GetMyMonitorStock(MyStockType.HB);
                //120买点
                var thb = GetMyMonitorStock(MyStockType.THB);
                //日买点
                var db = GetMyMonitorStock(MyStockType.DB);
                //3日买点
                var tdb = GetMyMonitorStock(MyStockType.TDB);
                //周买点
                var wb = GetMyMonitorStock(MyStockType.WB);
                //2周买点
                var twb = GetMyMonitorStock(MyStockType.TWB);
                //月买点
                var mb = GetMyMonitorStock(MyStockType.MB);

                //首板
                var first = GetMyMonitorStock(MyStockType.First);
                //涨停
                var zt = GetMyMonitorStock(MyStockType.ZT);
                var AQS = GetMyStock(MyStockMode.AQS);
                var Wave = GetMyStock(MyStockMode.Wave);
                var AR = GetMyStock(MyStockMode.AR);

                var HHS = GetMyStock(MyStockMode.HHS);
                var HS = GetMyStock(MyStockMode.HS);
                var THS = GetMyStock(MyStockMode.THS);
                var DS = GetMyStock(MyStockMode.DS);
                var TDS = GetMyStock(MyStockMode.TDS);
                var WS = GetMyStock(MyStockMode.WS);
                var TWS = GetMyStock(MyStockMode.TWS);

                var HB = GetMyStock(MyStockMode.HB);
                var THB = GetMyStock(MyStockMode.THB);
                var DB = GetMyStock(MyStockMode.DB);
                var TDB = GetMyStock(MyStockMode.TDB);
                var WB = GetMyStock(MyStockMode.WB);
                var TWB = GetMyStock(MyStockMode.TWB);
                var MB = GetMyStock(MyStockMode.MB);

                var realAR = GetRealAR(ar, HHS, HS, THS, DS, TDS, WS, TWS, HB, THB, DB, TDB, WB, TWB, MB);

                var wv1 = GetMyMonitorStock(MyStockType.WV1);
                var wv2 = GetMyMonitorStock(MyStockType.WV2);
                var wv3 = GetMyMonitorStock(MyStockType.WV3);
                var wv4 = GetMyMonitorStock(MyStockType.WV4);

                var all = Union(AQS, Wave, AR, HHS, HS, THS, DS, TDS, WS, TWS, HB, THB, DB, TDB, WB, TWB, MB);
                MonitorIndex();
                MonitorStock(Continue, shortContinue, ar, shen, hhs, hs, ths, ds, tds, ws, tws,
                    hb, thb, db, tdb, wb, twb, mb, first, zt, wv1, wv2, wv3, wv4, AQS, realAR, all, true);
            }
            Console.WriteLine("Program End! Press Any Key!");
            Console.ReadKey();
        }
    }
}
