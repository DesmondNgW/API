using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using X.Business.Entities.Stock;
using X.Util.Other;

namespace X.Business.Helper.Stock
{
    public class StockDataHelper
    {
        #region http 接口数据
        /// <summary>
        /// 股票代码列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStockList()
        {
            var ret = new List<string>();
            var regex = new Regex("<li><a target=\"_blank\" href=\"http://quote.eastmoney.com/(.+?).html\">(.+?)</a></li>");
            var content = HttpRequestBase.GetHttpInfo("http://quote.eastmoney.com/stocklist.html", "gb2312", "application/json", null, string.Empty);
            var list = regex.Matches(content.Content);
            if (list != null)
            {
                foreach (Match Match in list)
                {
                    var target = Match.Groups[1].ToString();
                    if (target.StartsWith("sh60") || target.StartsWith("sz002") || target.StartsWith("sz300"))
                    {
                        ret.Add(target.Substring(2));
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 单个股票行情实时
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static StockPrice GetStockPrice(string code)
        {
            var uri = code.StartsWith("6") ? "http://hq.sinajs.cn/list=sh" + code : "http://hq.sinajs.cn/list=sz" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            if (string.IsNullOrEmpty(ret.Content)) return default(StockPrice);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            if (string.IsNullOrEmpty(content) || arr.Length == 1) return default(StockPrice);
            var result = new StockPrice
            {
                Id = string.Format("{0}-{1}", code, arr[30]),
                StockCode = code,
                StockName = arr[0],
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                Buy1 = decimal.Parse(arr[10]),
                Sell1 = decimal.Parse(arr[20]),
                LastClosePrice = decimal.Parse(arr[2]),
                Datetime = DateTime.Parse(arr[30] + " " + arr[31]),
                Vol = decimal.Parse(arr[8]),
                Amount = decimal.Parse(arr[9]) / 100000000M,
                MY = decimal.Parse(arr[10]),
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice / result.LastClosePrice * 100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice / result.LastClosePrice * 100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice / result.LastClosePrice * 100 - 100;
            return result;
        }

        /// <summary>
        /// 获取指数价格
        /// </summary>
        /// <returns></returns>
        public static StockPrice GetIndexPrice(string code)
        {
            var uri = "http://hq.sinajs.cn/list=" + code;
            var ret = HttpRequestBase.GetHttpInfo(uri, "gb2312", "application/json", null, string.Empty);
            if (string.IsNullOrEmpty(ret.Content)) return default(StockPrice);
            var content = ret.Content.Split('\"')[1];
            var arr = content.Split(',');
            if (string.IsNullOrEmpty(content) || arr.Length == 1) return default(StockPrice);
            var result = new StockPrice
            {
                Id = string.Format("{0}-{1}", code, arr[30]),
                StockCode = code,
                StockName = arr[0],
                CurrentPrice = decimal.Parse(arr[3]),
                MaxPrice = decimal.Parse(arr[4]),
                MinPrice = decimal.Parse(arr[5]),
                OpenPrice = decimal.Parse(arr[1]),
                LastClosePrice = decimal.Parse(arr[2]),
                Datetime = DateTime.Parse(arr[30] + " " + arr[31]),
                Amount = decimal.Parse(arr[9]) / 100000000M,
                MY = decimal.Parse(arr[10]),
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice / result.LastClosePrice * 100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice / result.LastClosePrice * 100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice / result.LastClosePrice * 100 - 100;
            return result;
        }

        /// <summary>
        /// 所有股票行情
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, StockPrice> GetStockPrice(IEnumerable<MyStock> list)
        {
            var ret = new Dictionary<string, StockPrice>();
            foreach (var item in list.Where(item => !ret.ContainsKey(item.Code) && !string.IsNullOrEmpty(item.Code)))
            {
                var sp = GetStockPrice(item.Code);
                if (sp != null)
                {
                    ret[item.Code] = GetStockPrice(item.Code);
                }
            }
            return ret;
        }
        #endregion
    }
}
