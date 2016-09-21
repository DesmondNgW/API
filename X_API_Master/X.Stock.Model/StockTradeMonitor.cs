﻿using System;
using X.Util.Entities;

namespace X.Stock.Model
{
    public class StockTradeMonitor : MongoBaseModel
    {
        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        public double TotalVol { get; set; }

        public double StockPrice { get; set; }

        public double StockKm { get; set; }

        public DateTime CreateTime { get; set; }

        public MonitorState State { get; set; }
    }
}
