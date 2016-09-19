using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using X.Stock.Service.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.Service.Utils
{
    public class MonitorService
    {
        public static void MonitorBuy(List<StockInfo> stocks)
        {
            var targets = stocks.Where(p => p.StockKm2 >= 3.3M && p.StockKm2 <= 8.8M);
            var list = GetStockMonitor();
            foreach (var target in targets.Where(target => list.Count(p => p.Id == target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_Buy") == 0))
            {
                var msg = string.Format("Stock Code {0}({1}) has at price {2},inc {3}%", target.StockCode, target.StockName, target.StockPrice, target.StockKm2);
                SmtpMailHelper.Send("Stock." + target.StockCode, msg);
                MongoDbBase<StockMonitor>.Default.InsertMongo(new StockMonitor
                {
                    StockCode = target.StockCode,
                    StockName = target.StockName,
                    StockPrice = target.StockPrice,
                    StockKm = target.StockKm2,
                    State = "Buy",
                    CreateTime = DateTime.Now,
                    Id = target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_Buy"
                }, "Stock", "Monitor", null);
            }
        }

        public static void MonitorSell(List<StockInfo> stocks)
        {
            var targets = stocks.Where(p => p.StockKm2 <=-3.3M);
            var list = GetStockMonitor();
            foreach (var target in targets.Where(target => list.Count(p => p.Id == target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_Sell") == 0))
            {
                var msg = string.Format("Stock Code {0}({1}) has at price {2},inc {3}%", target.StockCode, target.StockName, target.StockPrice, target.StockKm2);
                SmtpMailHelper.Send("Stock." + target.StockCode, msg);
                MongoDbBase<StockMonitor>.Default.InsertMongo(new StockMonitor
                {
                    StockCode = target.StockCode,
                    StockName = target.StockName,
                    StockPrice = target.StockPrice,
                    StockKm = target.StockKm2,
                    State = "Buy",
                    CreateTime = DateTime.Now,
                    Id = target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_Sell"
                }, "Stock", "Monitor", null);
            }
        }

        public static MongoCursor<StockMonitor> GetStockMonitor()
        {
            var query = Query.GTE("CreateTime", DateTime.Now.AddMonths(-1));
            return MongoDbBase<StockMonitor>.Default.ReadMongo("Stock", "Monitor", null, query);
        }
    }
}
