using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using X.Util.Core.Common;

namespace X.UI.Consoles
{
    public class Stock
    {
        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Inc { get; set; }

        public Dictionary<int, double> Y { get; set; }
    }

    public class Boll
    {
        public double U { get; set; }

        public double L { get; set; }

        public double B { get; set; }

        public double V { get; set; }

        public int N { get; set; }

        public int C { get; set; }

        public Stock Stock { get; set; }
    }


    public class StockHelper
    {
        private static double R(double upStep, double downStep)
        {
            return StringConvert.SysRandom.NextDouble()*(upStep - downStep) + downStep;
        }

        public static List<Stock> G(double upStep, double downStep, int count, double start)
        {
            var list = new List<Stock>();
            while (count-- > 0)
            {
                var last = list.LastOrDefault();
                var b = last != null ? last.Close : start;
                var p1 = b * (1 + R(upStep, downStep));
                var p2 = b * (1 + R(upStep, downStep));
                var p3 = b * (1 + R(upStep, downStep));
                var p4 = b * (1 + R(upStep, downStep));
                var s = new Stock
                {
                    Close = p1,
                    Open = p2,
                    High = Math.Max(Math.Max(Math.Max(p1, p2), p3), p4),
                    Low = Math.Min(Math.Min(Math.Min(p1, p2), p3), p4),
                    Y = new Dictionary<int, double>(),
                    Inc = (p1 - b) / b
                };
                list.Add(s);
            }
            for (var i = 0; i < list.Count; i++)
            {
                for (var j = 1; j <= 20; j++)
                {
                    if (i + j < list.Count)
                    {
                        list[i].Y[j] = (list[i + j].Close - list[i].Close)/list[i].Close;
                    }
                }
                for (var j = 1; j <= 20; j++)
                {
                    if (!list[i].Y.ContainsKey(j))
                    {
                        list[i].Y[j] = 0;
                    }
                }
            }
            return list;
        }
    }

    public class BollHelper
    {
        /// <summary>
        /// 标准差计算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Boll GetBoll(List<Stock> data, int c)
        {
            var b = new Boll
            {
                N = data.Count,
                C = c,
                B = data.Average(p => p.Close),
                Stock = data.Last()
            };
            var sd = Math.Sqrt(data.Sum(p => Math.Pow(p.Close - data.Average(q => q.Close), 2))/data.Count);
            b.U = b.B + c*sd;
            b.L = b.B - c*sd;
            b.V = (b.U - b.B)/b.B;
            return b;
        }

        public static List<Boll> GetBoll(List<Stock> data, int n, int c)
        {
            var ret = new List<Boll>();
            for (var i = 0; i <= data.Count - n; i++)
            {
                ret.Add(GetBoll(data.Skip(i).Take(n).ToList(), c));
            }
            return ret;
        }


        /*
         * 
         * 
         * 
         * 
         * 
         */

    }
}
