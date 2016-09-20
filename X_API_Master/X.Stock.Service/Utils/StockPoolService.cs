using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using MongoDB.Driver.Builders;
using X.Stock.Service.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Mongo;

namespace X.Stock.Service.Utils
{
    public class StockPoolService
    {
        public static string StockPoolDir = "stockpool";

        public static void ImportStockPool(string encode, string path)
        {
            if (!File.Exists(path)) return;
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "Start import stockpool", string.Empty);
            var ed = encode.Contains("utf8") || encode.Contains("utf-8") ? Encoding.UTF8 : encode.Contains("unicode") ? Encoding.Unicode : Encoding.GetEncoding(encode);
            var sr = new StreamReader(path, ed);
            string line;
            var stockPool = new List<StockPool>();
            while ((line = sr.ReadLine()) != null)
            {
                var array = line.Split('\t');
                if (array.Length <= 2 || array[0].Length != 6) continue;
                stockPool.Add(new StockPool
                {
                    StockCode = array[0],
                    StockName = array[1],
                    CreateTime = DateTime.Now.Date
                });
            }
            sr.Close();
            MongoDbBase<StockPool>.Default.InsertBatchMongo(stockPool, "Stock", "Pool", null);
            File.Delete(path);
            Logger.Client.Info(MethodBase.GetCurrentMethod(), LogDomain.Core, "End import stockpool", string.Empty);
        }



        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="encode"></param>
        public static void ImportStockPool(string encode)
        {
            if (!Directory.Exists(StockPoolDir)) Directory.CreateDirectory(StockPoolDir);
            var dir = new DirectoryInfo(StockPoolDir);
            var files = dir.GetFiles();
            if (files.Length <= 0) return;
            foreach (var file in files)
            {
                ImportStockPool(encode, file.FullName);
            }
        }

        /// <summary>
        /// 获取
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

        public static List<StockInfo> GetStockInfoFromPool()
        {
            var iscantrade = StockTradeService.IsCanTrade();
            var result = HttpRuntime.Cache.Get("GetStockInfoFromPool") as List<StockInfo>;
            if (result != null && iscantrade) return result;
            var pool = GetStockPool();
            if (pool == null || pool.Count <= 0) return null;
            var stockIds = new string[pool.Count];
            for (var i = 0; i < pool.Count; i++)
            {
                stockIds[i] = StockService.GetStockId(pool[i].StockCode);
            }
            result = StockService.GetStockInfo(stockIds);
            HttpRuntime.Cache.Insert("GetStockInfoFromPool", result, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            return result;
        }
    }
}
