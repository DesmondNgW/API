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
        /// Strong
        /// </summary>
        public double Strong { get; set; }

        /// <summary>
        /// StrongLength
        /// </summary>
        public int StrongLength { get; set; }

        public double ComputePrice { get; set; }

        public double ComputePrice2 { get; set; }

        public double Remark { get; set; }
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
