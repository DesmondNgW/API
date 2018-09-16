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
            //ConsoleHelper.Draw(".", 30).ForEach(p => Console.WriteLine(p));
            //StockMonitor.Monitor();
            //var key = Guid.NewGuid().ToString("N");
            //Console.WriteLine(ThirdPartyTest.CouchBaseTest(key, key));
            //TestCacheClient(1000);

            //var s = MongoDbBase<MongoTest>.Default.FindBsonDocument("Test", "test", Query.Null);
            //var f = MongoDbBase<MongoTest>.ToEntity(s);
            
            var fs = new FileStream("C:\\new_tdx\\vipdoc\\sz\\lday\\sz300738.day", FileMode.Open);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            var a = GetValue(data, true);
            var b = GetValue(data, false);
            Console.ReadKey();
        }

        public static uint GetValue(byte[] data, bool High2Low)
        {
            if (!High2Low)
            {
                return (uint)(data[3] * 255 * 255 * 255 + data[2] * 255 * 255 + data[1] * 255 + data[0]);
            }
            else
            {
                return (uint)(data[0] * 255 * 255 * 255 + data[1] * 255 * 255 + data[2] * 255 + data[3]);
            }

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