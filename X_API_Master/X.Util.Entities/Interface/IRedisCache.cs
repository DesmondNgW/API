using System;

namespace X.Util.Entities.Interface
{
    /// <summary>
    /// Redis Cache Interface
    /// </summary>
    public interface IRedisCache
    {
        T Get<T>(string key);

        T GetJson<T>(string key);

        void Set<T>(string key, T value);

        void SetJson<T>(string key, T value);

        void Set<T>(string key, T value, DateTime expire);

        void SetJson<T>(string key, T value, DateTime expire);

        void Set<T>(string key, T value, TimeSpan expire);

        void SetJson<T>(string key, T value, TimeSpan expire);

        void Remove(string key);
    }
}
