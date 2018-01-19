using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Consoles.Stock
{ 
    /*
     * 资金是推动力、也是阻力。买方资金是推动力，卖方资金是阻力，寻找阻力最小/推动力最大
     * 
     * 
     * 
     * 
     */



    public class StockTest
    {
        public static void Test1Result(IEnumerable<StockScoreExtend> data)
        {
            FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv",
                "Inc|Close|Score|Score2|Score3|Score4|Score5|Score6|Score7|Score8|Score9|Score10|Y1|Y2|Y3|Y4|Y5", "utf-8",
                FileBaseMode.Create);
            foreach (var content in data.Select(item => string.Format(
                "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}",
                item.Stock.Inc,
                item.Stock.Close,
                item.Score,
                item.Score2,
                item.Score3,
                item.Score4,
                item.Score5,
                item.Score6,
                item.Score7,
                item.Score8,
                item.Score9,
                item.Score10,
                item.Stock.Y[1],
                item.Stock.Y[2],
                item.Stock.Y[3],
                item.Stock.Y[4],
                item.Stock.Y[5])))
            {
                FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv", content, "utf-8", FileBaseMode.Append);
            }
        }

        public static void Test2Result(List<StockScoreExtend> data, int count, double step)
        {
            FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "2.csv",
                "Item|Score|Score2|Score3|Score4|Score5|Score6|Score7|Score8|Score9|Score10", "utf-8",
                FileBaseMode.Create);
            for (var i = 0; i < count; i++)
            {
                var st = i * step;
                var et = i * step + step;
                Func<StockScoreExtend, bool> t1 = p => p.Score > st && p.Score < et;
                Func<StockScoreExtend, bool> t2 = p => p.Score2 > st && p.Score2 < et;
                Func<StockScoreExtend, bool> t3 = p => p.Score3 > st && p.Score3 < et;
                Func<StockScoreExtend, bool> t4 = p => p.Score4 > st && p.Score4 < et;
                Func<StockScoreExtend, bool> t5 = p => p.Score5 > st && p.Score5 < et;
                Func<StockScoreExtend, bool> t6 = p => p.Score6 > st && p.Score6 < et;
                Func<StockScoreExtend, bool> t7 = p => p.Score7 > st && p.Score7 < et;
                Func<StockScoreExtend, bool> t8 = p => p.Score8 > st && p.Score8 < et;
                Func<StockScoreExtend, bool> t9 = p => p.Score9 > st && p.Score9 < et;
                Func<StockScoreExtend, bool> t10 = p => p.Score10 > st && p.Score10 < et;
                var content =
                    string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}{9}|{10}",
                        st + "," + et,
                        data.Count(t1),
                        data.Count(t2),
                        data.Count(t3),
                        data.Count(t4),
                        data.Count(t5),
                        data.Count(t6),
                        data.Count(t7),
                        data.Count(t8),
                        data.Count(t9),
                        data.Count(t10));
                FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "2.csv", content, "utf-8", FileBaseMode.Append);
            }
        }

        public static void Test()
        {
            var g = StockHelper.G(0.1, -0.1, 1000, 20);
            var s = StockScoreHelper.GetScore(g);
            var se = StockScoreHelper.GetScoreExtend(s);

            Func<StockScoreExtend, bool> func =
                p => p.Stock.Y[1] >= 0.05 ||
                    p.Stock.Y[2] >= 0.10 ||
                    p.Stock.Y[3] >= 0.15 ||
                    p.Stock.Y[4] >= 0.20 ||
                    p.Stock.Y[5] >= 0.25;

            var all = se.Where(func).ToList();

            Test2Result(all, 50, 0.1);

            Test1Result(all);
        }
    }
}
