using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using X.Util.Other;

namespace X.UI.Consoles
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(ChineseConvert.Get("相王璟").ToJson());
            Console.WriteLine(ChineseConvert.GetFirst("相王璟").ToJson());

            Console.WriteLine(ConsoleHelper.MobileEncrypt("15618169140").ToJson());
            Console.WriteLine(ConsoleHelper.MobileEncrypt("156****9140").ToJson());



            //TestCacheClient(1000);
            //StockPerformanceHelper.Compute(StockPerformanceHelper.Init(new DateTime(2017, 9, 25)));
            //MongoDbBase<MongoTest1>.Default.SaveMongo(new MongoTest1 { Dt = DateTime.Now, Value = 1, Key = "test" }, "Test", "test");
            //MongoDbBase<MongoTest2>.Default.SaveMongo(new MongoTest2 { Dt = DateTime.Now, Value = "15", Key = "test" }, "Test", "test");
            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);
            //Console.WriteLine(ThirdPartyTest.CouchBaseTest("1234567890", "1234567890"));
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
                typeof(ApiTestMethods).GetMethod(item.ToString()).Invoke(null, null);
                Index();
                break;
            }
        }
    }
}


/*
 * net work js 
 * 
 * 
 * 
 * 
 */