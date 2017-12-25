using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using X.Cache.Service;
using X.UI.Consoles.Stock;
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


        private static void WriteFile(IEnumerable<StockScoreExtend> data)
        {
            FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv",
                "Inc|Close|Score|Score2|Score3|Score4|Score5|Score6|Score7|Score8|Score9|Score10|Y1|Y2|Y3|Y4|Y5", "utf-8",
                FileBaseMode.Create);
            foreach (var item in data)
            {
                var content =
                    string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}",
                        item.Stock.Inc,
                        item.Stock.Close,
                        item.Score,
                        item.Score2,
                        item.Score3,
                        item.Score4,
                        item.Score5,
                        item.Score6,
                        item.Score7,
                        item.Score8,
                        item.Score9,
                        item.Score10,
                        item.Stock.Y[1],
                        item.Stock.Y[2],
                        item.Stock.Y[3],
                        item.Stock.Y[4],
                        item.Stock.Y[5]);
                FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "1.csv", content, "utf-8", FileBaseMode.Append);
            }
        }

        private static void WriteFile2(IEnumerable<StockScoreExtend> data, int count, double step)
        {
            FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "2.csv",
                "Item|Score|Score2|Score3|Score4|Score5|Score6|Score7|Score8|Score9|Score10", "utf-8",
                FileBaseMode.Create);
            for (var i = 0; i < count; i++)
            {
                var st = i*step;
                var et = i*step + step;
                Func<StockScoreExtend, bool> t1 = p => p.Score > st && p.Score < et;
                Func<StockScoreExtend, bool> t2 = p => p.Score2 > st && p.Score2 < et;
                Func<StockScoreExtend, bool> t3 = p => p.Score3 > st && p.Score3 < et;
                Func<StockScoreExtend, bool> t4 = p => p.Score4 > st && p.Score4 < et;
                Func<StockScoreExtend, bool> t5 = p => p.Score5 > st && p.Score5 < et;
                Func<StockScoreExtend, bool> t6 = p => p.Score6 > st && p.Score6 < et;
                Func<StockScoreExtend, bool> t7 = p => p.Score7 > st && p.Score7 < et;
                Func<StockScoreExtend, bool> t8 = p => p.Score8 > st && p.Score8 < et;
                Func<StockScoreExtend, bool> t9 = p => p.Score9 > st && p.Score9 < et;
                Func<StockScoreExtend, bool> t10 = p => p.Score10 > st && p.Score10 < et;
                var content =
                    string.Format(
                        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}{9}|{10}",
                        st + "," + et,
                        data.Count(t1),
                        data.Count(t2),
                        data.Count(t3),
                        data.Count(t4),
                        data.Count(t5),
                        data.Count(t6),
                        data.Count(t7),
                        data.Count(t8),
                        data.Count(t9),
                        data.Count(t10));
                FileBase.WriteFile(AppDomain.CurrentDomain.BaseDirectory, "2.csv", content, "utf-8", FileBaseMode.Append);
            }
        }

        static void Main()
        {
            Console.WriteLine(string.Join("|", ChineseConvert.Get("相王璟")));
            Console.WriteLine(string.Join(",", ChineseConvert.GetFirst("相王璟")));

            Console.WriteLine(ConsoleHelper.MobileEncrypt("15618169140").ToJson());
            Console.WriteLine(ConsoleHelper.MobileEncrypt("156****9140").ToJson());

            //var g = StockHelper.G(0.1, -0.1, 1000, 20);
            //var s = StockScoreHelper.GetScore(g);
            //var se = StockScoreHelper.GetScoreExtend(s);

            //Func<StockScoreExtend, bool> func =
            //    p => p.Stock.Y[1] >= 0.05 || 
            //        p.Stock.Y[2] >= 0.10 || 
            //        p.Stock.Y[3] >= 0.15 || 
            //        p.Stock.Y[4] >= 0.20||
            //        p.Stock.Y[5] >= 0.25;

            //var all = se.Where(func);

            //WriteFile2(all, 50, 0.1);

            //WriteFile(all);


            //Console.WriteLine("1.MobileEncrypt");
            //Console.WriteLine("请输入选项和参数，格式如1.15901846845");
            //string input;
            //while (!string.IsNullOrEmpty(input = Console.ReadLine()))
            //{
            //    switch (input.Split('.')[0])
            //    {
            //        case "1":
            //            Console.WriteLine(ConsoleHelper.MobileEncrypt(input.Split('.')[1]).ToJson());
            //            break;
            //    }
            //}

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
