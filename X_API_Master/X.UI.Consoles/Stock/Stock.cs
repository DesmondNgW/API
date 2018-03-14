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
        public double Ma  { get; set; }

        /// <summary>
        /// 标准差
        /// </summary>
        public double Std { get; set; }

        /// <summary>
        /// 标准分数
        /// </summary>
        public double ZScore
        {
            get { return Std > 0 ? (Close - Ma) / Std : 0; }
        }

        /// <summary>
        /// 变异系数
        /// </summary>
        public double CoefficientVariation
        {
            get { return Ma > 0 ? Std / Ma : 0; }
        }

        /// <summary>
        /// 标准分数总体均值
        /// </summary>
        public double ZScoreMa { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// ScoreMax
        /// </summary>
        public double ScoreMax { get; set; }
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
