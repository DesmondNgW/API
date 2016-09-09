using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using X.Stock.Monitor.Model;
using X.Util.Extend.Mongo;

namespace X.Stock.Monitor.Utils
{
    public class StockPoolService
    {
        public static string StockPoolFile = "stockpool.txt";

        public static void ImportStockPool(string encode)
        {
            if (!File.Exists(StockPoolFile)) return;
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
        }

        public static List<StockPool> GetStockPool()
        {
            var result = new List<StockPool>();
            var query = new QueryDocument();
            var list = MongoDbBase<StockPool>.Default.ReadMongo("Stock", "Pool", null, query).OrderByDescending(p => p.CreateTime).ToList();
            if (list.Count <= 0) return result;
            var n = list.First().CreateTime;
            return list.Where(p => p.CreateTime == n).ToList();
        }
    }
}
