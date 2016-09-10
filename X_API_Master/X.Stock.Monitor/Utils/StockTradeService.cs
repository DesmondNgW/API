using System;
using System.Collections.Generic;
using System.Linq;
using X.Stock.Monitor.Model;
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
                CreateTime = DateTime.Now.Date
            }, "Stock", "Share", null);
            CustomerService.UpdateCustomerInfo(info.CustomerNo, info.CoinAsset - amount);
        }

        public static void SellStock(List<StockInfo> stocks, List<StockShare> shares, AssetInfo info)
        {
            if (shares == null || shares.Count == 0) return;
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
        }
    }
}
