using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using X.Util.Core.Cache;

namespace X.Util.Core.Kernel
{
    public class CoreUtil
    {
        private static readonly ConcurrentDictionary<string, object> _locker = new ConcurrentDictionary<string, object>();
        private const string CoderLockerPrefix = "X.Util.Core.Kernel.CoderLockerPrefix";
        public delegate bool ParseFunc<T>(string value, out T t);
        public delegate bool ParseFunc2<T>(string value, NumberStyles style, IFormatProvider provider, out T t);
        public delegate bool ParseFunc3<T>(string value, string format, IFormatProvider provider, DateTimeStyles style, out T t);

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
            var encryptedBytes = sha1.ComputeHash(s.ToAsciiBytes());
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
            var circle = LocalCache.Default.Get<IDictionary<string, string>>(cacheKey);
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
                LocalCache.Default.Set(cacheKey, circle, DateTime.Now.AddDays(1), string.Empty);
            }
            var order = circle.OrderBy(p => p.Key);
            var sha1 = Sha1(key);
            foreach (var item in order.Where(item => string.Compare(sha1, item.Key, StringComparison.Ordinal) <= 0))
            {
                return item.Value;
            }
            return order.FirstOrDefault().Value;
        }

        public static T Convert2Type<T>(string value, ParseFunc<T> parse, T defaultValue)
        {
            return parse(value, out T result) ? result : defaultValue;
        }

        public static T? Convert2Type<T>(string value, ParseFunc<T> parse, T? defaultValue) where T : struct
        {
            return parse(value, out T result) ? result : defaultValue;
        }

        public static T Convert2Type<T>(string value, ParseFunc2<T> parse, NumberStyles style, IFormatProvider provider, T defaultValue)
        {
            return parse(value, style, provider, out T result) ? result : defaultValue;
        }

        public static T? Convert2Type<T>(string value, ParseFunc2<T> parse, NumberStyles style, IFormatProvider provider, T? defaultValue) where T : struct
        {
            return parse(value, style, provider, out T result) ? result : defaultValue;
        }

        public static T Convert2Type<T>(string value, ParseFunc3<T> parse, string format, DateTimeStyles style, IFormatProvider provider, T defaultValue)
        {
            return parse(value, format, provider, style, out T result) ? result : defaultValue;
        }

        public static T? Convert2Type<T>(string value, ParseFunc3<T> parse, string format, DateTimeStyles style, IFormatProvider provider, T? defaultValue) where T : struct
        {
            return parse(value, format, provider, style, out T result) ? result : defaultValue;
        }
    }
}
