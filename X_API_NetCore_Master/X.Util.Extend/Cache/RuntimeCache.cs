using Microsoft.Extensions.Caching.Memory;
using System;
using X.Util.Core.Cache;

namespace X.Util.Extend.Cache
{
    public class RuntimeCache
    {
        private static readonly LocalCache LocalCache = new LocalCache();

        public static T Get<T>(object key)
        {
            return LocalCache.Get<T>(key);
        }

        public static void AbsoluteExpirationSet(object key, object value, DateTime absoluteExpiration, CacheItemPriority priority, string filePath)
        {
            LocalCache.AbsoluteExpirationSet(key, value, absoluteExpiration, priority, filePath);
        }

        public static void SlidingExpirationSet(object key, object value, TimeSpan slidingExpiration, CacheItemPriority priority, string filePath)
        {
            LocalCache.SlidingExpirationSet(key, value, slidingExpiration, priority, filePath);
        }

        public static void Remove(object key)
        {
            LocalCache.Remove(key);
        }

        public static void Set(string key, object value, DateTime dt, string filePath)
        {
            LocalCache.Set(key, value, dt, filePath);
        }

        public static void Set(string key, object value, TimeSpan ts, string filePath)
        {
            LocalCache.Set(key, value, ts, filePath);
        }
    }
}
