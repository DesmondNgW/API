using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core.Common;

namespace X.UI.Consoles
{
    public class StatisticsHelper
    {
        /// <summary>
        /// 生成随机统计样本
        /// </summary>
        /// <param name="count">样本数量</param>
        /// <param name="start">初始值</param>
        /// <param name="stepMax">变化幅度上限</param>
        /// <param name="stepMin">变化幅度下限</param>
        /// <returns></returns>
        public static List<Price> Greanate(int count, double start, double stepMax, double stepMin)
        {
            stepMax = Math.Abs(stepMax);
            stepMin = Math.Abs(stepMin);
            var result = new List<Price>();
            while (count-- > 0)
            {
                var p1 = start*((StringConvert.SysRandom.NextDouble()*(stepMax + stepMin) - stepMin) + 1);
                var p2 = start*((StringConvert.SysRandom.NextDouble()*(stepMax + stepMin) - stepMin) + 1);
                var p3 = start*((StringConvert.SysRandom.NextDouble()*(stepMax + stepMin) - stepMin) + 1);
                var p4 = start*((StringConvert.SysRandom.NextDouble()*(stepMax + stepMin) - stepMin) + 1);
                var price = new Price()
                {
                    Open = p1,
                    Close = p2,
                    High = Math.Max(Math.Max(Math.Max(p1, p2), p3), p4),
                    Low = Math.Min(Math.Min(Math.Min(p1, p2), p3), p4)
                };
                result.Add(price);
            }
            return result;
        }

        /// <summary>
        /// 计算策略结果
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="circle1"></param>
        /// <param name="circle2"></param>
        /// <returns></returns>
        public static List<Strategy> StrategyResult(List<Price> datas, int circle1, int circle2)
        {
            var count = datas.Count - circle2 + 1;
            if (count < 1) return null;
            var result = new List<Strategy>();
            for (var i = 0; i < count; i++)
            {
                var model = new Strategy(circle1, circle2)
                {
                    Price = datas[i],
                    Result = new AreaResult
                    {
                        Area2 = circle2 * (datas[i].Close + datas[i + circle2 - 1].Close) / 2,
                        Area1 = -(datas.Skip(i).Take(circle1 - 1).Sum(p => p.Close) + datas.Skip(i + circle2 - circle1 - 1).Take(circle1 - 1).Sum(p => p.Close)) / 2
                    },
                    Wave = new Wave()
                };
                for (var j = 0; j < circle2; j++)
                {
                    model.Result.Area1 += datas[i + j].Close;
                }
                model.Result.AreaRate = model.Result.Area1 / model.Result.Area2;
                if (i < count - 3)
                {
                    model.Wave.Wave3After = datas[i + 3].Close / datas[i].Close - 1;
                }
                if (i < count - 5)
                {
                    model.Wave.Wave5After = datas[i + 5].Close / datas[i].Close - 1;
                }
                if (i < count - 7)
                {
                    model.Wave.Wave7After = datas[i + 7].Close / datas[i].Close - 1;
                }
                if (i < count - 10)
                {
                    model.Wave.Wave10After = datas[i + 10].Close / datas[i].Close - 1;
                }
                if (i < count - 20)
                {
                    model.Wave.Wave10After = datas[i + 20].Close / datas[i].Close - 1;
                }
                if (i >= 3)
                {
                    model.Wave.Wave3Before = datas[i].Close / datas[i - 3].Close - 1;
                }
                if (i >= 5)
                {
                    model.Wave.Wave5Before = datas[i].Close / datas[i - 5].Close - 1;
                }
                if (i >= 7)
                {
                    model.Wave.Wave7Before = datas[i].Close / datas[i - 7].Close - 1;
                }
                if (i >= 10)
                {
                    model.Wave.Wave10Before = datas[i].Close / datas[i - 10].Close - 1;
                }
                if (i >= 20)
                {
                    model.Wave.Wave20Before = datas[i].Close / datas[i - 20].Close - 1;
                }
                result.Add(model);
            }
            return result;
        }

        public static Dictionary<double, int> GetCountResult(List<Strategy> result)
        {
            var iresult = new Dictionary<double, int>();
            var max = result.Max(p => p.Result.AreaRate);
            var min = result.Min(p => p.Result.AreaRate);
            var c = Math.Floor(min * 100) / 100;
            while (c < max)
            {
                iresult[c] = result.Count(p => p.Result.AreaRate >= c && p.Result.AreaRate <= c + 0.01);
                c += 0.01;
            }
            return iresult;
        }

        public static void CountTest(Dictionary<double, int> result1, Dictionary<double, int> result2)
        {
            foreach (var item in result1)
            {
                double c1 = 0;
                double c2 = 0;
                if (result2.ContainsKey(item.Key))
                {
                    c1 = result2[item.Key];
                    c2 = c1 / item.Value;
                }
                Console.WriteLine("区间：{0}~{1},总数:{2},有效:{3},效率：{4}", item.Key, item.Key + 0.01, item.Value, c1, c2);
            }
        }


        public static void StrategyTest()
        {
            var list = Greanate(10000, 20, 0.1, 0.1);
            var result = StrategyResult(list, 2, 12);

            var list1 = GetCountResult(result);

            result = result.Where(p => p.Wave.Wave3After >= 0.06 || p.Wave.Wave5After >= 0.10 || p.Wave.Wave7After >= 0.14 || p.Wave.Wave10After >= 0.20).ToList();

            var list2 = GetCountResult(result);

            CountTest(list1, list2);

            //FileBase.DeleteFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "123.csv"));
            //foreach (var item in result)
            //{
            //    FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "123.csv", string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", item.Result.Area2, item.Result.Area1, item.Result.AreaRate, item.Price, item.Wave.Wave3After, item.Inc5, item.Inc10, item.Inc20), "utf8", FileBaseMode.Append);
            //}
            Console.WriteLine("AreaModel.MaxAreaRate:{0}", result.Max(p => p.Result.AreaRate));
            Console.WriteLine("AreaModel.MinAreaRate:{0}", result.Min(p => p.Result.AreaRate));

        }

    }
}
