using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Entities.Interface;
using X.Util.Other;
using X.Util.Provider;

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
            //Index();
            var c1 = new Channel();
            var c2 = new Channel();
            var c3 = new Channel();
            c2.Init();
            var th = new Thread(() =>
            {
                c1.Test();
                c1.In(7);
            });
            var th2 = new Thread(() =>
            {
                c2.Test();
                c2.In(8);
            });
            var th3 = new Thread(() =>
            {
                c3.Test();
                c3.In(9);
            });
            th.Start();
            th2.Start();
            th3.Start();

            //Console.WriteLine(ThirdPartyTest.CouchBaseTest());

            //Func<string, int, string> pad = (s, c) => s.Length < c ? string.Join("0", new string[c - s.Length + 1]) + s : s;

            //var result = new List<string>();
            //for (var i = 2; i <= 999999; i++)
            //{
            //    var count = 0;
            //    for (var j = 2; j <= Math.Sqrt(i); j++)
            //    {
            //        if (i % j != 0) continue;
            //        count++;
            //        break;
            //    }
            //    if (count != 0) continue;
            //    if ((i >= 1000 || i <= 0) && (i < 2000 || i >= 3000) && (i < 300000 || i >= 301000) &&
            //        (i < 600000 || i >= 610000)) continue;
            //    var uri = "http://hq.sinajs.cn/list=sh" + pad(i.ToString(), 6);
            //    var d = HttpRequestBase.GetHttpInfo(uri, "utf-8", "application/json", null, string.Empty);
            //    if (d.Content.Length < 50) continue;
            //    Console.WriteLine(pad(i.ToString(), 6));
            //    result.Add(pad(i.ToString(), 6));
            //}
            //Console.WriteLine(result.ToJson());
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
