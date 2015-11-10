using System;
using System.Collections.Generic;
using X.UI.API.Controllers;
using X.Util.Core;
using X.Util.Extend.Cryption;
using X.Util.Other;

namespace X.UI.Consoles
{
    public enum EnumApiTestItem
    {
        ApiMd5Test = 1,
        ApiCacheTest = 2,
        ApiWebHtmlTest = 3,
        ApiWorkdayTest = 4,
        ApiConsistentHashTest = 5,
        ApiFundCompaniesTest = 6,
        ApiFundsTest = 7,
        ApiRsaEnTest = 8,
        ApiRsaDeTest = 9
    }

    public class ApiTestMethods
    {
        public static void ApiMd5Test()
        {
            Console.WriteLine("请输入需要计算MD5值的字符串:");
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
                Console.WriteLine("CouchBase测试:");
                Console.WriteLine(ThirdPartyTest.CouchBaseTest());
                Console.WriteLine("MongoDB测试:");
                ThirdPartyTest.MongoTest();
                Console.WriteLine("Redis测试:");
                Console.WriteLine(ThirdPartyTest.RedisTest());
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }

        public static void ApiWebHtmlTest()
        {
            Console.WriteLine("请输入需要访问的url:");
            string url;
            var cookie = string.Empty;
            while (!string.IsNullOrEmpty(url = Console.ReadLine()))
            {
                Console.WriteLine(HttpRequestBase.GetHttpInfo(url, "utf8", string.Empty, null, ref cookie));
            }
        }

        public static void ApiWorkdayTest()
        {
            Console.WriteLine("从此时开始的后续多个交易日:");
            string i;
            var controller = new WorkDayController();
            while (!string.IsNullOrEmpty(i = Console.ReadLine()))
            {
                int count;
                int.TryParse(i, out count);
                while (count > 0)
                {
                    Console.WriteLine("第" + count + "天:" + controller.GetPointWorkday(DateTime.Now, count).ToJson());
                    count--;
                }
            }
        }

        public static void ApiConsistentHashTest()
        {
            var nodes = new List<string> { "1", "2", "3" };
            do
            {
                Console.WriteLine("一致性Hash测试:");
                var dic = new Dictionary<string, int>();
                for (var i = 0; i < 10000; i++)
                {
                    var r = CoreUtil.GetConsistentHash(nodes, Guid.NewGuid().ToString("N"));
                    if (dic.ContainsKey(r)) dic[r]++;
                    else dic[r] = 1;
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + "\t序号：" + i + "\t值：" + r + "\t次数：" + dic[r]);
                }
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }

        public static void ApiFundCompaniesTest()
        {
            do
            {
                Console.WriteLine("所有基金公司的基金信息:");
                var controller = new FundController();
                Console.WriteLine(controller.GetFundCompanies().ToJson());
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }
        public static void ApiFundsTest()
        {
            do
            {
                Console.WriteLine("所有的基金信息:");
                var controller = new FundController();
                Console.WriteLine(controller.GetFunds().ToJson());
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }

        public static void ApiRsaEnTest()
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            Console.WriteLine("请输入需要加密的字符串:");
            string rsa;
            while (!string.IsNullOrEmpty(rsa = Console.ReadLine()))
            {
                var en = RsaCryption.Encrypt(id, rsa, nonce);
                Console.WriteLine("密文：" + en);
            }
        }

        public static void ApiRsaDeTest()
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            Console.WriteLine("请输入需要解密的字符串:");
            string rsa;
            while (!string.IsNullOrEmpty(rsa = Console.ReadLine()))
            {
                var de = RsaCryption.Decrypt(id, rsa, nonce);
                Console.WriteLine("明文：" + de);
            }
        }
    }
}
