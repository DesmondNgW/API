using System;
using X.Util.Entities;

namespace X.Stock.Service.Model
{
    public class StockShare : MongoBaseModel
    {
        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public decimal CostValue { get; set; }

        public decimal TotalVol { get; set; }

        public decimal AvailableVol { get; set; }

        public decimal CurrentStockPrice { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
