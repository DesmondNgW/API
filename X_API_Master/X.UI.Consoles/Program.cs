using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using X.Cache.Service;
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
        public static void TestCacheClient(int number)
        {
            if (number <= 1) number = 1;
            var client = new CacheClient("127.0.0.1", 12234, 1000);
            var c1 = 0;
            var c2 = 0;
            var total = new Stopwatch();
            total.Start();
            for (var i = 0; i < number; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                var sw = new Stopwatch();
                
                sw.Start();
                var ret1 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret1, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret2 = client.Set(key, key, DateTime.Now.AddMinutes(2));
                sw.Stop();
                Console.WriteLine("Set DateTime key {0},result {1}, ElapsedMilliseconds {2}", key, ret2, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret3 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret3, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret4 = client.Set(key, key, new TimeSpan(0, 2, 0));
                sw.Stop();
                Console.WriteLine("Set TimeSpan key {0},result {1}, ElapsedMilliseconds {2}", key, ret4, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;
                c2++;
                sw.Restart();
                var ret5 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret5, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret6 = client.Remove(key);
                sw.Stop();
                Console.WriteLine("Remove key {0},result {1}, ElapsedMilliseconds {2}", key, ret6, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;

                sw.Restart();
                var ret7 = client.Get(key);
                sw.Stop();
                Console.WriteLine("Get key {0},result {1}, ElapsedMilliseconds {2}", key, ret7, sw.ElapsedMilliseconds);
                if (sw.ElapsedMilliseconds > 0) c1++;
                c2++;
            }
            total.Stop();
            Console.WriteLine("above zoro {0}, total {1}", c1, c2);
            Console.WriteLine("Total ElapsedMilliseconds {0}", total.ElapsedMilliseconds);
        }

        static void Main()
        {
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
