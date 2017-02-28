using System;
using System.Collections.Generic;
using System.Linq;
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

        public Dictionary<int, double> Y { get; set; }
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
                    Y = new Dictionary<int, double>()
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
            }
            return list;
        }
    }
}
