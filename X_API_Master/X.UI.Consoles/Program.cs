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

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {

            //var data = JRJDataHelper.GetTab(new DateTime(2018, 11, 23), 0);

            //var list = JRJDataHelper.GetDataFromMongo(DateTime.Now.AddMonths(-6), new TimeSpan(9, 35, 0));

            var list = JRJDataHelper.GetTab(DateTime.Now.Date, EnumTab.ZT);

            var f1 = list.Where(p => p.StockCode.ToList().Sum(q => int.Parse(q.ToString())) % 5 == 0 && p.PriceLimit > 7);
            var f2 = list.Where(p => p.StockCode.ToList().Skip(3).Sum(q => int.Parse(q.ToString())) % 5 == 0 && p.PriceLimit > 7);
            Console.WriteLine(f1.Select(p => p.StockName).ToJson());
            Console.WriteLine(f2.Select(p => p.StockName).ToJson());


            //JRJDataHelper.DealData(DateTime.Now.AddDays(-10).Date, DateTime.Now.Date);
            //ConsoleHelper.Draw(".", 30).ForEach(p => Console.WriteLine(p));
            //StockMonitor.Monitor();
            //var key = Guid.NewGuid().ToString("N");
            //Console.WriteLine(ThirdPartyTest.CouchBaseTest(key, key));
            //TestCacheClient(1000);

            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);

            //TdxFileHelper.BatchDay();
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