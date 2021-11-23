using System;
using System.Collections.Generic;
using X.UI.Helper;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Extend.Cryption;
using X.Util.Other;

namespace X.UI.Consoles
{
    public enum EnumApiTestItem
    {
        ApiMd5Test = 1,
        ApiCacheTest = 2,
        ApiWebHtmlTest = 3,
        ApiConsistentHashTest = 4,
        ApiRsaEnTest = 5,
        ApiRsaDeTest = 6,
        StockDeal = 7,
        StockProgram = 8,
        TestStockPrice = 9,
        MyTest = 10
    }

    public class ApiTestMethods
    {
        public static void ApiMd5Test()
        {
            Console.WriteLine(@"请输入需要计算MD5值的字符串:");
            string md5;
            while (!string.IsNullOrEmpty(md5 = Console.ReadLine()))
            {
                Console.WriteLine(BaseCryption.Md5Bit32(md5));
            }
        }

        public static void ApiCacheTest()
        {
            do
            {
                Console.WriteLine(@"CouchBase测试:");
                Console.WriteLine(ThirdPartyTest.CouchBaseTest("test", "CouchBaseWriteTest"));
                Console.WriteLine(@"MongoDB测试:");
                ThirdPartyTest.MongoTest();
                Console.WriteLine(@"Redis测试:");
                Console.WriteLine(ThirdPartyTest.RedisTest("test", "RedisWriteTest"));
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }

        public static void ApiWebHtmlTest()
        {
            Console.WriteLine(@"请输入需要访问的url:");
            string url;
            var cookie = string.Empty;
            while (!string.IsNullOrEmpty(url = Console.ReadLine()))
            {
                Console.WriteLine(HttpRequestBase.GetHttpInfo(url, "utf8", string.Empty, null, cookie).ToJson());
            }
        }

        public static void ApiConsistentHashTest()
        {
            var nodes = new List<string> { "1", "2", "3" };
            do
            {
                Console.WriteLine(@"一致性Hash测试:");
                var dic = new Dictionary<string, int>();
                for (var i = 0; i < 10000; i++)
                {
                    var r = CoreUtil.GetConsistentHash(nodes, Guid.NewGuid().ToString("N"));
                    if (dic.ContainsKey(r)) dic[r]++;
                    else dic[r] = 1;
                    Console.WriteLine(@"{0:HH:mm:ss.fff} 序号：{1} 值：{2} 次数：{3}", DateTime.Now, i, r, dic[r]);
                }
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }

        public static void ApiRsaEnTest()
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            Console.WriteLine(@"请输入需要加密的字符串:");
            string rsa;
            while (!string.IsNullOrEmpty(rsa = Console.ReadLine()))
            {
                var en = RsaCryption.Encrypt(id, rsa, nonce);
                Console.WriteLine(@"密文：" + en);
            }
        }

        public static void ApiRsaDeTest()
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            Console.WriteLine(@"请输入需要解密的字符串:");
            string rsa;
            while (!string.IsNullOrEmpty(rsa = Console.ReadLine()))
            {
                var de = RsaCryption.Decrypt(id, rsa, nonce);
                Console.WriteLine(@"明文：" + de);
            }
        }

        public static void StockDeal()
        {
            JRJDataHelper.DealData(new DateTime(2020, 1, 1), DateTime.Now.Date);
            var list = JRJDataHelper.Continue(new DateTime(2020, 1, 1), DateTime.Now.Date, 2);
            JRJDataHelper.ContinueInFile(list);
        }

        public static void StockProgram()
        {
            StockDealHelper.Main();
        }

        public static void TestStockPrice()
        {
            Console.WriteLine(@"请输入高价-低价-收盘-基础价:");
            string inputs;
            while (!string.IsNullOrEmpty(inputs = Console.ReadLine()))
            {
                var arr = inputs.Split(' ');
                if (arr.Length >= 4)
                {
                    var t0 = arr[3].Convert2Decimal(0);
                    var t1 = arr[0].Convert2Decimal(0) * 1.025M;
                    var t2 = arr[1].Convert2Decimal(0) * 1.025M;
                    var t3 = arr[2].Convert2Decimal(0);
                    Console.WriteLine(@"{0}[{1}]", t1, t1 / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t2, t2 / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 1.15M, t0 * 1.15M / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 1.3M, t0 * 1.3M / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 1.5M, t0 * 1.5M / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 1.75M, t0 * 1.75M / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 2M, t0 * 2M / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 2.25M, t0 * 2.25M / t3 * 100 - 100);
                    Console.WriteLine(@"{0}[{1}]", t0 * 2.5M, t0 * 2.5M / t3 * 100 - 100);
                }
            }
        }

        //tmp todo
        public static void MyTest()
        {
            
        }
    }
}
