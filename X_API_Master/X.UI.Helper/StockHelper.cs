
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using X.UI.Entities;
using X.Util.Core;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Helper
{
    public class StockHelper
    {
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
                LastClosePrice = decimal.Parse(arr[2]),
                Datetime = DateTime.Parse(arr[30]),
                Indexs = new List<string>()
            };
            if (result.CurrentPrice != 0) result.Inc = result.CurrentPrice / result.LastClosePrice * 100 - 100;
            if (result.MaxPrice != 0) result.MaxInc = result.MaxPrice / result.LastClosePrice * 100 - 100;
            if (result.MinPrice != 0) result.MinInc = result.MinPrice / result.LastClosePrice * 100 - 100;
            return result;
        }

        public static Dictionary<string, StockPrice> GetStockPrice()
        {
            var ret = new Dictionary<string, StockPrice>();
            var list = GetStockList();
            foreach (var code in list.Where(code => !ret.ContainsKey(code) && !string.IsNullOrEmpty(code)))
            {
                var sp = GetStockPrice(code);
                if (sp != null)
                {
                    ret[code] = GetStockPrice(code);
                }
            }
            return ret;
        }

        public static void Down()
        {
            var dic = GetStockPrice();
            var content = dic.Where(p => p.Value != null && p.Value.Inc > 9.8M).Select(p=>p.Value).ToJson();
            FileBase.WriteFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "d"), DateTime.Now.ToString("yyyyMMdd"), content, "utf-8", FileBaseMode.Create);
        }

        public static List<StockPrice> Load(DateTime dt)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "d", dt.ToString("yyyyMMdd"));
            var ret = FileBase.ReadJson<List<StockPrice>>(path, "utf-8");
            return ret;
        }
    }
}