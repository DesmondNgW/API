using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
//using X.Stock.DB;
//using X.Stock.Service;
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
            //CustomerTable.InitCustomerInfo(Work.CustomerNo, Work.CustomerName, Work.CoinAsset);
            //Work.CreateThreads();

            //for (var i = 0; i < 10; i++)
            //{
            //    var th = new Thread(() => { ThirdPartyTest.HighFrequencyTest(100, ThirdPartyTest.CouchBaseTest); })
            //    {
            //        IsBackground = true
            //    };
            //    th.Start();
            //}

            Area.AreaTest();

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
