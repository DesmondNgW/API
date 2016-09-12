using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using X.Stock.Monitor;
using X.Stock.Monitor.Utils;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Mongo;
using X.Util.Other;

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
            //"http://nuff.eastmoney.com/EM_Finance2015TradeInterface/JS.ashx?id=3000592&token=beb0a0047196124721f56b0f0ff5a27c"
        }
    }

    public class Stock : MongoBaseModel
    {
        public string Code { get; set; }

        public string Amount { get; set; }

        public string Rate { get; set; }

        public DateTime Now { get; set; }

        public Stock(string code)
        {
            var format = "http://qt.gtimg.cn/q=ff_{0}{1}";
            var uri = string.Format(format, code.StartsWith("6") ? "sh" : "sz", code);
            var result = HttpRequestBase.GetHttpInfo(uri, "utf-8", "application/json", null, string.Empty);
            if (result.Success)
            {
                var arr = result.Content.Split('~');
                if (arr.Length <= 3) return;
                Id = code + "_" + DateTime.Now.ToString("yyyy-MM-dd");
                Code = code;
                Amount = arr[3];
                Rate = arr[4];
                Now = DateTime.Now;
            }
        }
    }


    class Program
    {
        static void Main()
        {
            //StockPoolService.ImportStockPool("gb2312");
            //StockPoolService.GetStockInfoFromPool();
            CustomerService.InitCustomerInfo(Work.CustomerNo, Work.CustomerName, Work.CoinAsset);
            Work.CreateThreads();



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
