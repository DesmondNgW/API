using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.IO;

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

        /// <summary>
        /// GetMemoryCacheEntryOptions
        /// </summary>
        /// <param name="option"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static MemoryCacheEntryOptions GetMemoryCacheEntryOptions(MemoryCacheEntryOptions option, string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    option.AddExpirationToken(new PollingFileChangeToken(fileInfo));
                }
            }
            return option;
        }


        public T AbsoluteExpirationSet<T>(object key, T value, DateTime absoluteExpiration, CacheItemPriority Priority, string filePath)
        {
            return MemoryCacheClient.Set(key, value, GetMemoryCacheEntryOptions(new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = absoluteExpiration,
                Priority = Priority
            }, filePath));
        }

        public T SlidingExpirationSet<T>(object key, T value, TimeSpan slidingExpiration, CacheItemPriority Priority, string filePath)
        {
            return MemoryCacheClient.Set(key, value, GetMemoryCacheEntryOptions(new MemoryCacheEntryOptions()
            {
                SlidingExpiration = slidingExpiration,
                Priority = Priority
            }, filePath));
        }

        public void Remove(object key)
        {
            MemoryCacheClient.Remove(key);
        }

        public void Set(string key, object value, DateTime dt, string filePath)
        {
            AbsoluteExpirationSet(key, value, dt, CacheItemPriority.Normal, filePath);
        }

        public void Set(string key, object value, TimeSpan ts, string filePath)
        {
            SlidingExpirationSet(key, value, ts, CacheItemPriority.Normal, filePath);
        }
    }
}
