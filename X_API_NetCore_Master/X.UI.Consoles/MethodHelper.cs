using X.UI.Util.Helper;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Extend.Cryption;
using X.Util.Other;

namespace X.UI.Consoles
{
    public enum EnumMethodItem
    {
        GetAllTcpConnections = 1,
        Md5 = 2,
        WebHtml = 3,
        ConsistentHash = 4,
        RsaEncrypt = 5,
        RsaDecrypt = 6,
    }

    /// <summary>
    /// MethodHelper
    /// </summary>
    internal class MethodHelper
    {
        public static void GetTcpConnections(string ip, int port)
        {
            var list = MonitorHelper.GetTcpConnection(ip, port);
            foreach (var t in list)
            {
                Console.WriteLine("Local endpoint: {0},Remote endpoint: {1}, {2}", t.Local, t.Remote, t.TcpState);
            }
        }

        public static void GetAllTcpConnections()
        {
            do
            {
                GetTcpConnections(string.Empty, 0);
            }
            while (!string.IsNullOrEmpty(Console.ReadLine()));
        }


        public static void Md5()
        {
            Console.WriteLine(@"请输入需要计算MD5值的字符串:");
            string md5;
            while (!string.IsNullOrEmpty(md5 = Console.ReadLine()))
            {
                Console.WriteLine(BaseCryption.Md5Bit32(md5));
            }
        }

        public static void WebHtml()
        {
            Console.WriteLine(@"请输入需要访问的url:");
            string url;
            var cookie = string.Empty;
            while (!string.IsNullOrEmpty(url = Console.ReadLine()))
            {
                Console.WriteLine(HttpRequestBase.GetHttpInfo(url, "utf8", string.Empty, null, cookie).ToJson());
            }
        }

        public static void ConsistentHash()
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

        public static void RsaEncrypt()
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

        public static void RsaDecrypt()
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
    }

    /// <summary>
    /// ConsoleUIHelper
    /// </summary>
    internal class ConsoleUIHelper
    {
        /// <summary>
        /// Center
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="content"></param>
        /// <param name="pad"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static string Center(string begin, string end, string content, string pad, int total)
        {
            var len = total - begin.Length - end.Length - content.Length;
            int leftLen = len / 2 + 1, retLength = total - content.Length;
            var ret = new string[retLength + 1];
            ret[0] = begin;
            ret[retLength] = end;
            ret[leftLen] = content;
            return string.Join(pad, ret);
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="total"></param>
        public static void Index(int total = 32)
        {
            Console.Clear();
            Console.WriteLine(Center("┏", "┓", "━", "━", total));
            Console.WriteLine(Center("┃", "┃", "Console Method List:", "*", total));
            foreach (var item in (EnumMethodItem[])Enum.GetValues(typeof(EnumMethodItem)))
            {
                Console.WriteLine(Center("┣", "┫", "━", "━", total));
                Console.WriteLine(Center("┃", "┃", string.Format("{0}.{1}", item.GetHashCode(), item.ToString()), "*", total));
            }
            Console.WriteLine(Center("┗", "┛", "━", "━", total));
            Next();
        }

        /// <summary>
        /// Next
        /// </summary>
        public static void Next()
        {
            var input = Console.ReadLine();
            foreach (var item in ((EnumMethodItem[])Enum.GetValues(typeof(EnumMethodItem))).Where(item => Equals(item.GetHashCode().ToString(), input)))
            {
                Console.Clear();
                var methodInfo = typeof(MethodHelper).GetMethod(item.ToString());
                methodInfo?.Invoke(null, null);
                Index();
                break;
            }
        }
    }
}
