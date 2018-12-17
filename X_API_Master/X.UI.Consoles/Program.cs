﻿using System;
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

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            var dt = (DateTime.Now.DayOfWeek == DayOfWeek.Saturday ?
                DateTime.Now.AddDays(-1) : DateTime.Now.DayOfWeek == DayOfWeek.Sunday ?
                DateTime.Now.AddDays(-2) : DateTime.Now.AddDays(-1)).Date;
            var list = JRJDataHelper.GetTab(dt, EnumTab.ZT);

            var f1 = list.Where(p => p.StockCode.ToList().Sum(q => int.Parse(q.ToString())) % 5 == 0 && p.PriceLimit > 7);
            var f2 = list.Where(p => p.StockCode.ToList().Skip(3).Sum(q => int.Parse(q.ToString())) % 5 == 0 && p.PriceLimit > 7);
            var f3 = list.Sum(p => p.Amount) / 100000000;

            Console.WriteLine(f1.Select(p => p.StockName).ToJson());
            Console.WriteLine(f2.Select(p => p.StockName).ToJson());
            Console.WriteLine(f3.ToString("0.00") + "亿");

            Func<double, double, double> c = (p, r) =>
            {
                double ret = p - (1 - p) / r;
                return ret <= 0 ? 0 : ret;
            };
            List<double> array = new List<double>()
            {
                15.5 /15,
                27.0 /15,
                39.75 /15,
                53.73 /15,
                69.1 /15,
                86.0 /15,
                105.0 /15
            };

            for (double i = 0.45; i <= 1; i += 0.05)
            {
                foreach(double item in array)
                {
                    Console.WriteLine("胜率:{0},盈亏比:{1},仓位:{2}", i, item.ToString("0.00"), c(i, item));
                }
            }

            Console.ReadKey();
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