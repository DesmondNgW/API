using System;
using System.Collections.Generic;
using System.Linq;
using X.DataBase.Helper;

namespace X.UI.Consoles.Stock
{
    public class StockDataHelper
    {
        private static readonly DbHelper DbHelper = DalHelper.DbHelper;

        public static double Std(List<Stock> list)
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
                result[i].Inc = i == 0 ? 0 : (result[i].Close - result[i - 1].Close)/result[i - 1].Close;
                result[i].Ev = i < 4 ? 0 : result.Skip(i - 4).Take(5).Average(p => p.Close);
                result[i].Std = i < 4 ? 0 : Std(result.Skip(i - 4).Take(5).ToList());
                result[i].Ze = result.Take(i + 1).Average(p => p.ZScore);
                result[i].Cve = result.Take(i + 1).Average(p => p.CoefficientVariation);
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
