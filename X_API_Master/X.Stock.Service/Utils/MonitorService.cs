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
        private const string Collection = "Monitor";
        private const string DataBase = "Stock";

        private static string GetId(string stockCode, MonitorState state)
        {
            return stockCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + state;
        }

        public static void MonitorBuy(List<StockInfo> stocks)
        {
            const MonitorState state = MonitorState.Buy;
            var targets = stocks.Where(p => p.StockKm2 >= 3.3 && p.StockKm2 <= 8.8);
            var list = GetStockMonitor();
            foreach (var target in targets.Where(target => list.Count(p => p.Id == GetId(target.StockCode, state)) == 0))
            {
                var msg = string.Format("Stock Code {0}({1}) has at price {2},inc {3}%", target.StockCode, target.StockName, target.StockPrice, target.StockKm2);
                SmtpMailHelper.Send("Stock." + target.StockCode, msg);
                MongoDbBase<StockMonitor>.Default.InsertMongo(new StockMonitor
                {
                    StockCode = target.StockCode,
                    StockName = target.StockName,
                    StockPrice = target.StockPrice,
                    StockKm = target.StockKm2,
                    State = state,
                    CreateTime = DateTime.Now,
                    Id = GetId(target.StockCode, state)
                }, DataBase, Collection, null);
            }
        }

        public static void MonitorSell(List<StockInfo> stocks)
        {
            const MonitorState state = MonitorState.Sell;
            var targets = stocks.Where(p => p.StockKm2 <=-3.3);
            var list = GetStockMonitor();
            foreach (var target in targets.Where(target => list.Count(p => p.Id == GetId(target.StockCode, state)) == 0))
            {
                var msg = string.Format("Stock Code {0}({1}) has at price {2},inc {3}%", target.StockCode, target.StockName, target.StockPrice, target.StockKm2);
                SmtpMailHelper.Send("Stock." + target.StockCode, msg);
                MongoDbBase<StockMonitor>.Default.InsertMongo(new StockMonitor
                {
                    StockCode = target.StockCode,
                    StockName = target.StockName,
                    StockPrice = target.StockPrice,
                    StockKm = target.StockKm2,
                    State = state,
                    CreateTime = DateTime.Now,
                    Id = GetId(target.StockCode, state)
                }, DataBase, Collection, null);
            }
        }

        public static MongoCursor<StockMonitor> GetStockMonitor()
        {
            var query = Query.GTE("CreateTime", DateTime.Now.AddMonths(-1));
            return MongoDbBase<StockMonitor>.Default.Find("Stock", "Monitor", null, query);
        }
    }
}
