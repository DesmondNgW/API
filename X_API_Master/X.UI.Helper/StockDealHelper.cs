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


        public static string[] GetStockName(List<MyStock> list, int top)
        {
            var a1 = list.OrderByDescending(p => p.S1).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST")).Select(p => p.Code + " " + p.Name);
            var a2 = list.OrderByDescending(p => p.S2).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST")).Select(p => p.Code + " " + p.Name);
            var a3 = list.OrderByDescending(p => p.S3).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST")).Select(p => p.Code + " " + p.Name);
            var a4 = list.OrderByDescending(p => p.S4).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST")).Select(p => p.Code + " " + p.Name);
            var b1 = a1.Union(a2);
            var b2 = a3.Union(a4);
            return b1.Where(p => b2.Contains(p)).Distinct().ToArray();
        }

        public static string[] GetStockNameByXS(List<MyStock> list, int top = 0)
        {
            var tmp = list.Where(p => p.Tmp > -100 && !p.Name.Contains("ST") && (p.S1 - p.S3 * 2 > 0 || p.S2 - p.S4 > 0));
            if (top <= 0)
            {
                var sp = SetOrder(tmp).OrderBy(p => p.Order);
                return sp.Select(p => p.Code + " " + p.Name + " " + p.Order).ToArray();
            }
            else
            {
                var a1 = tmp.OrderByDescending(p => p.S1).Take(top);
                var a2 = tmp.OrderByDescending(p => p.S2).Take(top);
                var a3 = tmp.OrderByDescending(p => p.S3).Take(top);
                var a4 = tmp.OrderByDescending(p => p.S4).Take(top);
                var b1 = a1.Union(a2);
                var b2 = a3.Union(a4);
                var b = SetOrder(b1.Where(p => b2.Count(q => q.Code == p.Code) > 0).Distinct()).OrderBy(p => p.Order);
                return b.Select(p => p.Code + " " + p.Name + " " + p.Order).ToArray();
            }
        }

        public static double StockScore(MyStock item)
        {
            return (item.K1 + item.K2 + item.K3 + item.K4) / 4;
        }

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

        public static string[] GetStockNameBK(List<MyStock> list, int top)
        {
            var a1 = list.OrderByDescending(p => p.S1).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var a2 = list.OrderByDescending(p => p.S2).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var a3 = list.OrderByDescending(p => p.S3).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var a4 = list.OrderByDescending(p => p.S4).Take(top).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var b1 = a1.Union(a2);
            var b2 = a3.Union(a4);
            var b = b1.Where(p => b2.Count(q => q.Code == p.Code) > 0).Distinct();
            return b.Select(p => p.Code + " " + p.Name + " " + StockScore(p)).ToArray();
        }

        public static void Monitor(List<MyStock> list, string dir, string encode)
        {
            var list1 = list.OrderByDescending(p => p.S1).Take(500).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var list2 = list.OrderByDescending(p => p.S2).Take(500).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var list3 = list.OrderByDescending(p => p.S3).Take(500).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            var list4 = list.OrderByDescending(p => p.S4).Take(500).Where(p => p.Tmp > -100 && !p.Name.Contains("ST"));
            FileBase.WriteFile(dir, "S1.txt", string.Join("\t\n", list1.Select(p => p.Name)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "S2.txt", string.Join("\t\n", list2.Select(p => p.Name)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "S3.txt", string.Join("\t\n", list3.Select(p => p.Name)), encode, FileBaseMode.Create);
            FileBase.WriteFile(dir, "S4.txt", string.Join("\t\n", list4.Select(p => p.Name)), encode, FileBaseMode.Create);
        }

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

        public static void Deal(List<MyStock> list, string encode = "utf-8")
        {
            var dir = "./dest";
            var _content = new List<string>();
            for (var i = 25; i <= 400; i += 25)
            {
                var dt = GetStockName(list, i);
                _content.Add(i + " " + ((dt.Length + 0.0) / i).ToString("0.000"));
                FileBase.WriteFile(dir, "K" + i + ".txt", string.Join("\t\n", dt), encode,
                    FileBaseMode.Create);
            }
            FileBase.WriteFile(dir, "K500.txt", string.Join("\t\n", GetStockName(list, 500)), encode,
                FileBaseMode.Create);
            FileBase.WriteFile(dir, "K825.txt", string.Join("\t\n", GetStockName(list, 825)), encode,
                FileBaseMode.Create);

            for (var i = 25; i <= 400; i += 25)
            {
                FileBase.WriteFile(dir, "T" + i + ".txt", string.Join("\t\n", GetStockNameByXS(list, i)), encode,
                    FileBaseMode.Create);
            }

            FileBase.WriteFile(dir, "T0400.txt", string.Join("\t\n", _content), encode,
                FileBaseMode.Create);

            Monitor(list, dir, encode);
        }

        public static void Deal2(List<MyStock> list, string encode = "utf-8")
        {
            var dir = "./dest";
            var tmp = list.Where(p => p.K4 >= 0).OrderByDescending(p => p.K3).ToList();

            for (var i = 3; i <= 48; i += 3)
            {
                FileBase.WriteFile(dir, "B" + i + ".txt", string.Join("\t\n", GetStockNameBK(tmp, i)), encode, FileBaseMode.Create);
            }
        }

        public static void Program()
        {
            Console.BackgroundColor = ConsoleColor.White;
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
