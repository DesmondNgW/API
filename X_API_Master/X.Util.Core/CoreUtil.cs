using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace X.Util.Core
{
    public class CoreUtil
    {
        private static volatile ConcurrentDictionary<string, object> _locker = new ConcurrentDictionary<string, object>();
        private const string CoderLockerPrefix = "X.Util.Core.CoderLockerPrefix";
        public const string LocalIp = "127.0.0.1";
        /// <summary>
        /// GetIP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            return Equals(HttpContext.Current, null) ? LocalIp : GetIp(new HttpContextWrapper(HttpContext.Current));
        }

        public static string GetIp(HttpContextWrapper context)
        {
            if (Equals(context, null)) return LocalIp;
            var ips = new[] { LocalIp, context.Request.ServerVariables["REMOTE_ADDR"], context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] };
            var length = ips.Length;
            var ret = ips[0];
            while (length-- > 0)
            {
                if (string.IsNullOrWhiteSpace(ips[length]) || Equals(ips[length], "::1")) continue;
                ret = ips[length];
                break;
            }
            return ret;
        }

        public static object Getlocker(string key)
        {
            return _locker.ContainsKey(key) && _locker[key] != null ? _locker[key] : (_locker[key] = new object());
        }

        /// <summary>
        /// Locker Code
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public static void CoderLocker(string key, Action callback)
        {
            key = CoderLockerPrefix + key;
            lock (Getlocker(key))
            {
                callback();
            }
        }
        /// <summary>
        /// SHA-1哈希算法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Sha1(string s)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(s));
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes)
                sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// 一致性hash
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConsistentHash(List<string> nodes, string key)
        {
            if (nodes.Count.Equals(0)) return string.Empty;
            var cacheKey = Sha1(nodes.ToJson());
            var circle = LocalCache.Get<IDictionary<string, string>>(cacheKey);
            if (Equals(circle, null))
            {
                circle = new Dictionary<string, string>();
                const int virtualNodeCount = 150;
                foreach (var t in nodes)
                {
                    for (var j = 0; j < virtualNodeCount; j++)
                    {
                        circle[Sha1(t + "_clone_" + j)] = t;
                    }
                }
                LocalCache.Set(cacheKey, circle, DateTime.Now.AddDays(1));
            }
            var order = circle.OrderBy(p => p.Key);
            var sha1 = Sha1(key);
            foreach (var item in order.Where(item => string.Compare(sha1, item.Key, StringComparison.Ordinal) <= 0))
            {
                return item.Value;
            }
            return order.FirstOrDefault().Value;
        }
    }
}
