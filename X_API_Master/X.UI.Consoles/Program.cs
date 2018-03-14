using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Xml;
using MongoDB.Bson;
using X.DataBase.Helper;
using X.UI.Consoles.Stock;
using X.Util.Core.Xml;
using X.Util.Entities.Enums;
using X.Util.Other;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            //StockTestHelper.Test("300666", stock =>
            //{
            //    return stock.StrongLength > 5;
            //});

            //var list = StockDataHelper.StockData("600903", true);
            //FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv", string.Empty, "utf-8", FileBaseMode.Create);
            //foreach (var item in list)
            //{
            //    FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv",
            //        string.Format("{0},{1},{2},{3},{4}", item.ZScore, item.Ze,
            //            item.CoefficientVariation, item.Inc, item.Cve),
            //        "utf-8", FileBaseMode.Append);
            //}

            //TestCacheClient(1000);

            //MongoDbBase<MongoTest1>.Default.SaveMongo(new MongoTest1 { Dt = DateTime.Now, Value = 1, Key = "test" }, "Test", "test");
            //MongoDbBase<MongoTest2>.Default.SaveMongo(new MongoTest2 { Dt = DateTime.Now, Value = "15", Key = "test" }, "Test", "test");
            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);
            Console.ReadKey();
        }

        static void Index()
        {
            Console.Clear();
            Console.WriteLine("┏━━━━━━━━━━━━━━━┓");
            Console.WriteLine("┃\tAPI-Console-Test:\t┃");
            foreach (var item in (EnumApiTestItem[])Enum.GetValues(typeof(EnumApiTestItem)))
            {
                Console.WriteLine("┣━━━━━━━━━━━━━━━┫");
                Console.WriteLine("┃" + (item.GetHashCode() + "." + item).PadRight(30) + "┃");
            }
            Console.WriteLine("┗━━━━━━━━━━━━━━━┛");
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