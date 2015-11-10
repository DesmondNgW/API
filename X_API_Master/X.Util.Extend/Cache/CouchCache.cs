using Enyim.Caching.Memcached;
using System;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Cache
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public sealed class CouchCache
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.CouchDefaultServername;
        public CouchCache() { }
        public static readonly CouchCache Default = new CouchCache();
        public const LogDomain EDomain = LogDomain.ThirdParty;

        public CouchCache(string serverName)
        {
            ServerName = serverName;
        }
        #endregion

        #region 对外公开的Api
        private static bool CallSuccess<T>(CasResult<T> result)
        {
            return !Equals(result, default(CasResult<T>)) && !Equals(result.Result, default(T));
        }

        public object Get(string key)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess.TryCall(EDomain, couchBaseProvider.Instance.Get, key, CoreBase.CallSuccess, couchBaseProvider.Close, true, ServerName);
        }

        public object Get(string key, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess.TryCall(EDomain, couchBaseProvider.Instance.Get, key, dt, CoreBase.CallSuccess, couchBaseProvider.Close, true, ServerName);
        }

        public T Get<T>(string key)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess.TryCall(EDomain, couchBaseProvider.Instance.Get<T>, key, CoreBase.CallSuccess, couchBaseProvider.Close, true, ServerName);
        }

        public T Get<T>(string key, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess.TryCall(EDomain, couchBaseProvider.Instance.Get<T>, key, dt, CoreBase.CallSuccess, couchBaseProvider.Close, true, ServerName);
        }

        public bool TryGet<T>(string key, out T obj)
        {
            obj = Get<T>(key);
            return !Equals(obj, default(T));
        }

        public bool TryGet<T>(string key, DateTime dt, out T obj)
        {
            obj = Get<T>(key, dt);
            return !Equals(obj, default(T));
        }

        public void Set(string key, object obj)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, couchBaseProvider.Instance.Cas, StoreMode.Set, key, obj, (ulong)0, CallSuccess, couchBaseProvider.Close, null, false, ServerName);
        }

        public void Set(string key, object obj, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, couchBaseProvider.Instance.Cas, StoreMode.Set, key, obj, dt, (ulong)0, CallSuccess, couchBaseProvider.Close, null, false, ServerName);
        }

        public void Set(string key, object obj, TimeSpan ts)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, couchBaseProvider.Instance.Cas, StoreMode.Set, key, obj, ts, (ulong)0, CallSuccess, couchBaseProvider.Close, null, false, ServerName);
        }

        public void Remove(string key)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess.TryCallAsync(EDomain, couchBaseProvider.Instance.Remove, key, CoreBase.CallSuccess, couchBaseProvider.Close, null, false, ServerName);
        }
        #endregion
    }
}
