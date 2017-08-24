using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core.Common;
using X.Util.Entities.Enums;
using X.Util.Other;

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

    public class Score
    {
        public Score()
        {
            Item1 = 1;
            Item2 = 1;
            Item3 = 1;
            Item4 = 1;
            Item5 = 1;
            Item6 = 1;
            Item7 = 1;
            Item8 = 1;
            Item9 = 1;
            Item10 = 1;
        }
        public double Item1 { get; set; }

        public double Item2 { get; set; }

        public double Item3 { get; set; }

        public double Item4 { get; set; }

        public double Item5 { get; set; }

        public double Item6 { get; set; }

        public double Item7 { get; set; }

        public double Item8 { get; set; }

        public double Item9 { get; set; }

        public double Item10 { get; set; }

        public double Value
        {
            get { return Item1*Item2*Item3*Item4*Item5*Item6*Item7*Item8*Item9*Item10; }
        }

        public Stock Stock { get; set; }
    }

    public class ScoreExtend
    {
        public double Score { get; set; }

        public double Score2 { get; set; }

        public double Score3 { get; set; }

        public double Score4 { get; set; }

        public double Score5 { get; set; }

        public double Score6 { get; set; }

        public double Score7 { get; set; }

        public double Score8 { get; set; }

        public double Score9 { get; set; }

        public double Score10 { get; set; }

        public double Score11 { get; set; }

        public double Score12 { get; set; }

        public double Score13 { get; set; }

        public double Score14 { get; set; }

        public double Score15 { get; set; }

        public double Score16 { get; set; }

        public double Score17 { get; set; }

        public double Score18 { get; set; }

        public double Score19 { get; set; }

        public double Score20 { get; set; }

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

    public class ScoreHelper
    {
        public static Score GetScore(Stock current, Stock previous)
        {
            return new Score
            {
                Item1 = current.High/previous.High,
                Item2 = current.Open/previous.Close,
                Item3 = current.Close/previous.Close,
                Item4 = current.Close/previous.Close,
                Item5 = 2*current.Close/(current.High + current.Low),
                Item6 = current.Low/previous.Low,
                Item7 = current.Close/previous.High,
                Stock = current
            };
        }

        public static List<Score> GetScore(List<Stock> list)
        {
            var ret = new List<Score>();
            for (var i = 1; i < list.Count; i++)
            {
                ret.Add(GetScore(list[i], list[i - 1]));
            }
            return ret;
        }

        private static double Mul(IEnumerable<Score> list, int num, int index)
        {
            var ret = index + 1 > num ? list.Skip(index + 1 - num).Take(num) : list.Take(index + 1);
            return ret.Aggregate<Score, double>(1, (current, t) => current*t.Value);
        }

        public static List<ScoreExtend> GetScoreExtend(List<Score> list)
        {
            return list.Select((t, i) => new ScoreExtend
            {
                Stock = t.Stock, Score = t.Value, Score2 = Mul(list, 2, i), Score3 = Mul(list, 3, i), Score4 = Mul(list, 4, i), Score5 = Mul(list, 5, i), Score6 = Mul(list, 6, i), Score7 = Mul(list, 7, i), Score8 = Mul(list, 8, i), Score9 = Mul(list, 9, i), Score10 = Mul(list, 10, i), Score11 = Mul(list, 11, i), Score12 = Mul(list, 12, i), Score13 = Mul(list, 13, i), Score14 = Mul(list, 14, i), Score15 = Mul(list, 15, i), Score16 = Mul(list, 16, i), Score17 = Mul(list, 17, i), Score18 = Mul(list, 18, i), Score19 = Mul(list, 19, i)
            }).ToList();
        }
    }

    public enum Operate
    {
        Buy,
        Sell,
        None,
    }

    public class StockPerformance
    {
        public string StockCode { get; set; }

        public DateTime CurrentDate { get; set; }

        public Operate Operate { get; set; }

        public double Profit { get; set; }

        public int Count { get; set; }
    }

    public class StockPerformanceHelper
    {
        public static List<StockPerformance> Init(DateTime start)
        {
            var ret = new List<StockPerformance>
            {
                //2017-08-14
                new StockPerformance
                {
                    StockCode = "002600",
                    CurrentDate = new DateTime(2017, 8, 14),
                    Operate = Operate.Buy,
                    Profit = 3.5
                },
                //2017-08-15
                new StockPerformance
                {
                    StockCode = "002600",
                    CurrentDate = new DateTime(2017, 8, 15),
                    Operate = Operate.None,
                    Profit = 10
                },
                //2017-08-16
                new StockPerformance
                {
                    StockCode = "002600",
                    CurrentDate = new DateTime(2017, 8, 16),
                    Operate = Operate.Sell,
                    Profit = 3
                },
                new StockPerformance
                {
                    StockCode = "300686",
                    CurrentDate = new DateTime(2017, 8, 16),
                    Operate = Operate.Buy,
                    Profit = 4
                },
                //2017-08-17
                new StockPerformance
                {
                    StockCode = "300686",
                    CurrentDate = new DateTime(2017, 8, 17),
                    Operate = Operate.Sell,
                    Profit = 5
                },
                new StockPerformance
                {
                    StockCode = "600198",
                    CurrentDate = new DateTime(2017, 8, 17),
                    Operate = Operate.Buy,
                    Profit = -2.5
                },
                //2017-08-18
                new StockPerformance
                {
                    StockCode = "600198",
                    CurrentDate = new DateTime(2017, 8, 18),
                    Operate = Operate.Sell,
                    Profit = -2
                },
                new StockPerformance
                {
                    StockCode = "002128",
                    CurrentDate = new DateTime(2017, 8, 18),
                    Operate = Operate.Buy,
                    Profit = -3.5
                },
                //2017-08-21
                new StockPerformance
                {
                    StockCode = "002128",
                    CurrentDate = new DateTime(2017, 8, 21),
                    Operate = Operate.Sell,
                    Profit = -1
                },
                new StockPerformance
                {
                    StockCode = "300686",
                    CurrentDate = new DateTime(2017, 8, 21),
                    Operate = Operate.Buy,
                    Profit = 6
                },
                //2017-08-22
                new StockPerformance
                {
                    StockCode = "300686",
                    CurrentDate = new DateTime(2017, 8, 22),
                    Operate = Operate.None,
                    Profit = 6.5
                },
                //2017-08-23
                new StockPerformance
                {
                    StockCode = "300686",
                    CurrentDate = new DateTime(2017, 8, 23),
                    Operate = Operate.Sell,
                    Profit = 4.5
                },
                new StockPerformance
                {
                    StockCode = "300675",
                    CurrentDate = new DateTime(2017, 8, 23),
                    Operate = Operate.Buy,
                    Profit = 0
                },
                //2017-08-24
                new StockPerformance
                {
                    StockCode = "300675",
                    CurrentDate = new DateTime(2017, 8, 24),
                    Operate = Operate.Sell,
                    Profit = 0
                },
                new StockPerformance
                {
                    StockCode = "002677",
                    CurrentDate = new DateTime(2017, 8, 24),
                    Operate = Operate.Buy,
                    Profit = 0.5
                }
            };
















            return ret.Where(p => p.CurrentDate >= start).ToList();
        }

        public static void Compute(List<StockPerformance> list)
        {
            var ret = list.Sum(it => it.Profit);

            var min = list.Min(it => it.CurrentDate);

            var max = list.Max(it => it.CurrentDate);

            FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory,
                min.ToString("yyyy-MM-dd") + "_" + max.ToString("yyyy-MM-dd"), string.Format("{0}--{1}:收益{2},目标{3}",
                    min.ToString("yyyy-MM-dd"),
                    max.ToString("yyyy-MM-dd"),
                    ret,
                    20), "utf8", FileBaseMode.Append);
        }
    }

}
