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
    }
}
