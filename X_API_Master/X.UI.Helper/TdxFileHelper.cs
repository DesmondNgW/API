using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using X.UI.Entities;
using X.Util.Other;
using X.Util.Core;
using System.IO;
using X.Util.Extend.Mongo;
using System.Linq;

namespace X.UI.Helper
{
    public class TdxFileHelper
    {
        private const string dir = "D:\\Tdx\\day\\";
        private const string logPath = "tdx.d";

        public static void BatchDay()
        {
            DateTime breakPoint = DateTime.MinValue;
            var log = new FileInfo(logPath);
            if (log.Exists)
            {
                DateTime.TryParse(FileBase.ReadFile(logPath, "utf-8"), out breakPoint);
            }
            DateTime nextBreakPoint = breakPoint;
            var Dir = new DirectoryInfo(dir);
            if (Dir.Exists)
            {
                var files = Dir.GetFiles();
                foreach(var file in files)
                {
                    var list = GetStockKLineByDay(file.FullName);
                    if (list.Count > 0)
                    {
                        var tmp = list.Max(p => p.Date);
                        if (tmp > nextBreakPoint) nextBreakPoint = tmp; 
                    }
                    foreach (var item in list.Where(p => p.Date > breakPoint))
                    {
                        if ((item.LastClose > 0 && item.Close / item.LastClose >= 1.07M) || (item.LastClose <= 0))
                        {
                            Console.WriteLine("insert mongodb {0}", item.ToJson());
                            MongoDbBase<StockKLine>.Default.SaveMongo(item, "Stock", "Day");
                        }
                    }
                    file.Delete();
                }
            }
            if (nextBreakPoint != breakPoint)
            {
                FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, logPath, nextBreakPoint.ToString("yyyy-MM-dd"), "utf-8", Util.Entities.Enums.FileBaseMode.Create);
            }
        }

        public static List<StockKLine> GetStockKLineByDay(string path)
        {
            var ret = new List<StockKLine>();
            var content = FileBase.ReadFile(path, "gb2312");
            var list = Regex.Split(content, "\r\n", RegexOptions.IgnoreCase);
            var first = list[0].Split(' ');
            var code = first[0];
            var name = first[1];
            var lastClose = default(decimal);
            for (var i = 2; i < list.Length; i++)
            {
                var item = list[i].Split('\t');
                if (item.Length < 5) continue;
                var close = item[4].Convert2Decimal(0);
                var date = DateTime.ParseExact(item[0], "yyyy/MM/dd", CultureInfo.InvariantCulture);
                ret.Add(new StockKLine
                {
                    Id = string.Format("{0}-{1}", code, date.ToString("yyyyMMdd")),
                    StockCode = code,
                    StockName = name,
                    Date = date,
                    Open = item[1].Convert2Decimal(0),
                    High = item[2].Convert2Decimal(0),
                    Low = item[3].Convert2Decimal(0),
                    Close = close,
                    Vol = item[5].Convert2Decimal(0),
                    Amount = item[6].Convert2Decimal(0),
                    LastClose = lastClose
                });
                lastClose = close;
            }
            return ret;
        }



    }
}
