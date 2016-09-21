using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X.Stock.DB;
using X.Stock.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Stock.Service
{
    public class CustomerService
    {
        /// <summary>
        /// 更新份额
        /// </summary>
        /// <param name="customerNo"></param>
        public static void UpdateStockShares(string customerNo)
        {
            var list = ShareTable.GetStockShare(customerNo);
            if (list == null || list.Count <= 0) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start compute stockshare", string.Empty);
            var stockIds = new string[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                stockIds[i] = StockService.GetStockId(list[i].StockCode);
            }
            var stocks = StockService.GetStockInfo(stockIds);
            foreach (var item in list)
            {
                if (item.UpdateTime.Hour < 15) continue;
                if (item.UpdateTime.Date == DateTime.Now.Date && item.UpdateTime.Hour >= 17) continue;
                var stock = stocks.First(p => p.StockCode == item.StockCode);
                item.CurrentStockPrice = stock.StockPrice;
                item.UpdateTime = DateTime.Now;
                MongoDbBase<StockShare>.Default.SaveMongo(item, "Stock", "Share", null);
            }
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End compute stockshare", string.Empty);
        }

        /// <summary>
        /// 获取用户资产
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public static AssetInfo GetAssetInfo(string customerNo)
        {
            var customer = CustomerTable.GetCustomerInfo(customerNo);
            var shares = ShareTable.GetStockShare(customerNo);
            return new AssetInfo
            {
                CustomerNo = customer.CustomerNo,
                CustomerName = customer.CustomerName,
                CoinAsset = customer.CoinAsset,
                Shares = shares
            };
        }

        /// <summary>
        /// 获取持仓行情
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<StockInfo> GetStockInfoFromShares(AssetInfo info)
        {
            var list = info.Shares;
            if (list == null || list.Count <= 0) return null;
            var stockIds = new string[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                stockIds[i] = StockService.GetStockId(list[i].StockCode);
            }
            return StockService.GetStockInfo(stockIds);
        }
    }
}
