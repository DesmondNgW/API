using System;

namespace X.UI.Entities
{
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
}
