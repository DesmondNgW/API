using System;
using System.Runtime.Caching;
using X.Util.Core.Cache;

namespace X.Util.Extend.Cache
{
    public class RuntimeCache
    {
        private static LocalCache LocalCache = new LocalCache("RuntimeCache");

        public static object Get(string key)
        {
            return LocalCache.Get(key);
        }

        public static string GetJson(string key)
        {
            return LocalCache.GetJson(key);
        }

        public static void AbsoluteExpirationSet(string key, object value, DateTime absoluteExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            LocalCache.AbsoluteExpirationSet(key, value, absoluteExpiration, hostFileMonitor);
        }

        public static void AbsoluteExpirationSetJson(string key, object value, DateTime absoluteExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            LocalCache.AbsoluteExpirationSetJson(key, value, absoluteExpiration, hostFileMonitor);
        }

        public static void SlidingExpirationSet(string key, object value, TimeSpan slidingExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            LocalCache.SlidingExpirationSet(key, value, slidingExpiration, hostFileMonitor);
        }

        public static void SlidingExpirationSetJson(string key, object value, TimeSpan slidingExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            LocalCache.SlidingExpirationSetJson(key, value, slidingExpiration, hostFileMonitor);
        }

        public static object Remove(string key)
        {
            return LocalCache.Remove(key);
        }

        public static T Get<T>(string key)
        {
            return LocalCache.Get<T>(key);
        }

        public static T GetFromFile<T>(string key)
        {
            return LocalCache.GetFromFile<T>(key);
        }

        public static T GetJson<T>(string key)
        {
            return LocalCache.GetJson<T>(key);
        }

        public static T GetJsonFromFile<T>(string key)
        {
            return LocalCache.GetJsonFromFile<T>(key);
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
    }
}
