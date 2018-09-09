using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.UI.Entities;
using X.Util.Entities;
using X.Util.Extend.Mongo;
using MongoDB.Driver;

namespace X.UI.Helper
{
    public class StockTask
    {
        public static void Task1()
        {
            var list = StockHelper.GetStockList();
            MongoDbBase<MongoBaseModel>.Default.InsertBatchMongo(list.Select(p => new MongoBaseModel { Id = p }), "Stock", "StockList");
        }

        public static void Task2()
        {
            var list = new List<StockPrice>();
            var ret = MongoDbBase<MongoBaseModel>.Default.Find("Stock", "StockList", null);
            var sort = new SortByDocument
            {
                { "Datetime", -1 }
            };
            var test = MongoDbBase<StockPrice>.Default.Find("Stock", "Top", null, null, sort, 1);
            foreach (var item in ret)
            {
                var price = StockHelper.GetStockPrice(item.Id);
                if (price.Datetime.Date == test.FirstOrDefault().Datetime.Date) return;
                if (price != null && price.Inc > 9.8M && !list.Exists(p => p.StockCode == price.StockCode))
                {
                    list.Add(price);
                }
            }
            MongoDbBase<StockPrice>.Default.InsertBatchMongo(list, "Stock", "Top");
        }
    }
}
