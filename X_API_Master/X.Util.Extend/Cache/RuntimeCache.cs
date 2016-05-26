using System;
using System.Web.Caching;
using X.Util.Core;
using X.Util.Core.Cache;

namespace X.Util.Extend.Cache
{
    public class RuntimeCache
    {
        public static object Get(string key)
        {
            return LocalCache.Get(key);
        }

        public static string GetJson(string key)
        {
            return LocalCache.GetJson(key);
        }

        public static void AbsoluteExpirationSet(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            LocalCache.AbsoluteExpirationSet(key, value, dependencies, absoluteExpiration, level);
        }

        public static void AbsoluteExpirationSetJson(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            LocalCache.AbsoluteExpirationSetJson(key, value, dependencies, absoluteExpiration, level);
        }

        public static void SlidingExpirationSet(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            LocalCache.SlidingExpirationSet(key, value, dependencies, slidingExpiration, level);
        }

        public static void SlidingExpirationSetJson(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            LocalCache.SlidingExpirationSetJson(key, value, dependencies, slidingExpiration, level);
        }

        public static object Remove(string key)
        {
            return LocalCache.Remove(key);
        }

        public static T Get<T>(string key)
        {
            return LocalCache.Get<T>(key);
        }

        public static T GetJson<T>(string key)
        {
            return LocalCache.GetJson<T>(key);
        }

        public static void Set(string key, object value, DateTime dt)
        {
            LocalCache.Set(key, value, dt);
        }

        public static void SetJson(string key, object value, DateTime dt)
        {
            LocalCache.SetJson(key, value, dt);
        }

        public static void Set(string key, object value, TimeSpan ts)
        {
            LocalCache.Set(key, value, ts);
        }

        public static void SetJson(string key, object value, TimeSpan ts)
        {
            LocalCache.SetJson(key, value, ts);
        }

        public static void Set(string key, object value, DateTime dt, CacheDependency dency)
        {
            LocalCache.Set(key, value, dt, dency);
        }

        public static void SetJson(string key, object value, DateTime dt, CacheDependency dency)
        {
            LocalCache.SetJson(key, value, dt, dency);
        }

        public static void Set(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            LocalCache.Set(key, value, ts, dency);
        }

        public static void SetJson(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            LocalCache.SetJson(key, value, ts, dency);
        }
    }
}
