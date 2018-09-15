using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using X.UI.Entities;
using X.Util.Extend.Mongo;

namespace X.UI.Helper
{
    public class StockHelper
    {
        public static StockKLineExtend GetStockMinute(StockKLineCombine data)
        {
            var ret = new StockKLineExtend();
            if (data != null)
            {
                ret.Id = data.Id;
                if (data.Day != null)
                {
                    ret.StockCode = data.Day.StockCode;
                    ret.StockName = data.Day.StockName;
                    ret.Date = data.Day.Date;
                    ret.High = data.Day.High;
                    ret.Low = data.Day.Low;
                    ret.Open = data.Day.Open;
                    ret.Close = data.Day.Close;
                    ret.LastClose = data.Day.LastClose;
                    ret.Vol = data.Day.Vol;
                    ret.Amount = data.Day.Amount;
                    ret.Hsl = data.Day.Hsl;
                    ret.Inc = ret.Close / ret.LastClose * 100 - 100;
                }
                if (data.Minutes != null && data.Minutes.Count > 0)
                {
                    ret.Amount = data.Minutes.Sum(p => p.Amount);
                    ret.Vol = data.Minutes.Sum(p => p.Vol);
                    var high = data.Minutes.Max(p => p.High);
                    var _ = data.Minutes.Where(p => p.Close >= high && p.High > p.Low).OrderBy(p => p.Time).ToList();
                    if (_.Count > 0)
                    {
                        ret.FirstTime = _.FirstOrDefault().Time;
                        ret.LastTime = _.LastOrDefault().Time;
                    }
                    foreach (var minute in data.Minutes)
                    {
                        if (string.IsNullOrEmpty(ret.StockCode))
                            ret.StockCode = minute.StockCode;
                        if (string.IsNullOrEmpty(ret.StockName))
                            ret.StockName = minute.StockName;
                        if (minute.Close == high && minute.High > minute.Low)
                        {
                            ret.CountOfHighPrice++;
                            ret.AmountOfHighPrice += minute.Amount;
                            ret.VolOfHighPrice += minute.Vol;
                        }
                        if (minute.Close == high && minute.High == minute.Low)
                        {
                            ret.CountOfOnePrice++;
                            ret.AmountOfOnePrice += minute.Amount;
                            ret.VolOfOnePrice += minute.Vol;
                        }
                    }
                    ret.AHP = ret.AmountOfHighPrice / ret.Amount * 100;
                    ret.VHP = ret.VolOfHighPrice / ret.Vol * 100;
                    ret.AOP = ret.AmountOfOnePrice / ret.Amount * 100;
                    ret.VOP = ret.VolOfOnePrice / ret.Vol * 100;
                }
            }
            return ret;
        }

        public static List<StockKLineExtend> GetDataFromMongo()
        {
            return MongoDbBase<StockKLineExtend>.Default.Find("Stock", "Data", Query.Null, Fields.Null, SortBy.Descending("Date", "Inc", "Amount")).ToList();
        }

        public static void DealData()
        {
            Console.WriteLine("query db start.");
            var list = StockDataHelper.StockKLineCombine();
            Console.WriteLine("query db end and insert mongodb start.");
            var i = 0;
            foreach (var item in list)
            {
                var sk = GetStockMinute(item.Value);
                MongoDbBase<StockKLineExtend>.Default.SaveMongo(sk, "Stock", "Data");
                Console.WriteLine("DealDone Code {1}({0}), save done mongodb", sk.StockCode, sk.StockName);
                i++;
            }
            Console.WriteLine("insert mongodb end! deal {0} done!", i);
        }
    }
}