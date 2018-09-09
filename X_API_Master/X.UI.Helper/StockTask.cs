using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;
using X.Util.Entities;
using X.Util.Extend.Mongo;
using MongoDB.Driver;

namespace X.UI.Helper
{
    public class StockTask
    {
        public void Task1()
        {
            var list = StockHelper.GetStockList();
            MongoDbBase<MongoBaseModel>.Default.InsertBatchMongo(list.Select(p => new MongoBaseModel { Id = p }), "Stock", "StockList");

        }

        public void Task2()
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

        public void HistoryTask()
        {
            var list = StockDataHelper.GetDayData();
            MongoDbBase<StockKLine>.Default.InsertBatchMongo(list, "Stock", "History");
        }

        public void HistoryTopDTask()
        {
            var sort = new SortByDocument
            {
                { "Datetime", -1 }
            };
            var top = MongoDbBase<StockKLine>.Default.Find("Stock", "History", null, null, sort, 10000);
            var firstDatetime = top.FirstOrDefault().Date;
            var StockMinutes = new List<StockMinute>();
            foreach (var item in top)
            {
               var t = StockHelper.GetStockMinute(item.StockCode, item.Date.Date);
               StockMinutes.Add(t);
               MongoDbBase<StockMinute>.Default.InsertMongo(t, "Stock", "History_D");
            }
            //if (StockMinutes.Count > 0)
            //{
            //    MongoDbBase<StockMinute>.Default.InsertBatchMongo(StockMinutes, "Stock", "History_D");
            //}
        }

        public void Task3()
        {
            var sort = new SortByDocument
            {
                { "Datetime", -1 }
            };
            var top = MongoDbBase<StockPrice>.Default.Find("Stock", "Top", null, null, sort, 200);
            var firstDatetime = top.FirstOrDefault().Datetime;
            var StockMinutes = new List<StockMinute>();
            foreach(var item in top)
            {
                if (item.Datetime.Date == firstDatetime.Date)
                {
                    StockMinutes.Add(StockHelper.GetStockMinute(item.StockCode, item.Datetime.Date));
                }
            }
            if (StockMinutes.Count > 0)
            {
                MongoDbBase<StockMinute>.Default.InsertBatchMongo(StockMinutes, "Stock", "Top_D");
            }
        }
    }
}
