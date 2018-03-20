using System;
using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockPerformanceHelper
    {
        /// <summary>
        /// 第一期:60个点
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static List<StockPerformance> Init(DateTime start)
        {
            var ret = new List<StockPerformance>
            {
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
