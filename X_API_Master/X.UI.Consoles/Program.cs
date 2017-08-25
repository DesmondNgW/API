using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using X.UI.API;
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
            StockPerformanceHelper.Compute(StockPerformanceHelper.Init(new DateTime(2017, 8, 21)));
            //MongoDbBase<MongoTest1>.Default.SaveMongo(new MongoTest1 { Dt = DateTime.Now, Value = 1, Key = "test" }, "Test", "test");
            //MongoDbBase<MongoTest2>.Default.SaveMongo(new MongoTest2 { Dt = DateTime.Now, Value = "15", Key = "test" }, "Test", "test");
            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);
            //Console.WriteLine(ThirdPartyTest.CouchBaseTest("1234567890", "1234567890"));
            Console.ReadKey();
        }

        //static void Send()
        //{
        //    for (var i = 0; i < 10; i++)
        //    {
        //        var sw = new Stopwatch();
        //        sw.Start();
        //        var t = NetworkCommsHelper.Send<DateTime, DateTime>("127.0.0.1", 10000, "Test", DateTime.Now, 1000);
        //        sw.Stop();
        //        Console.WriteLine("客户端总耗时:" + sw.ElapsedMilliseconds);
        //        Console.WriteLine("服务端发送到客户端总耗时:" + (DateTime.Now - t).TotalMilliseconds);
        //    }
        //}

        //static void Reply()
        //{
        //    NetworkCommsHelper.StartListening("127.0.0.1", 10000);
        //    NetworkCommsHelper.Reply<DateTime, DateTime>("Test", a =>
        //    {
        //        Console.WriteLine("客户端发送到服务端总耗时:" + (DateTime.Now - a).TotalMilliseconds);
        //        return DateTime.Now;
        //    });
        //}


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
