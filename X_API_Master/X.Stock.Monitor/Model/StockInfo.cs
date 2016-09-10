using System;

namespace X.Stock.Monitor.Model
{
    public class StockInfo
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public decimal StockPrice { get; set; }

        public decimal StockKm1 { get; set; }

        public decimal StockKm2 { get; set; }

        public DateTime Now { get; set; }
    }
}
