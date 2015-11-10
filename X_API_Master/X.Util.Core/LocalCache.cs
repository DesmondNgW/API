using System;
using System.Web;
using System.Web.Caching;

namespace X.Util.Core
{
    public class LocalCache
    {
        public static object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public static void AbsoluteExpirationSet(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value, dependencies, absoluteExpiration, Cache.NoSlidingExpiration, level, null);
        }

        public static void SlidingExpirationSet(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value, dependencies, Cache.NoAbsoluteExpiration, slidingExpiration, level, null);
        }

        public static object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }

        public static T Get<T>(string key)
        {
            var o = HttpRuntime.Cache.Get(key);
            return o != null ? (T)o : default(T);
        }

        public static void Set(string key, object value, DateTime dt)
        {
            HttpRuntime.Cache.Insert(key, value, null, dt, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, TimeSpan ts)
        {
            HttpRuntime.Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, DateTime dt, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value, dency, dt, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value, dency, Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }
    }
}
