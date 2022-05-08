using Microsoft.Extensions.Caching.Memory;
using System;

namespace X.Util.Core.Cache
{
    public class LocalCache
    {
        public static LocalCache Default = new LocalCache();
        private static readonly IMemoryCache MemoryCacheClient = new MemoryCache(new MemoryCacheOptions());
        public LocalCache() { }

        public T Get<T>(object key)
        {
            return MemoryCacheClient.Get<T>(key);
        }

        public T AbsoluteExpirationSet<T>(object key, T value, DateTime absoluteExpiration, CacheItemPriority Priority)
        {
            return MemoryCacheClient.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = absoluteExpiration,
                Priority = Priority
            });
        }

        public T SlidingExpirationSet<T>(object key, T value, TimeSpan slidingExpiration, CacheItemPriority Priority)
        {
            return MemoryCacheClient.Set(key, value, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = slidingExpiration,
                Priority = Priority
            });
        }

        public void Remove(object key)
        {
            MemoryCacheClient.Remove(key);
        }

        public void Set(string key, object value, DateTime dt)
        {
            AbsoluteExpirationSet(key, value, dt, CacheItemPriority.Normal);
        }

        public void Set(string key, object value, TimeSpan ts)
        {
            SlidingExpirationSet(key, value, ts, CacheItemPriority.Normal);
        }
    }
}
