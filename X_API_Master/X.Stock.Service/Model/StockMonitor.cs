﻿using System;
using X.Util.Entities;

namespace X.Stock.Service.Model
{
    public enum MonitorState
    {
        Buy = 0,
        Sell = 1
    }

    public class StockMonitor : MongoBaseModel
    {
        public string StockCode { get; set; }

        public string StockName { get; set; }

        public decimal StockPrice { get; set; }

        public decimal StockKm { get; set; }

        public DateTime CreateTime { get; set; }

        public MonitorState State { get; set; }
    }
}
