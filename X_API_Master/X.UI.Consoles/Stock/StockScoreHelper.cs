﻿using System.Collections.Generic;
using System.Linq;

namespace X.UI.Consoles.Stock
{
    public class StockScoreHelper
    {
        public static StockScore GetScore(Stock current, Stock previous)
        {
            return new StockScore
            {
                Item1 = current.High / previous.High,
                Item2 = current.Open / previous.Close,
                Item3 = current.Close / previous.Close,
                Item4 = current.Close / previous.Close,
                Item5 = 2 * current.Close / (current.High + current.Low),
                Item6 = current.Low / previous.Low,
                Item7 = current.Close / previous.High,
                Stock = current
            };
        }

        public static List<StockScore> GetScore(List<Stock> list)
        {
            var ret = new List<StockScore>();
            for (var i = 1; i < list.Count; i++)
            {
                ret.Add(GetScore(list[i], list[i - 1]));
            }
            return ret;
        }

        private static double Mul(IEnumerable<StockScore> list, int num, int index)
        {
            var ret = index + 1 > num ? list.Skip(index + 1 - num).Take(num) : list.Take(index + 1);
            return ret.Aggregate<StockScore, double>(1, (current, t) => current * t.Value);
        }

        public static List<StockScoreExtend> GetScoreExtend(List<StockScore> list)
        {
            return list.Select((t, i) => new StockScoreExtend
            {
                Stock = t.Stock,
                Score = t.Value,
                Score2 = Mul(list, 2, i),
                Score3 = Mul(list, 3, i),
                Score4 = Mul(list, 4, i),
                Score5 = Mul(list, 5, i),
                Score6 = Mul(list, 6, i),
                Score7 = Mul(list, 7, i),
                Score8 = Mul(list, 8, i),
                Score9 = Mul(list, 9, i),
                Score10 = Mul(list, 10, i),
                Score11 = Mul(list, 11, i),
                Score12 = Mul(list, 12, i),
                Score13 = Mul(list, 13, i),
                Score14 = Mul(list, 14, i),
                Score15 = Mul(list, 15, i),
                Score16 = Mul(list, 16, i),
                Score17 = Mul(list, 17, i),
                Score18 = Mul(list, 18, i),
                Score19 = Mul(list, 19, i)
            }).ToList();
        }
    }
}