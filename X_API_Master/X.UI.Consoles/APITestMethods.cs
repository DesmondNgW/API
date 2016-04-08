using System;
using System.Collections.Generic;
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
        ApiConsistentHashTest = 4,
        ApiRsaEnTest = 5,
        ApiRsaDeTest = 6
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
                Console.WriteLine(HttpRequestBase.GetHttpInfo(url, "utf8", string.Empty, null, cookie).ToJson());
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
