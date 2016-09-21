using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using X.Stock.DB;
using X.Stock.Model;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Stock.Service
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
            var count = info.Shares != null ? info.Shares.Count : 0;
            if (count >= 4) return;
            lock (Blocker)
            {
                var total = info.CoinAsset/(4 - count);
                if (stocks == null) return;
                var targets = stocks.Where(p => p.StockKm2 >= 3.3 && p.StockKm2 <= 8.8).OrderByDescending(p => p.StockKm2).Take(4 - count);
                Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start Buy stock", string.Empty);
                foreach (var target in targets)
                {
                    if (info.Shares != null && info.Shares.Count > 0)
                    {
                        if (info.Shares.Count(p => p.StockCode == target.StockCode) <= 0)
                        {
                            ShareTable.StockBuy(target, total, info.CustomerNo, info.CustomerName);
                        }
                    }
                    else
                    {
                        ShareTable.StockBuy(target, total, info.CustomerNo, info.CustomerName);
                    }
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
            var shares = info.Shares.Where(p => p.CreateTime != DateTime.Now.Date).ToList();
            if (shares.Count <= 0) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start Sell stock", string.Empty);
            var stocks = CustomerService.GetStockInfoFromShares(info);
            foreach (var share in shares)
            {
                var share1 = share;
                var stock = stocks.FirstOrDefault(p => p.StockCode == share1.StockCode && p.StockKm2 <= -3.3 && StockService.GetBenifit(share1.CostValue, p.StockPrice) > 0.05);
                if (stock == null) continue;
                lock (Slocker)
                {
                    ShareTable.StockSell(stock, share1);
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
