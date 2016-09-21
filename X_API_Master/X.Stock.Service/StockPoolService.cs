using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using X.Stock.DB;
using X.Stock.Model;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Stock.Service
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
            PoolTable.ImportStockPool(stockPool);
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

        public static List<StockInfo> GetStockInfoFromPool()
        {
            var iscantrade = StockTradeService.IsCanTrade();
            var result = HttpRuntime.Cache.Get("GetStockInfoFromPool") as List<StockInfo>;
            if (result != null && iscantrade) return result;
            var pool = PoolTable.GetStockPool();
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
