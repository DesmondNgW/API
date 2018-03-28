using System;
using System.Collections.Generic;

namespace X.UI.Entities
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

        /// <summary>
        /// HeiKinAShiClose
        /// </summary>
        public double HeiKinAShiClose
        {
            get { return Close + High + Low + Open; }
        }

        /// <summary>
        /// HeiKinAShiLow
        /// </summary>
        public double HeiKinAShiLow
        {
            get { return Math.Min(Math.Min(HeiKinAShiClose, HeiKinAShiOpen), Low); }
        }

        /// <summary>
        /// HeiKinAShiOpen
        /// </summary>
        public double HeiKinAShiOpen { get; set; }

        /// <summary>
        /// HeiKinAShiHigh
        /// </summary>
        public double HeiKinAShiHigh
        {
            get { return Math.Max(Math.Max(HeiKinAShiClose, HeiKinAShiOpen), High); }
        }

        /// <summary>
        /// 概率数据
        /// </summary>
        public Dictionary<string, double> Feature { get; set; }
    }
}
