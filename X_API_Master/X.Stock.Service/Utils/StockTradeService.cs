using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using X.Stock.Service.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Stock.Service.Utils
{
    public class StockTradeService
    {
        public static object Blocker = new object();

        public static object Slocker = new object();
        /// <summary>
        /// 买
        /// </summary>
        /// <param name="stocks"></param>
        /// <param name="info"></param>
        public static void BuyStock(List<StockInfo> stocks, AssetInfo info)
        {
            var count = info.Shares != null ? info.Shares.Count(p => p.TotalVol > 0) : 0;
            if (count >= 4) return;
            lock (Blocker)
            {
                var total = info.CoinAsset/(4 - count);
                if (stocks == null) return;
                var targets = stocks.Where(p => p.StockKm2 >= 3.3 && p.StockKm2 <= 8.8).OrderByDescending(p => p.StockKm2).Take(4 - count);
                Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start Buy stock", string.Empty);
                foreach (var target in targets)
                {
                    var vol = Math.Floor(total / target.StockPrice / 100) * 100;
                    if (vol <= 0) continue;
                    var amount = vol * target.StockPrice;
                    Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Buy stock " + target.StockCode, string.Empty);
                    MongoDbBase<StockShare>.Default.InsertMongo(new StockShare
                    {
                        Id = target.StockCode + "_" + DateTime.Now.ToString("yyyyMMdd"),
                        CustomerNo = info.CustomerNo,
                        CustomerName = info.CustomerName,
                        StockCode = target.StockCode,
                        StockName = target.StockName,
                        CostValue = target.StockPrice,
                        TotalVol = vol,
                        AvailableVol = vol,
                        CurrentStockPrice = target.StockPrice,
                        CreateTime = DateTime.Now.Date,
                        UpdateTime = DateTime.Now
                    }, "Stock", "Share", null);
                    CustomerService.UpdateCustomerInfo(info.CustomerNo, 0 - amount);
                }
            }
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End Buy stock", string.Empty);
        }

        /// <summary>
        /// 卖
        /// </summary>
        /// <param name="info"></param>
        public static void SellStock(AssetInfo info)
        {
            var shares = info.Shares.Where(p => p.TotalVol > 0 && p.CreateTime != DateTime.Now.Date).ToList();
            if (shares.Count <= 0) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start Sell stock", string.Empty);
            var stocks = CustomerService.GetStockInfoFromShares(info);
            foreach (var share in shares.Where(p => p.CreateTime != DateTime.Now.Date && p.TotalVol > 0))
            {
                var share1 = share;
                var stock = stocks.FirstOrDefault(p => p.StockCode == share1.StockCode && p.StockKm2 <= -3.3 && StockService.GetBenifit(share1.CostValue, p.StockPrice) > 0.05);
                if (stock == null) continue;
                lock (Slocker)
                {
                    share1.CurrentStockPrice = stock.StockPrice;
                    share1.TotalVol = 0;
                    share1.AvailableVol = 0;
                    Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Sell stock " + stock.StockCode, string.Empty);
                    var amount = stock.StockPrice*share1.TotalVol;
                    MongoDbBase<StockShare>.Default.SaveMongo(share1, "Stock", "Share", null);
                    CustomerService.UpdateCustomerInfo(info.CustomerNo, amount);
                }
            }
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End Sell stock", string.Empty);
        }

        /// <summary>
        /// 是否交易时间
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static bool CanTrade(DateTime now)
        {
            var amBegin = now.Date + new TimeSpan(9, 31, 0);
            var amEnd = now.Date + new TimeSpan(11, 29, 0);
            var pmBegin = now.Date + new TimeSpan(13, 1, 0);
            var pmEnd = now.Date + new TimeSpan(14, 55, 0);
            if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday && now >= amBegin && now <= pmEnd)
            {
                return now <= amEnd || now >= pmBegin;
            }
            return false;
        }

        /// <summary>
        /// 是否交易日
        /// </summary>
        /// <returns></returns>
        public static bool IsCanTrade()
        {
            if (!CanTrade(DateTime.Now)) return false;
            var result = HttpRuntime.Cache.Get("StockInfo_3000592") as StockInfo;
            if (result != null) return result.Now.Date == DateTime.Now.Date;
            result = StockService.GetStockInfo("3000592");
            HttpRuntime.Cache.Insert("StockInfo_3000592", result, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            return result != null && result.Now.Date == DateTime.Now.Date;
        }
    }
}
