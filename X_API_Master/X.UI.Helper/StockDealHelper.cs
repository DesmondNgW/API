using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Helper
{
    public enum MyStockMode
    {
        Stock,
        Index
    }

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
            return p.Tmp > -100 && !p.Name.Contains("ST");
        }

        /// <summary>
        /// 非ST且趋势强的有效股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool F2(MyStock p)
        {
            return p.Tmp > -100 && !p.Name.Contains("ST") && (p.S1 - p.S3 * 2 > 0 || p.S2 - p.S4 > 0);
        }

        /// <summary>
        /// 非ST，中轨上方的有效股票
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool F3(MyStock p)
        {
            return p.Tmp > -100 && !p.Name.Contains("ST") && p.K4 > 0;
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
            return p.Code + " " + p.Name + " " + (p.K1 + p.K2 + p.K3 + p.K4) / 4;
        }

        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static MyStock SetOrder(MyStock item, int index, int type)
        {
            switch (type)
            {
                case 1:
                    item.Order1 = index;
                    break;
                case 2:
                    item.Order2 = index;
                    break;
                case 3:
                    item.Order3 = index;
                    break;
                case 4:
                    item.Order4 = index;
                    break;
                case 5:
                    item.Order5 = index;
                    break;
                case 6:
                    item.Order6 = index;
                    break;
                case 7:
                    item.Order7 = index;
                    break;
                case 8:
                    item.Order8 = index;
                    break;

            }
            return item;
        }

        /// <summary>
        /// 对集合重新计算排序序号
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<MyStock> SetOrder(IEnumerable<MyStock> list)
        {
            return list.OrderByDescending(p => p.S1).Select((p, index) => SetOrder(p, index, 1))
                .OrderByDescending(p => p.S2).Select((p, index) => SetOrder(p, index, 2))
                .OrderByDescending(p => p.S3).Select((p, index) => SetOrder(p, index, 3))
                .OrderByDescending(p => p.S4).Select((p, index) => SetOrder(p, index, 4))
                .OrderByDescending(p => p.K1).Select((p, index) => SetOrder(p, index, 5))
                .OrderByDescending(p => p.K2).Select((p, index) => SetOrder(p, index, 6))
                .OrderByDescending(p => p.K3).Select((p, index) => SetOrder(p, index, 7))
                .OrderByDescending(p => p.K4).Select((p, index) => SetOrder(p, index, 8));
        }

        /// <summary>
        /// GetAnswer
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetAnswer(List<double> list)
        {
            if (list.Count >= 5)
            {
                int countA = 0;
                for (var i = 2; i < list.Count; i++)
                {
                    if (list[i - 1] >= list[i - 2] && list[i - 1] >= list[i])
                    {
                        countA++;
                    }
                    if (countA == 2 && list[i - 1] <= list[i - 2] && list[i - 1] <= list[i])
                    {
                        return i * 25;
                    }
                }
            }
            return 0;
        }
        #endregion

        #region 主体逻辑
        /// <summary>
        /// 输入文件导入
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<MyStock> GetMyStock(MyStockMode mode)
        {
            var file = mode == MyStockMode.Index ? "./src/板块.txt" : "./src/股票.txt";
            var content = FileBase.ReadFile(file, "gb2312");
            var list = Regex.Split(content, "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<MyStock>();
            foreach (var item in list)
            {
                var t = item.Split('\t');
                if (t.Length >= 15)
                {
                    var myStock = new MyStock()
                    {
                        Code = t[0],
                        Name = t[1],
                        S1 = t[6].Convert2Double(-10000),
                        S2 = t[7].Convert2Double(-10000),
                        S3 = t[8].Convert2Double(-10000),
                        S4 = t[9].Convert2Double(-10000),
                        K1 = t[10].Convert2Double(-100),
                        K2 = t[11].Convert2Double(-100),
                        K3 = t[12].Convert2Double(-100),
                        K4 = t[13].Convert2Double(-100),
                        Tmp = t[2].Convert2Double(-10000),
                    };
                    ret.Add(myStock);
                }
            }
            SetOrder(ret);
            return ret;
        }

        /// <summary>
        /// 前排股票
        /// </summary>
        /// <param name="list"></param>
        /// <param name="top"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string[] GetStockName(List<MyStock> list, int top, Func<MyStock, bool> f, Func<MyStock, string> o)
        {
            var a1 = list.Where(f).OrderByDescending(p => p.S1).Take(top);
            var a2 = list.Where(f).OrderByDescending(p => p.S2).Take(top);
            var a3 = list.Where(f).OrderByDescending(p => p.S3).Take(top);
            var a4 = list.Where(f).OrderByDescending(p => p.S4).Take(top);
            var b1 = a1.Union(a2);
            var b2 = a3.Union(a4);
            return SetOrder(b1.Where(p => b2.Count(q => q.Code == p.Code) > 0).Distinct()).OrderBy(p => p.Order).Select(o).ToArray();
        }
     
        /// <summary>
        /// 风控输出
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dir"></param>
        /// <param name="encode"></param>
        public static void Monitor(List<MyStock> list, string dir, string encode)
        {
            var list1 = list.Where(F1).OrderByDescending(p => p.S1).Take(500);
            var list2 = list.Where(F1).OrderByDescending(p => p.S2).Take(500);
            var list3 = list.Where(F1).OrderByDescending(p => p.S3).Take(500);
            var list4 = list.Where(F1).OrderByDescending(p => p.S4).Take(500);
            FileBase.WriteFile(dir, "S1.txt", string.Join("\t\n", list1.Select(O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "S2.txt", string.Join("\t\n", list2.Select(O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "S3.txt", string.Join("\t\n", list3.Select(O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "S4.txt", string.Join("\t\n", list4.Select(O1)), encode, FileBaseMode.Create);
        }

        /// <summary>
        /// 盘中风控指数监控
        /// </summary>
        public static void MonitorIndex()
        {
            var a = StockDataHelper.GetIndexPrice("sh000001");
            var b = StockDataHelper.GetIndexPrice("sz399001");
            var c = StockDataHelper.GetIndexPrice("sz399005");
            var d = StockDataHelper.GetIndexPrice("sz399006");
            var e = (a.Inc + b.Inc + c.Inc + d.Inc) / 4;
            if (a.Inc > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine("上证:{0}%,深圳:{1}%,中小:{2}%,创业:{3}%,综合:{4}%", a.Inc.ToString("0.00"), b.Inc.ToString("0.00"), c.Inc.ToString("0.00"), d.Inc.ToString("0.00"), e.ToString("0.00"));
        }

        /// <summary>
        /// 处理股票输出
        /// </summary>
        /// <param name="list"></param>
        /// <param name="encode"></param>
        public static void Deal(List<MyStock> list, string encode = "utf-8")
        {
            var dir = "./dest";
            var KContent = new List<double>();
            var TContent = new List<double>();
            for (var i = 25; i <= 400; i += 25)
            {
                //K系列
                var kContent = GetStockName(list, i, F1, O1);
                KContent.Add((kContent.Length + 0.0) / i);
                FileBase.WriteFile(dir, "K" + i + ".txt", string.Join("\t\n", kContent), encode, FileBaseMode.Create);
                //T系列
                var tContent = GetStockName(list, i, F2, O2);
                TContent.Add((tContent.Length + 0.0) / i);
                FileBase.WriteFile(dir, "T" + i + ".txt", string.Join("\t\n", tContent), encode, FileBaseMode.Create);
            }
            FileBase.WriteFile(dir, "K500.txt", string.Join("\t\n", GetStockName(list, 500, F1, O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "K825.txt", string.Join("\t\n", GetStockName(list, 825, F1, O1)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "T500.txt", string.Join("\t\n", GetStockName(list, 500, F2, O2)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "T825.txt", string.Join("\t\n", GetStockName(list, 825, F2, O2)), encode, FileBaseMode.Create);
            Console.WriteLine("KContent:{0}", GetAnswer(KContent));
            Console.WriteLine("TContent:{0}", GetAnswer(TContent));
            FileBase.WriteFile(dir, "K0400.txt", string.Join("\t\n", KContent.Select((p, index) => (index + 1) * 25 + " " + p.ToString("0.000"))), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "T0400.txt", string.Join("\t\n", KContent.Select((p, index) => (index + 1) * 25 + " " + p.ToString("0.000"))), encode, FileBaseMode.Create);
            Monitor(list, dir, encode);
        }

        /// <summary>
        /// 处理板块输出
        /// </summary>
        /// <param name="list"></param>
        /// <param name="encode"></param>
        public static void Deal2(List<MyStock> list, string encode = "utf-8")
        {
            var dir = "./dest";
            for (var i = 3; i <= 48; i += 3)
            {
                FileBase.WriteFile(dir, "B" + i + ".txt", string.Join("\t\n", GetStockName(list, i, F3, O2)), encode, FileBaseMode.Create);
            }
        }
        #endregion

        public static void Program()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            var t1 = GetMyStock(MyStockMode.Stock);
            Deal(t1);
            var t2 = GetMyStock(MyStockMode.Index);
            Deal2(t2);

            var dt = DateTime.Now;
            while (dt.TimeOfDay >= new TimeSpan(9, 30, 0) && dt.TimeOfDay <= new TimeSpan(15, 0, 0))
            {
                MonitorIndex();
                Thread.Sleep(6000);
                dt = DateTime.Now;
            }
            Console.WriteLine("Program End! Press Any Key!");
            Console.ReadKey();
        }
    }
}
