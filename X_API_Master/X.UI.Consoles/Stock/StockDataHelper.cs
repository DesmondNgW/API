using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.DataBase.Helper;

namespace X.UI.Consoles.Stock
{
    public class StockDataHelper
    {
        private static readonly DbHelper DbHelper = DalHelper.DbHelper;

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
                    Y = new Dictionary<int, double>()
                };
                result.Add(item);
            }
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Inc = i == 0 ? 0 : (result[i].Close - result[i - 1].Close)/result[i - 1].Close;
                for (var j = 1; j <= 20; j++)
                {
                    if (i + j < result.Count)
                    {
                        result[i].Y[j] = (result[i + j].Close - result[i].Close) / result[i].Close;
                    }
                }
                for (var j = 1; j <= 20; j++)
                {
                    if (!result[i].Y.ContainsKey(j))
                    {
                        result[i].Y[j] = 0;
                    }
                }
            }
            return result.OrderBy(p=>p.Date).ToList();
        }
    }
}
