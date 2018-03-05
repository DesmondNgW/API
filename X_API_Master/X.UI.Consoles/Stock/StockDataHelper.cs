using System;
using System.Collections.Generic;
using System.Linq;
using X.DataBase.Helper;

namespace X.UI.Consoles.Stock
{
    public class StockDataHelper
    {
        private static readonly DbHelper DbHelper = DalHelper.DbHelper;

        public static double GetStrong(Stock current, Stock prevent)
        {
            var ret = current.High/prevent.High;
            ret *= current.Open/prevent.Close;
            ret *= current.Close / prevent.Close;
            ret *= 2*current.Close / (current.High + current.Low);
            ret *= current.Close / current.Open;
            ret *= current.Low / prevent.Low;
            ret *= current.Close / prevent.High;
            return ret;
        }

        public static double GetComputePrice(List<Stock> list)
        {
            var data = list.OrderBy(p => p.Date);
            var ma = data.Skip(1).Average(p => p.Close);
            var roc = (data.Last().Close - data.First().Close)/(list.Count - 1);
            return ma + roc;
        }

        public static double std(List<Stock> list)
        {
            var ma = list.Average(p => p.Close);
            var std = Math.Pow(list.Sum(p => Math.Pow(p.Close - ma, 2))/list.Count, 0.5);
            return std;
        }

        public static List<Stock> StockData(string code)
        {
            var result = new List<Stock>();
            var ret = DbHelper.GetPageList("tradedata_tdx", "tdate desc", "scode='" + code + "'", 0, 10000);
            while (ret.Read())
            {
                var item = new Stock
                {
                    StockCode = DalHelper.GetString(ret["Scode"]),
                    StockName = DalHelper.GetString(ret["Sname"]),
                    Date = DalHelper.GetDateTime(ret["Tdate"]),
                    Open = DalHelper.GetDouble(ret["OpenPrice"], 0),
                    High = DalHelper.GetDouble(ret["HighPrice"], 0),
                    Close = DalHelper.GetDouble(ret["ClosePrice"], 0),
                    Low = DalHelper.GetDouble(ret["LowPrice"], 0),
                    StockSimple = new Dictionary<int, StockSimple>()
                };
                result.Add(item);
            }
            result = result.OrderBy(p => p.Date).ToList();
            for (var i = 0; i < result.Count; i++)
            {
                result[i].ComputePrice = i < 6 ? 0 : GetComputePrice(result.Skip(i - 6).Take(6).ToList());
                result[i].ComputePrice2 = i < 11 ? 0 : GetComputePrice(result.Skip(i - 11).Take(11).ToList());
                result[i].Inc = i == 0 ? 0 : (result[i].Close - result[i - 1].Close)/result[i - 1].Close;
                result[i].Strong = i == 0 ? 0 : GetStrong(result[i], result[i - 1]);
                result[i].Ma = i < 4 ? 0 : result.Skip(i - 4).Take(5).Average(p => p.Close);
                result[i].Std = i < 4 ? 0 : std(result.Skip(i - 4).Take(5).ToList());
                if (result[i].Std > 0)
                {
                    result[i].K = (result[i].Close - result[i].Ma)/result[i].Std;
                }
                if (result[i].Ma > 0)
                {
                    result[i].Cv = result[i].Std / result[i].Ma;
                }
                if (i < 2)
                {
                    result[i].StrongLength = 0;
                }
                else if (i == 2)
                {
                    result[i].StrongLength = result[i].Strong * result[i - 1].Strong * result[i - 2].Strong >= 1 ? 1 : 0;
                }
                else
                {
                    result[i].StrongLength = result[i].Strong * result[i - 1].Strong * result[i - 2].Strong >= 1 ? 1 + result[i - 1].StrongLength : 0;
                }
                for (var j = 1; j <= 20; j++)
                {
                    if (i + j < result.Count)
                    {
                        result[i].StockSimple[j] = new StockSimple
                        {
                            Low = result[i + j].Low,
                            High = result[i + j].High,
                            Open = result[i + j].Open,
                            Close = result[i + j].Close,
                            StockCode = result[i + j].StockCode,
                            StockName = result[i + j].StockName,
                            Date = result[i + j].Date ?? DateTime.MinValue
                        };
                    }
                }
                for (var j = 1; j <= 20; j++)
                {
                    if (!result[i].StockSimple.ContainsKey(j))
                    {
                        result[i].StockSimple[j] = default(StockSimple);
                    }
                }
            }
            return result.OrderBy(p=>p.Date).ToList();
        }
    }
}
