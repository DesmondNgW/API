﻿using System;

namespace X.Stock.Monitor.Model
{
    public class StockShare
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
    }
}
