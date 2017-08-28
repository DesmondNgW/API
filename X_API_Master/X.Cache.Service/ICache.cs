using System;
using System.Web;

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
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public bool Set(SendModel send)
        {
            if (send.ExpireAt != DateTime.MinValue)
            {
                HttpRuntime.Cache.Insert(send.Key, send.Value, null, send.ExpireAt, System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else if (send.Expire != TimeSpan.Zero)
            {
                HttpRuntime.Cache.Insert(send.Key, send.Value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, send.Expire);
            }
            return true;
        }

        public bool Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
            return true;
        }
    }
}
