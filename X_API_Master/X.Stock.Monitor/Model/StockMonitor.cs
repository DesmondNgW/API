using System;
using X.Util.Entities;

namespace X.Stock.Monitor.Model
{
    public class StockMonitor : MongoBaseModel
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public decimal StockPrice { get; set; }

        public decimal StockKm { get; set; }

        public DateTime CreateTime { get; set; }

        public string State { get; set; }
    }
}
