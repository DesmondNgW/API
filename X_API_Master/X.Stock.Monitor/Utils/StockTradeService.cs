using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using X.Stock.Monitor.Model;
using X.Util.Other;

namespace X.Stock.Monitor.Utils
{
    public class StockTradeService
    {
        public static StockInfo GetStockInfo(string code)
        {
            var result = new StockInfo();
            const string uri = "http://nuff.eastmoney.com/EM_Finance2015TradeInterface/JS.ashx?id={0}&token=beb0a0047196124721f56b0f0ff5a27c";
            var id = code + (code.StartsWith("6") ? "1" : "2");
            var iresult = HttpRequestBase.GetHttpInfo(string.Format(uri, id), "utf-8", "application/json", null,
                string.Empty);
            var reg = new Regex("\\\"(.+?)\\\"");
            var groups = reg.Matches(iresult.Content);
            if (groups.Count <= 0) return result;
            result.StockCode = groups[3].Groups[1].Value;
            result.StockName = groups[4].Groups[1].Value;
            result.StockPrice = int.Parse(groups[27].Groups[1].Value);
            result.StockKm1 = int.Parse(groups[29].Groups[1].Value);
            result.StockKm2 = int.Parse(groups[31].Groups[1].Value);
            return result;
        }

        public static void AddStockShare(StockInfo stock, CustomerInfo info)
        {
        }
    }
}
