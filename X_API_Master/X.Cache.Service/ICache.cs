using System;
using X.Util.Core.Cache;

namespace X.Cache.Service
{
    public interface ICache
    {
        object Get(string key);

        bool Set(SendModel send);

        bool Remove(string key);
    }

    public class CacheManager : ICache
    {
        private static LocalCache LocalCache = new LocalCache("CacheManager");
        public object Get(string key)
        {
            return LocalCache.Get(key);
        }

        public bool Set(SendModel send)
        {
            if (send.ExpireAt != DateTime.MinValue)
            {
                LocalCache.Set(send.Key, send.Value, send.ExpireAt);
            }
            else if (send.Expire != TimeSpan.Zero)
            {
                LocalCache.Set(send.Key, send.Value, send.Expire);
            }
            return true;
        }

        public bool Remove(string key)
        {
            LocalCache.Remove(key);
            return true;
        }
    }
}
