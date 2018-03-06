using System;
using System.Collections.Generic;

namespace X.UI.Consoles.Stock
{
    public class Stock
    {
        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Inc { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public DateTime? Date { get; set; }

        public Dictionary<int, StockSimple> StockSimple { get; set; }

        /// <summary>
        /// 均值
        /// </summary>
        public double Ev  { get; set; }

        /// <summary>
        /// 标准差
        /// </summary>
        public double Std { get; set; }

        /// <summary>
        /// 标准分数
        /// </summary>
        public double ZScore
        {
            get { return Std > 0 ? (Close - Ev) / Std : 0; }
        }

        /// <summary>
        /// 变异系数
        /// </summary>
        public double CoefficientVariation
        {
            get { return Ev > 0 ? Std / Ev : 0; }
        }

        /// <summary>
        /// 标准分数总体均值
        /// </summary>
        public double Ze { get; set; }

        /// <summary>
        /// 变异系数总体均值
        /// </summary>
        public double Cve { get; set; }
    }

    public class StockSimple
    {
        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public DateTime Date { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }
    }
}
