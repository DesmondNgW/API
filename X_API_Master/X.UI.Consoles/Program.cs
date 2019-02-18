using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using X.UI.Helper;
using X.Util.Core;
using X.Util.Extend;
using X.Util.Core.Kernel;
using X.Util.Other;
using X.Util.Extend.Mongo;
using X.Util.Entities;
using X.Util.Provider;
using X.Util.Entities.Enums;
using MongoDB.Driver.Builders;
using System.Globalization;
using X.UI.Entities;
using System.IO;
using System.Threading;

namespace X.UI.Consoles
{
    class Program
    {
        static DateTime GetWorkDay(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Saturday)
            {
                return dt.AddDays(-1).Date;
            }
            else if (dt.DayOfWeek == DayOfWeek.Sunday)
            {
                return dt.AddDays(-2).Date;
            }
            else if (dt.DayOfWeek == DayOfWeek.Monday && dt.Hour < 15)
            {
                return dt.AddDays(-3).Date;
            }
            else if (dt.Hour < 15)
            {
                return dt.AddDays(-1).Date;
            }
            return dt.Date;
        }

        static void Main()
        {
            var list = JRJDataHelper.GetDataFromMongo(DateTime.Now.AddMonths(-1), new TimeSpan(15, 0, 0)).OrderByDescending(p => p.DateTime);
            var dt = list.FirstOrDefault().DateTime.Date;
            Console.BackgroundColor = ConsoleColor.White;
            while (true)
            {
                var last = DateTime.Now;
                var ret = new List<StockPrice>();
                foreach (var item in list.Where(p => p.DateTime.Date == dt && p.PriceLimit > 7))
                {
                    var price = StockDataHelper.GetStockPrice(item.StockCode);
                    ret.Add(price);
                    last = price.Datetime;
                }
                int count = 1;
                foreach (var item in ret.OrderByDescending(p => p.Inc))
                {
                    if (item.Inc > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    var fb = item.MY * item.CurrentPrice / 100000000M;
                    Console.WriteLine("{6}:{0}({1})：当前价格{2}，涨幅{3}%,成交额{4}亿, 封板金额{5}亿",
                        item.StockName,
                        item.StockCode,
                        item.CurrentPrice,
                        item.Inc.ToString("0.00"),
                        item.Amount.ToString("0.00"),
                        item.Inc > 9.6M ? fb.ToString("0.00") : "0.00",
                        count);
                    count++;
                }
                if ((DateTime.Now - last).TotalMinutes > 10)
                {
                    Console.WriteLine("时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "行情暂停");
                    if (DateTime.Now.Hour < 9 || DateTime.Now.Hour >= 15)
                    {
                        Thread.Sleep(1000 * 60 * 15);
                    }
                    else if (DateTime.Now.Minute == 29 || DateTime.Now.Minute == 59 || DateTime.Now.Minute == 14)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread.Sleep(1000 * 60);
                    }
                }
                else
                {
                    Console.WriteLine("时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    Thread.Sleep(1000);
                }
            }
        }

        static void Index()
        {
            Console.Clear();
            Console.WriteLine(@"┏━━━━━━━━━━━━━━━┓");
            Console.WriteLine(@"┃	API-Console-Test:	┃");
            foreach (var item in (EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem)))
            {
                Console.WriteLine(@"┣━━━━━━━━━━━━━━━┫");
                Console.WriteLine(@"┃{0}.{1}┃", item.GetHashCode(), item.ToString().PadRight(30));
            }
            Console.WriteLine(@"┗━━━━━━━━━━━━━━━┛");
            Next();
        }

        static void Next()
        {
            var input = Console.ReadLine();
            foreach (var item in ((EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem))).Where(item => Equals(item.GetHashCode().ToString(), input)))
            {
                Console.Clear();
                var methodInfo = typeof(ApiTestMethods).GetMethod(item.ToString());
                if (methodInfo != null)
                    methodInfo.Invoke(null, null);
                Index();
                break;
            }
        }
    }
}