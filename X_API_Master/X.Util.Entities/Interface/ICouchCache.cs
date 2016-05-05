using System;

namespace X.Util.Entities.Interface
{
    /// <summary>
    /// Couchbase Cache Interface
    /// </summary>
    public interface ICouchCache
    {
        object Get(string key);

        string GetJson(string key);

        object Get(string key, DateTime dt);

        string GetJson(string key, DateTime dt);

        T Get<T>(string key);

        T GetJson<T>(string key);

        T Get<T>(string key, DateTime dt);

        T GetJson<T>(string key, DateTime dt);

        bool TryGet<T>(string key, out T obj);

        bool TryGetJson<T>(string key, out T obj);

        bool TryGet<T>(string key, DateTime dt, out T obj);

        bool TryGetJson<T>(string key, DateTime dt, out T obj);

        void Set(string key, object obj);

        void SetJson(string key, object obj);

        void Set(string key, object obj, DateTime dt);

        void SetJson(string key, object obj, DateTime dt);

        void Set(string key, object obj, TimeSpan ts);

        void SetJson(string key, object obj, TimeSpan ts);

        void Remove(string key);
    }
}
