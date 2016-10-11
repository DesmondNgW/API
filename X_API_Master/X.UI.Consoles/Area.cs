using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.Util.Core.Common;
using X.Util.Entities;
using X.Util.Other;

namespace X.UI.Consoles
{
    public class AreaModel
    {
        /// <summary>
        /// 区间长度
        /// </summary>
        public int ZoneLength { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 忽略中间变化的面积
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// 计算中间变化的面积（接近真实面积）
        /// </summary>
        public double SumArea { get; set; }

        /// <summary>
        /// 计算面积比率
        /// </summary>
        public double AreaRate { get; set; }

        /// <summary>
        /// 幅度
        /// </summary>
        public double Inc3 { get; set; }
        public double Inc5 { get; set; }
        public double Inc10 { get; set; }
        public double Inc20 { get; set; }

    }


    public class Area
    {
        /// <summary>
        /// 生成随机统计样本
        /// </summary>
        /// <param name="count">样本数量</param>
        /// <param name="start">初始值</param>
        /// <param name="stepMax">变化幅度上限</param>
        /// <param name="stepMin">变化幅度下限</param>
        /// <returns></returns>
        public static List<double> Greanate(int count, double start, double stepMax, double stepMin)
        {
            stepMax = Math.Abs(stepMax);
            stepMin = Math.Abs(stepMin);
            var result = new List<double> ();
            while (count-- > 0)
            {
                var tmp = StringConvert.SysRandom.NextDouble()*(stepMax + stepMin) - stepMin;
                result.Add(start*(1 + tmp));
            }
            return result;
        }

        /// <summary>
        /// 针对样本数据，计算面积模型
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<AreaModel> GetAreaInfo(List<double> datas, int length)
        {
            var count = datas.Count - length + 1;
            if (count < 1) return null; 
            var result = new List<AreaModel>();
            for (var i = 0; i < count; i++)
            {
                var model = new AreaModel
                {
                    Price = datas[i],
                    ZoneLength = length,
                    Area = length*(datas[i] + datas[i + length - 1])/2,
                    SumArea = -(datas[i] + datas[i + length - 1])/2,
                    Inc3 = double.MinValue,
                    Inc5 = double.MinValue,
                    Inc10 = double.MinValue,
                    Inc20 = double.MinValue,
                };
                for (var j = 0; j < length; j++)
                {
                    model.SumArea += datas[i + j];
                }
                //model.Area -= length*datas[i];
                //model.SumArea -= length * datas[i];
                model.AreaRate = model.SumArea/model.Area;
                if (result.Count >= 3)
                {
                    model.Inc3 = datas[i]/result[result.Count - 3].Price - 1;
                }
                if (result.Count >= 5)
                {
                    model.Inc5 = datas[i] / result[result.Count - 5].Price - 1;
                }
                if (result.Count >= 10)
                {
                    model.Inc10 = datas[i] / result[result.Count - 10].Price - 1;
                }
                if (result.Count >= 20)
                {
                    model.Inc20 = datas[i] / result[result.Count - 20].Price - 1;
                }
                result.Add(model);
            }
            return result;
        }

        public static Dictionary<double, int> GetCountResult(List<AreaModel> result)
        {
            var iresult = new Dictionary<double, int>();
            var max = result.Max(p => p.AreaRate);
            var min = result.Min(p => p.AreaRate);
            var c = Math.Floor(min * 100) / 100;
            while (c < max)
            {
                iresult[c] = result.Count(p => p.AreaRate >= c && p.AreaRate <= c + 0.01);
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
                    c2 = c1/item.Value;
                }
                Console.WriteLine("区间：{0}~{1},总数:{2},有效:{3},效率：{4}", item.Key, item.Key + 0.01, item.Value, c1, c2);
            }
        }


        public static void AreaTest()
        {
            var list = Greanate(1000, 20, 0.1, 0.1);
            var result = GetAreaInfo(list, 12);

            var list1 = GetCountResult(result);

            result = result.Where(p => p.Inc3 >= 0.06).ToList();

            var list2 = GetCountResult(result);

            CountTest(list1, list2);

            FileBase.DeleteFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "123.csv"));
            foreach (var item in result)
            {
                FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "123.csv", string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", item.Area, item.SumArea, item.AreaRate, item.Price, item.Inc3, item.Inc5, item.Inc10, item.Inc20), "utf8", FileBaseMode.Append);
            }
            Console.WriteLine("AreaModel.MaxAreaRate:{0}", result.Max(p => p.AreaRate));
            Console.WriteLine("AreaModel.MinAreaRate:{0}", result.Min(p => p.AreaRate));
        }
    }
}
