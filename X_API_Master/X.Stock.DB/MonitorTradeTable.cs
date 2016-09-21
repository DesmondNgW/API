using System;
using X.Stock.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.DB
{
    public class MonitorTradeTable
    {
        /// <summary>
        /// 监控交易
        /// </summary>
        /// <param name="target"></param>
        /// <param name="totalVol"></param>
        /// <param name="customerNo"></param>
        /// <param name="customerName"></param>
        /// <param name="state"></param>
        public static void MonitorTrade(StockInfo target, double totalVol, string customerNo, string customerName, MonitorState state)
        {
            MongoDbBase<StockTradeMonitor>.Default.InsertMongo(new StockTradeMonitor
            {
                CustomerNo = customerNo,
                CustomerName = customerName,
                StockCode = target.StockCode,
                StockName = target.StockName,
                StockPrice = target.StockPrice,
                StockKm = target.StockKm2,
                TotalVol = totalVol,
                CreateTime = DateTime.Now,
                State = state
            }, "Stock", "StockTradeMonitor", null);
        }
    }
}
