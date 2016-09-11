using System;
using System.Collections.Generic;
using System.Linq;
using X.Stock.Monitor.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.Monitor.Utils
{
    public class MonitorService
    {
        public static void MonitorBuy(List<StockInfo> stocks)
        {
            var targets = stocks.Where(p => p.StockKm2 >= 3.3M && p.StockKm2 <= 8.8M).OrderByDescending(p => p.StockKm2);
            foreach (var monitor in targets.Select(target => new StockMonitor()
            {
                StockCode = target.StockCode,
                StockName = target.StockName,
                StockPrice = target.StockPrice,
                StockKm = target.StockKm2,
                State = "Buy",
                CreateTime = DateTime.Now,
                Id = target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd")
            }))
            {
                MongoDbBase<StockMonitor>.Default.InsertMongo(monitor, "Stock", "Monitor", null);
            }
        }
    }
}
