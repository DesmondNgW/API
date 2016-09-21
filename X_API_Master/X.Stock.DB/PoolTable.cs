using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Builders;
using X.Stock.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.DB
{
    public class PoolTable
    {
        /// <summary>
        /// 导入数据库
        /// </summary>
        /// <param name="pool"></param>
        public static void ImportStockPool(List<StockPool> pool)
        {
            MongoDbBase<StockPool>.Default.InsertBatchMongo(pool, "Stock", "Pool", null);
        }

        /// <summary>
        /// 获取最新的一期StockPool
        /// </summary>
        /// <returns></returns>
        public static List<StockPool> GetStockPool()
        {
            var result = new List<StockPool>();
            var query = Query.GTE("CreateTime", DateTime.Now.AddMonths(-1));
            var sortBy = SortBy.Descending("CreateTime");
            var list = MongoDbBase<StockPool>.Default.Find("Stock", "Pool", null, query, null, sortBy).ToList();
            if (list.Count <= 0) return result;
            var n = list.First().CreateTime;
            return list.Where(p => p.CreateTime == n).ToList();
        }
    }
}
