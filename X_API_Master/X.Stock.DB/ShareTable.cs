using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Driver.Builders;
using X.Stock.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Stock.DB
{
    public class ShareTable
    {
        /// <summary>
        /// 获取份额(非0)
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public static List<StockShare> GetStockShare(string customerNo)
        {
            var query = Query.And(Query.EQ("CustomerNo", customerNo), Query.GT("TotalVol", 0));
            var sortBy = SortBy.Descending("CreateTime");
            return MongoDbBase<StockShare>.Default.Find("Stock", "Share", null, query, null, sortBy).ToList();
        }

        /// <summary>
        /// 获取份额可卖
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public static List<StockShare> GetStockShareSell(string customerNo)
        {
            return GetStockShare(customerNo).Where(p => p.CreateTime != DateTime.Now.Date).ToList();
        }

        /// <summary>
        /// StockBuy
        /// </summary>
        /// <param name="target"></param>
        /// <param name="total"></param>
        /// <param name="customerNo"></param>
        /// <param name="customerName"></param>
        public static void StockBuy(StockInfo target, double total, string customerNo, string customerName)
        {
            var vol = Math.Floor(total / target.StockPrice / 100) * 100;
            if (vol <= 0) return;
            var amount = vol * target.StockPrice;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "buy stock " + target.StockCode, string.Empty);
            MonitorTradeTable.MonitorTrade(target, vol, customerNo, customerName, MonitorState.Buy);
            MongoDbBase<StockShare>.Default.InsertMongo(new StockShare
            {
                Id = target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd"),
                CustomerNo = customerNo,
                CustomerName = customerName,
                StockCode = target.StockCode,
                StockName = target.StockName,
                CostValue = target.StockPrice,
                TotalVol = vol,
                AvailableVol = vol,
                CurrentStockPrice = target.StockPrice,
                CreateTime = DateTime.Now.Date,
                UpdateTime = DateTime.Now
            }, "Stock", "Share", null);
            CustomerTable.UpdateCustomerInfo(customerNo, 0 - amount);
        }

        /// <summary>
        /// StockSell
        /// </summary>
        /// <param name="target"></param>
        /// <param name="share"></param>
        public static void StockSell(StockInfo target, StockShare share)
        {
            share.CurrentStockPrice = target.StockPrice;
            share.TotalVol = 0;
            share.AvailableVol = 0;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Sell stock " + target.StockCode, string.Empty);
            MonitorTradeTable.MonitorTrade(target, 0, share.CustomerNo, share.CustomerName, MonitorState.Sell);
            var amount = target.StockPrice * share.TotalVol;
            MongoDbBase<StockShare>.Default.SaveMongo(share, "Stock", "Share", null);
            CustomerTable.UpdateCustomerInfo(share.CustomerNo, amount);
        }
    }
}
