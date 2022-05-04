using Microsoft.Extensions.Caching.Memory;
using System;
using X.Util.Core.Cache;

namespace X.Util.Extend.Cache
{
    public class RuntimeCache
    {
        private static LocalCache LocalCache = new LocalCache();

        public static T Get<T>(object key)
        {
            return LocalCache.Get<T>(key);
        }

        public static void AbsoluteExpirationSet(object key, object value, DateTime absoluteExpiration, CacheItemPriority priority)
        {
            LocalCache.AbsoluteExpirationSet(key, value, absoluteExpiration, priority);
        }

        public static void SlidingExpirationSet(object key, object value, TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            LocalCache.SlidingExpirationSet(key, value, slidingExpiration, priority);
        }

        public static void Remove(object key)
        {
            LocalCache.Remove(key);
        }

        public static void Set(string key, object value, DateTime dt)
        {
            LocalCache.Set(key, value, dt);
        }

        public static void Set(string key, object value, TimeSpan ts)
        {
            LocalCache.Set(key, value, ts);
        }
    }
}
