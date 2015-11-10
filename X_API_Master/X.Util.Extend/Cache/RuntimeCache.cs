using System;
using System.Web.Caching;
using X.Util.Core;

namespace X.Util.Extend.Cache
{
    public class RuntimeCache
    {
        public static object Get(string key)
        {
            return LocalCache.Get(key);
        }

        public static void AbsoluteExpirationSet(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            LocalCache.AbsoluteExpirationSet(key, value, dependencies, absoluteExpiration, level);
        }

        public static void SlidingExpirationSet(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            LocalCache.SlidingExpirationSet(key, value, dependencies, slidingExpiration, level);
        }

        public static object Remove(string key)
        {
            return LocalCache.Remove(key);
        }

        public static T Get<T>(string key)
        {
            return LocalCache.Get<T>(key);
        }

        public static void Set(string key, object value, DateTime dt)
        {
            LocalCache.Set(key, value, dt);
        }

        public static void Set(string key, object value, TimeSpan ts)
        {
            LocalCache.Set(key, value, ts);
        }

        public static void Set(string key, object value, DateTime dt, CacheDependency dency)
        {
            LocalCache.Set(key, value, dt, dency);
        }

        public static void Set(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            LocalCache.Set(key, value, ts, dency);
        }
    }
}
