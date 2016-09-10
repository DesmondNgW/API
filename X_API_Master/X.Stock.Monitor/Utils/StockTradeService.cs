using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X.Stock.Monitor.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Stock.Monitor.Utils
{
    public class StockTradeService
    {
        public static void BuyStock(List<StockInfo> stocks, AssetInfo info)
        {
            var count = info.Shares != null ? info.Shares.Count : 0;
            if (count >= 4) return;
            var total = info.CoinAsset/(4 - count);
            var target = stocks.Where(p => p.StockKm2 >= 3.3M && p.StockKm2 <= 8.8M).OrderByDescending(p => p.StockKm2).FirstOrDefault();
            if (target == null) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start Buy stock", string.Empty);
            var vol = Math.Floor(total/target.StockPrice/100)*100;
            var amount = vol*target.StockPrice;
            MongoDbBase<StockShare>.Default.SaveMongo(new StockShare
            {
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
            CustomerService.UpdateCustomerInfo(info.CustomerNo, info.CoinAsset - amount);
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End Buy stock", string.Empty);
        }

        public static void SellStock(List<StockInfo> stocks, AssetInfo info)
        {
            var shares = info.Shares;
            if (shares == null || shares.Count(p => p.CreateTime != DateTime.Now.Date) == 0) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start Sell stock", string.Empty);
            foreach (var share in shares.Where(p => p.CreateTime != DateTime.Now.Date))
            {
                var share1 = share;
                var stock = stocks.FirstOrDefault(p => p.StockCode == share1.StockCode && p.StockKm2 <= -3.3M && StockService.GetBenifit(share1.CostValue, p.StockPrice) > 0.05M);
                if (stock == null) continue;
                share1.CurrentStockPrice = stock.StockPrice;
                share1.TotalVol = 0;
                share1.AvailableVol = 0;
                var amount = stock.StockPrice*share1.TotalVol;
                MongoDbBase<StockShare>.Default.SaveMongo(share1, "Stock", "Share", null);
                CustomerService.UpdateCustomerInfo(info.CustomerNo, info.CoinAsset + amount);
            }
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End Sell stock", string.Empty);
        }

        public static bool IsCanTrade(DateTime now)
        {
            var amBegin = now.Date + new TimeSpan(9, 30, 0);
            var amEnd = now.Date + new TimeSpan(11, 30, 0);
            var pmBegin = now.Date + new TimeSpan(13, 0, 0);
            var pmEnd = now.Date + new TimeSpan(15, 0, 0);
            if (now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday && now >= amBegin && now <= pmEnd)
            {
                return now <= amEnd || now >= pmBegin;
            }
            return false;
        }

        public static bool IsCanTrade()
        {
            if (!IsCanTrade(DateTime.Now)) return false;
            var result = StockService.GetStockInfo("3000592");
            return result != null && result.Now.Date == DateTime.Now.Date;
        }
    }
}
