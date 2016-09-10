using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MongoDB.Driver;
using X.Stock.Monitor.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Stock.Monitor.Utils
{
    public class StockPoolService
    {
        public static string StockPoolFile = "stockpool.txt";

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="encode"></param>
        public static void ImportStockPool(string encode)
        {
            if (!File.Exists(StockPoolFile)) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start import stockpool", string.Empty);
            var ed = encode.Contains("utf8") || encode.Contains("utf-8") ? Encoding.UTF8 : encode.Contains("unicode") ? Encoding.Unicode : Encoding.GetEncoding(encode);
            var sr = new StreamReader(StockPoolFile, ed);
            string line;
            var stockPool = new List<StockPool>();
            while ((line = sr.ReadLine()) != null)
            {
                var array = line.Split('\t');
                stockPool.Add(new StockPool
                {
                    StockCode = array[0],
                    StockName = array[1],
                    CreateTime = DateTime.Now.Date
                });
            }
            MongoDbBase<StockPool>.Default.InsertBatchMongo(stockPool, "Stock", "Pool", null);
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End import stockpool", string.Empty);

        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public static List<StockPool> GetStockPool()
        {
            var result = new List<StockPool>();
            var query = new QueryDocument();
            var list = MongoDbBase<StockPool>.Default.ReadMongo("Stock", "Pool", null, query).OrderByDescending(p => p.CreateTime).ToList();
            if (list.Count <= 0) return result;
            var n = list.First().CreateTime;
            return list.Where(p => p.CreateTime == n).ToList();
        }

        public static List<StockInfo> GetStockInfoFromPool()
        {
            var pool = GetStockPool();
            if (pool == null || pool.Count <= 0) return null;
            var stockIds = new string[pool.Count];
            for (var i = 0; i < pool.Count; i++)
            {
                stockIds[i] = StockService.GetStockId(pool[i].StockCode);
            }
            return StockService.GetStockInfo(stockIds);
        }
    }
}
