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

        public static double GetAtr(Stock current, Stock prevent)
        {
            //var ret = Math.Max(current.High - current.Low, Math.Abs(prevent.Close - current.High));
            //ret = Math.Max(ret, Math.Abs(prevent.Close - current.Low));
            return (current.High - current.Low) / prevent.Close;
        }

        public static List<Stock> StockData(string code)
        {
            var result = new List<Stock>();
            var ret = DbHelper.GetPageList("tradedata_tdx", "tdate desc", "scode='" + code + "'", 0, 500);
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
                result[i].Position = (result[i].Close - result[i].Open) / (result[i].High - result[i].Low);
                result[i].Strong = i == 0 ? 0 : GetStrong(result[i], result[i - 1]);
                result[i].Atr = i == 0 ? 0 : GetAtr(result[i], result[i - 1]);
                if (i == 0)
                {
                    result[i].PositionLength = result[i].Position >= 0.8 ? 1 : 0;
                    result[i].StrongLength = result[i].Strong >= 1.3 ? 1 : 0;
                    result[i].IncLength = result[i].Inc >= 0.095 ? 1 : 0;
                }
                else
                {
                    result[i].PositionLength = result[i].Position >= 0.8 ? 1 + result[i - 1].PositionLength : 0;
                    result[i].StrongLength = result[i].Strong >= 1.3 ? 1 + result[i - 1].StrongLength : 0;
                    result[i].IncLength = result[i].Inc >= 0.095 ? 1 + result[i - 1].IncLength : 0;
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
