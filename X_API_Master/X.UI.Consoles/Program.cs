using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using X.UI.API.Controllers;
using X.Util.Core;
using X.Util.Core.Cache;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Core.Xml;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Extend.Core;
using X.Util.Extend.Cryption;
//using MongoDB.Bson;
//using MongoDB.Driver.Builders;
//using X.Util.Entities;
using X.Util.Extend.Mongo;
using X.Util.Other;

//using X.Util.Core;

namespace X.UI.Consoles
{
    public class Channel
    {
        public static ConcurrentQueue<int> Queue = new ConcurrentQueue<int>();
        private static readonly ManualResetEvent EventWait = new ManualResetEvent(false);

        public void Init()
        {
            for (var i = 4; i < 5; i++) In(i);
        }

        public void In(int i)
        {
            Queue.Enqueue(i);
            EventWait.Set();
        }

        public int Out()
        {
            int r;
            Queue.TryDequeue(out r);
            Thread.Sleep(1000);
            return r;
        }

        public void Test()
        {
            while (true)
            {
                var o = Out();
                if (o == default(int))
                {
                    EventWait.Reset();
                    EventWait.WaitOne();
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff>") + o);
                    break;
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            //var s = StockHelper.G(0.1, -0.1, 1000, 20);
            //var l = ScoreHelper.GetScore(s);
            ////var t = ScoreHelper.GetScoreExtend(l);
            //var head =
            //    string.Format(
            //        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}",
            //        "Item1",
            //        "Item2",
            //        "Item3",
            //        "Item4",
            //        "Item5",
            //        "Item6",
            //        "Item7",
            //        "Value",
            //        "Y[1]",
            //        "Y[2]",
            //        "Y[3]",
            //        "Y[4]",
            //        "Y[5]",
            //        "Y[6]",
            //        "Y[7]",
            //        "Y[8]",
            //        "Y[9]",
            //        "Y[10]",
            //        "Y[11]",
            //        "Y[12]");
            //FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv", head, "utf8", FileBaseMode.Create);

            //foreach (var d in l)
            //{
            //    var content = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}",
            //        d.Item1,
            //        d.Item2,
            //        d.Item3,
            //        d.Item4,
            //        d.Item5,
            //        d.Item6,
            //        d.Item7,
            //        d.Value,
            //        d.Stock.Y[1],
            //        d.Stock.Y[2],
            //        d.Stock.Y[3],
            //        d.Stock.Y[4],
            //        d.Stock.Y[5],
            //        d.Stock.Y[6],
            //        d.Stock.Y[7],
            //        d.Stock.Y[8],
            //        d.Stock.Y[9],
            //        d.Stock.Y[10],
            //        d.Stock.Y[11],
            //        d.Stock.Y[12]);
            //    FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv", content, "utf8", FileBaseMode.Append);
            //}
            //FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "2.txt", t.ToJson(), "utf8", FileBaseMode.Create);

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
