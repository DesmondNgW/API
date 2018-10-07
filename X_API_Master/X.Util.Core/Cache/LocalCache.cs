using System;
using System.Runtime.Caching;

namespace X.Util.Core.Cache
{
    public class LocalCache
    {
        public static LocalCache Default = new LocalCache();
        private static MemoryCache _MemoryCache = MemoryCache.Default;
        public LocalCache() { }
        public LocalCache(string name)
        {
            _MemoryCache = new MemoryCache(name);
        }

        public object Get(string key)
        {
            return _MemoryCache.Get(key);
        }

        public string GetJson(string key)
        {
            return (string)_MemoryCache.Get(key);
        }

        public void AbsoluteExpirationSet(string key, object value, DateTime absoluteExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
            };
            policy.ChangeMonitors.Add(hostFileMonitor ?? CacheDependencyHelper.CacheDependency(key, value));
            _MemoryCache.Add(new CacheItem(key, value), policy);
        }

        public void AbsoluteExpirationSetJson(string key, object value, DateTime absoluteExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            AbsoluteExpirationSet(key, value.ToJson(), absoluteExpiration, hostFileMonitor);
        }

        public void SlidingExpirationSet(string key, object value, TimeSpan slidingExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = slidingExpiration,
            };
            policy.ChangeMonitors.Add(hostFileMonitor ?? CacheDependencyHelper.CacheDependency(key, value));
            _MemoryCache.Add(new CacheItem(key, value), policy);
        }

        public void SlidingExpirationSetJson(string key, object value, TimeSpan slidingExpiration, HostFileChangeMonitor hostFileMonitor)
        {
            SlidingExpirationSet(key, value.ToJson(), slidingExpiration, hostFileMonitor);
        }

        public object Remove(string key)
        {
            CacheDependencyHelper.UpdateCacheDependencyFile(key, null);
            return _MemoryCache.Remove(key);
        }

        public T Get<T>(string key)
        {
            var o = _MemoryCache.Get(key);
            return o != null ? (T)o : default(T);
        }

        public T GetFromFile<T>(string key)
        {
            var o = _MemoryCache.Get(key);
            return o != null ? (T)o : CacheDependencyHelper.GetCacheResultFromFile<T>(key);
        }

        public T GetJson<T>(string key)
        {
            var o = (string)_MemoryCache.Get(key);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public T GetJsonFromFile<T>(string key)
        {
            var o = (string)_MemoryCache.Get(key);
            return o != null ? o.FromJson<T>() : CacheDependencyHelper.GetCacheResultFromFile<T>(key);
        }

        public void Set(string key, object value, DateTime dt)
        {
            AbsoluteExpirationSet(key, value, dt, null);
        }

        public void SetJson(string key, object value, DateTime dt)
        {
            AbsoluteExpirationSetJson(key, value, dt, null);
        }

        public void Set(string key, object value, TimeSpan ts)
        {
            SlidingExpirationSet(key, value, ts, null);
        }

        public void SetJson(string key, object value, TimeSpan ts)
        {
            SlidingExpirationSetJson(key, value, ts, null);
        }
    }
}
