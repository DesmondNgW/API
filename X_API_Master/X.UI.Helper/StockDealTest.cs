using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockDealTest
    {
        #region 情绪周期模型
        /// <summary>
        /// 读取源数据
        /// </summary>
        /// <returns></returns>
        public static List<StockCircleUnit> GetStockCircleUnitList()
        {
            var content = FileBase.ReadFile(@"D:\stock\股票工具\Debug\src\1.txt", "gb2312");
            var list = Regex.Split(content, "\r\n", RegexOptions.IgnoreCase);
            var ret = new List<decimal>();
            var result = new List<StockCircleUnit>();
            foreach (string item in list)
            {
                if (!string.IsNullOrEmpty(item))
                    ret.Add(item.Convert2Decimal(0));
            }
            int lastH = 0, lastL = 0;
            for (var i = 1; i < ret.Count - 1; i++)
            {
                if (ret[i] > ret[i + 1] && ret[i] > ret[i - 1])
                {
                    lastH = i;
                    if (lastL != 0)
                    {
                        var item = new StockCircleUnit()
                        {
                            High = ret[i],
                            Low = ret[lastL],
                            Mode = CircleUnitMode.UP,
                            Data = new List<decimal>()
                        };
                        for (var k = lastL; k <= i; k++)
                        {
                            item.Data.Add(ret[k]);
                        }
                        result.Add(item);
                    }
                }
                if (ret[i] < ret[i + 1] && ret[i] < ret[i - 1])
                {
                    lastL = i;
                    if (lastH != 0)
                    {
                        var item = new StockCircleUnit()
                        {
                            High = ret[lastH],
                            Low = ret[i],
                            Mode = CircleUnitMode.DOWN,
                            Data = new List<decimal>()
                        };
                        for (var k = lastH; k <= i; k++)
                        {
                            item.Data.Add(ret[k]);
                        }
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="ret"></param>
        public static void TestCircle(List<StockCircleUnit> ret)
        {
            Console.WriteLine("一日游比例：{0}", (ret.Where(p => p.Data.Count <= 2).Count() + 0.0) / ret.Count);

            Console.WriteLine("高点：{0}，低点：{1}", ret.Average(p => p.High), ret.Average(p => p.Low));

            Console.WriteLine("一日游空仓比例：{0}", (ret.Where(p => p.Data.Count <= 2 && p.Low <= -2 && p.High >= 2).Count() + 0.0) / ret.Count);

            var k1 = ret.Where(p => p.Mode == CircleUnitMode.UP && p.SecondData < 2 && p.Low < -2);
            Console.WriteLine("冰点次日非高潮：{0}", k1.Average(p => p.SecondData));

            var k2 = ret.Where(p => p.Mode == CircleUnitMode.DOWN && p.SecondData > -2 && p.High > 2);
            Console.WriteLine("高潮次日非冰点：{0}", k2.Average(p => p.SecondData));

            var k3 = ret.Where(p => p.Data.Count <= 2 && p.Mode == CircleUnitMode.UP && p.SecondData < 2 && p.Low < -2);
            Console.WriteLine("一日游冰点次日非高潮：{0}", k3.Average(p => p.SecondData));

            var k4 = ret.Where(p => p.Data.Count <= 2 && p.Mode == CircleUnitMode.DOWN && p.SecondData > -2 && p.High > 2);
            Console.WriteLine("一日游高潮次日非冰点：{0}", k2.Average(p => p.SecondData));

            var next = ret.Where(p => p.Data[0] < 2 && p.Data[0] > -2);
            Console.WriteLine("弱分歧中一日游比例：{0}", (next.Where(p => p.Data.Count <= 2).Count() + 0.0) / next.Count());

            Console.WriteLine("弱分歧上升当日结果：{0}", next.Where(p => p.Mode == CircleUnitMode.UP).Average(p => p.Data[0]));
            Console.WriteLine("弱分歧次日上升结果：{0}", next.Where(p => p.Mode == CircleUnitMode.UP).Average(p => p.Data[1]));

            Console.WriteLine("弱分歧当日下降结果：{0}", next.Where(p => p.Mode == CircleUnitMode.DOWN).Average(p => p.Data[0]));
            Console.WriteLine("弱分歧次日下降结果：{0}", next.Where(p => p.Mode == CircleUnitMode.DOWN).Average(p => p.Data[1]));
        }

        /// <summary>
        /// unuse
        /// </summary>
        /// <param name="mode"></param>
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

        #endregion
    }
}
