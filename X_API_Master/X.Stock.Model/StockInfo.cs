using System;

namespace X.Stock.Model
{
    public class StockInfo
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public double StockPrice { get; set; }

        public double StockKm1 { get; set; }

        public double StockKm2 { get; set; }

        public DateTime Now { get; set; }
    }
}
