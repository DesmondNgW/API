using System;
using System.Web;
using System.Web.Caching;

namespace X.Util.Core.Cache
{
    public class LocalCache
    {
        public static object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public static string GetJson(string key)
        {
            return (string)HttpRuntime.Cache.Get(key);
        }

        public static void AbsoluteExpirationSet(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value, dependencies ?? CacheDependencyHelper.CacheDependency(key, value), absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, level, null);
        }

        public static void AbsoluteExpirationSetJson(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dependencies ?? CacheDependencyHelper.CacheDependency(key, value), absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, level, null);
        }

        public static void SlidingExpirationSet(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value, dependencies ?? CacheDependencyHelper.CacheDependency(key, value), System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, level, null);
        }

        public static void SlidingExpirationSetJson(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dependencies ?? CacheDependencyHelper.CacheDependency(key, value), System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, level, null);
        }

        public static object Remove(string key)
        {
            CacheDependencyHelper.UpdateCacheDependencyFile(key, null);
            return HttpRuntime.Cache.Remove(key);
        }

        public static T Get<T>(string key)
        {
            var o = HttpRuntime.Cache.Get(key);
            return o != null ? (T)o : default(T);
        }

        public static T GetFromFile<T>(string key)
        {
            var o = HttpRuntime.Cache.Get(key);
            return o != null ? (T)o : CacheDependencyHelper.GetCacheResultFromFile<T>(key);
        }

        public static T GetJson<T>(string key)
        {
            var o = (string)HttpRuntime.Cache.Get(key);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public static T GetJsonFromFile<T>(string key)
        {
            var o = (string) HttpRuntime.Cache.Get(key);
            return o != null ? o.FromJson<T>() : CacheDependencyHelper.GetCacheResultFromFile<T>(key);
        }

        public static void Set(string key, object value, DateTime dt)
        {
            HttpRuntime.Cache.Insert(key, value, CacheDependencyHelper.CacheDependency(key, value), dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, DateTime dt)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), CacheDependencyHelper.CacheDependency(key, value), dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, TimeSpan ts)
        {
            HttpRuntime.Cache.Insert(key, value, CacheDependencyHelper.CacheDependency(key, value), System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, TimeSpan ts)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), CacheDependencyHelper.CacheDependency(key, value), System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, DateTime dt, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value, dency ?? CacheDependencyHelper.CacheDependency(key, value), dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, DateTime dt, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dency ?? CacheDependencyHelper.CacheDependency(key, value), dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value, dency ?? CacheDependencyHelper.CacheDependency(key, value), System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dency ?? CacheDependencyHelper.CacheDependency(key, value), System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }
    }
}
