using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using X.Stock.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.DB
{
    public class MonitorTable
    {
        /// <summary>
        /// 获取Monitor
        /// </summary>
        /// <returns></returns>
        public static MongoCursor<StockMonitor> GetStockMonitor()
        {
            var query = Query.GTE("CreateTime", DateTime.Now.AddMonths(-1));
            return MongoDbBase<StockMonitor>.Default.Find("Stock", "Monitor", null, query);
        }

        /// <summary>
        /// 监控
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public static void MonitorTrade(StockInfo stock, string id, MonitorState state)
        {
            MongoDbBase<StockMonitor>.Default.InsertMongo(new StockMonitor
            {
                StockCode = stock.StockCode,
                StockName = stock.StockName,
                StockPrice = stock.StockPrice,
                StockKm = stock.StockKm2,
                State = state,
                CreateTime = DateTime.Now,
                Id = id
            }, "Stock", "Monitor", null);
        }
    }
}
