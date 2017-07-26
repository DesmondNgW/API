using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace X.Util.Entities.Interface
{
    /// <summary>
    /// Redis Cache Interface
    /// </summary>
    public interface IRedisCache
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, DateTime expire);

        void Set<T>(string key, T value, TimeSpan expire);

        bool KeyDelete(string key);
    }
}
