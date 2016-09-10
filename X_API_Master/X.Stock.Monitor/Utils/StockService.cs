using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using X.Stock.Monitor.Model;
using X.Util.Other;

namespace X.Stock.Monitor.Utils
{
    public class StockService
    {
        public static string GetStockId(string stockCode)
        {
            return stockCode + (stockCode.StartsWith("6") ? "1" : "2");
        }

        public static string[] GetStockId(string[] stockCodes)
        {
            if (stockCodes == null || stockCodes.Length <= 0) return null;
            var result = new string[stockCodes.Length];
            for (var i = 0; i < stockCodes.Length; i++)
            {
                result[i] = GetStockId(stockCodes[i]);
            }
            return result;
        }

        /// <summary>
        /// 获取行情
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public static StockInfo GetStockInfo(string stockId)
        {
            var result = new StockInfo();
            const string uri = "http://nuff.eastmoney.com/EM_Finance2015TradeInterface/JS.ashx?id={0}&token=beb0a0047196124721f56b0f0ff5a27c";
            var iresult = HttpRequestBase.GetHttpInfo(string.Format(uri, stockId), "utf-8", "application/json", null,
                string.Empty);
            var reg = new Regex("\\\"(.+?)\\\"");
            var groups = reg.Matches(iresult.Content);
            if (groups.Count <= 0) return result;
            result.StockCode = groups[3].Groups[1].Value;
            result.StockName = groups[4].Groups[1].Value;
            result.StockPrice = int.Parse(groups[27].Groups[1].Value);
            result.StockKm1 = int.Parse(groups[29].Groups[1].Value);
            result.StockKm2 = int.Parse(groups[31].Groups[1].Value);
            result.Now = DateTime.Parse(groups[51].Groups[1].Value);
            return result;
        }

        /// <summary>
        /// 获取行情
        /// </summary>
        /// <param name="stockIds"></param>
        /// <returns></returns>
        public static List<StockInfo> GetStockInfo(string[] stockIds)
        {
            var result = new List<StockInfo>();
            if (stockIds == null) return result;
            const string uri = "http://nufm.dfcfw.com/EM_Finance2014NumericApplication/JS.aspx?ps=500&token=580f8e855db9d9de7cb332b3990b61a3&type=CT&cmd={0}&sty=CTALL";
            var iresult = HttpRequestBase.GetHttpInfo(string.Format(uri, string.Join(",", stockIds)), "utf-8", "application/json", null,
                string.Empty);
            var reg = new Regex("\\\"(.+?)\\\"");
            var groups = reg.Matches(iresult.Content);
            if (groups.Count > 0)
            {
                result.AddRange(from Match @group in groups
                    select @group.Groups[1].ToString().Split(',')
                    into array
                    select new StockInfo()
                    {
                        StockCode = array[1],
                        StockName = array[2],
                        StockPrice = int.Parse(array[3]),
                        StockKm2 = int.Parse(array[4].Replace("%", string.Empty)),
                        StockKm1 = int.Parse(array[5])
                    });
            }
            return result;
        }

        public static decimal GetBenifit(decimal cost, decimal current)
        {
            return current/cost - 1;
        }
    }
}
