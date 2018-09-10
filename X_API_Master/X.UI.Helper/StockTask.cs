using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;
using X.Util.Entities;
using X.Util.Extend.Mongo;
using MongoDB.Driver;
using System;
using System.Globalization;
using MongoDB.Driver.Builders;
using X.Util.Core;

namespace X.UI.Helper
{
    public class StockTask
    {
        public void Task1()
        {
            StockHelper.GetStockList().ForEach(p =>
            {
                MongoDbBase<MongoBaseModel>.Default.SaveMongo(new MongoBaseModel { Id = p }, "Stock", "StockList");
                Console.WriteLine(p);
            });
        }

        public void Task2()
        {
            var ret = MongoDbBase<MongoBaseModel>.Default.Find("Stock", "StockList", Query.Null);
            var test = MongoDbBase<StockPrice>.Default.Find("Stock", "Top", Query.Null, Fields.Null, SortBy.Descending("Datetime"), 1);
            foreach (var item in ret)
            {
                var price = StockHelper.GetStockPrice(item.Id);
                if (price == null || price.Datetime.Date == test.FirstOrDefault().Datetime.Date) return;
                if (price != null && price.Inc > 9.8M)
                {
                    MongoDbBase<StockPrice>.Default.SaveMongo(price, "Stock", "Top");
                    Console.WriteLine(price.ToJson());
                }
            }
        }

        public void HistoryTask()
        {
            StockDataHelper.GetDayData().ForEach(p =>
            {
                MongoDbBase<StockKLine>.Default.SaveMongo(p, "Stock", "History");
                Console.WriteLine(p.ToJson());
            });
        }

        public void Task3()
        {
            var top = MongoDbBase<StockPrice>.Default.Find("Stock", "Top", Query.Null, Fields.Null, SortBy.Descending("Datetime"), 200);
            var firstDatetime = top.FirstOrDefault().Datetime;
            foreach(var item in top)
            {
                if (item.Datetime.Date == firstDatetime.Date)
                {
                    var sm = StockHelper.GetStockMinute(item.StockCode, item.Datetime.Date);
                    MongoDbBase<StockMinute>.Default.SaveMongo(sm, "Stock", "Top_D");
                    Console.WriteLine(sm.ToJson());
                }
            }
        }

        public void Task4()
        {
            var list = StockDataHelper.History_Top_D();
            foreach (var item in list)
            {
                var dt = DateTime.ParseExact(item.Key.Split('-')[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                var sk = StockHelper.GetStockMinute(item.Key, dt, item.Value);
                MongoDbBase<StockMinute>.Default.SaveMongo(sk, "Stock", "History_Top_D");
                Console.WriteLine(sk.ToJson());
            }
        }
    }
}
