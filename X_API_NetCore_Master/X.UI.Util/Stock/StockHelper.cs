using System;
using System.Collections.Generic;
using System.Linq;
using X.Util.Core.Common;
using X.Util.Other;

namespace X.UI.Util.Stock
{
    public class StockHelper
    {
        private static readonly string StockDestPath = "D:\\stock\\股票工具\\Debug\\dest.txt";
        private static readonly string StockDestPath2 = "D:\\stock\\股票工具\\Debug\\src\\dp\\涨停.txt";

        public static Dictionary<string, string> GetStockList()
        {
            var ret = new Dictionary<string, string>();
            var content = FileBase.ReadFile(StockDestPath, "utf-8");
            if (!string.IsNullOrEmpty(content))
            {
                var list = content.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (list != null && list.Length > 0)
                {
                    foreach (var item in list)
                    {
                        var items = item.Split(' ');
                        if (!ret.ContainsKey(items[0]))
                        {
                            ret[items[0]] = items[1];
                        }
                    }
                }
            }
            return ret;
        }

        public static string GetRandomStock()
        {
            var dic = GetStockList().ToList();
            var index = StringConvert.SysRandom.Next(0, dic.Count - 1);
            return dic[index].Value;
        }

        public static Dictionary<string, string> GetStockList2()
        {
            var ret = new Dictionary<string, string>();
            var content = FileBase.ReadFile(StockDestPath2, "gb2312");
            if (!string.IsNullOrEmpty(content))
            {
                var list = content.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
                if (list != null && list.Length > 0)
                {
                    foreach (var item in list.Skip(1))
                    {
                        var items = item.Split('\t');
                        if (items.Length >= 2 && !ret.ContainsKey(items[0]))
                        {
                            ret[items[0]] = items[1];
                        }
                    }
                }
            }
            return ret;
        }

        public static string GetRandomStock2()
        {
            var dic = GetStockList2().ToList();
            var index = StringConvert.SysRandom.Next(0, dic.Count - 1);
            return dic[index].Value;
        }
    }
}
