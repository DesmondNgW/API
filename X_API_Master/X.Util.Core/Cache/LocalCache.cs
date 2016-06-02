﻿using System;
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
            HttpRuntime.Cache.Insert(key, value, dependencies, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, level, null);
        }

        public static void AbsoluteExpirationSetJson(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dependencies, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, level, null);
        }

        public static void SlidingExpirationSet(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value, dependencies, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, level, null);
        }

        public static void SlidingExpirationSetJson(string key, object value, CacheDependency dependencies, TimeSpan slidingExpiration, CacheItemPriority level)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dependencies, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, level, null);
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

        public static T GetJson<T>(string key)
        {
            var o = (string)HttpRuntime.Cache.Get(key);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public static void Set(string key, object value, DateTime dt)
        {
            HttpRuntime.Cache.Insert(key, value, null, dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, DateTime dt)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), null, dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, TimeSpan ts)
        {
            HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, TimeSpan ts)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, DateTime dt, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value, dency, dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, DateTime dt, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dency, dt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
        }

        public static void Set(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value, dency, System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }

        public static void SetJson(string key, object value, TimeSpan ts, CacheDependency dency)
        {
            HttpRuntime.Cache.Insert(key, value.ToJson(), dency, System.Web.Caching.Cache.NoAbsoluteExpiration, ts, CacheItemPriority.AboveNormal, null);
        }
    }
}