﻿using Enyim.Caching.Memcached;
using System;
using Couchbase;
using X.Util.Core;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Interface;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Util.Extend.Cache
{
    /// <summary>
    /// 缓存基础类
    /// </summary>
    public sealed class CouchCache : ICouchCache
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.CouchDefaultServername;
        private CouchCache() { }
        public static readonly ICouchCache Default = new CouchCache();

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
            if (!AppConfig.CouchCacheEnable) return null;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(couchBaseProvider.Client.Get, key, couchBaseProvider, new LogOptions<object>(CoreBase.CallSuccess));
        }

        public string GetJson(string key)
        {
            if (!AppConfig.CouchCacheEnable) return null;
            return (string)Get(key);
        }

        public object Get(string key, DateTime dt)
        {
            if (!AppConfig.CouchCacheEnable) return null;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(couchBaseProvider.Client.Get, key, dt, couchBaseProvider, new LogOptions<object>(CoreBase.CallSuccess));
        }

        public string GetJson(string key, DateTime dt)
        {
            if (!AppConfig.CouchCacheEnable) return null;
            return (string)Get(key, dt);
        }

        public T Get<T>(string key)
        {
            if (!AppConfig.CouchCacheEnable) return default(T);
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(couchBaseProvider.Client.Get<T>, key, couchBaseProvider, new LogOptions<T>(CoreBase.CallSuccess));
        }

        public T GetJson<T>(string key)
        {
            if (!AppConfig.CouchCacheEnable) return default(T);
            var o = GetJson(key);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public T Get<T>(string key, DateTime dt)
        {
            if (!AppConfig.CouchCacheEnable) return default(T);
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(couchBaseProvider.Client.Get<T>, key, dt, couchBaseProvider, new LogOptions<T>(CoreBase.CallSuccess));
        }

        public T GetJson<T>(string key, DateTime dt)
        {
            if (!AppConfig.CouchCacheEnable) return default(T);
            var o = GetJson(key, dt);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public bool TryGet<T>(string key, out T obj)
        {
            if (!AppConfig.CouchCacheEnable)
            {
                obj = default(T);
                return false;
            }
            obj = Get<T>(key);
            return !Equals(obj, default(T));
        }

        public bool TryGetJson<T>(string key, out T obj)
        {
            if (!AppConfig.CouchCacheEnable)
            {
                obj = default(T);
                return false;
            }
            obj = GetJson<T>(key);
            return !Equals(obj, default(T));
        }

        public bool TryGet<T>(string key, DateTime dt, out T obj)
        {
            if (!AppConfig.CouchCacheEnable)
            {
                obj = default(T);
                return false;
            }
            obj = Get<T>(key, dt);
            return !Equals(obj, default(T));
        }

        public bool TryGetJson<T>(string key, DateTime dt, out T obj)
        {
            if (!AppConfig.CouchCacheEnable)
            {
                obj = default(T);
                return false;
            }
            obj = GetJson<T>(key, dt);
            return !Equals(obj, default(T));
        }

        public void Set(string key, object obj)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Cas, StoreMode.Set, key, obj, (ulong)0, couchBaseProvider, null, new LogOptions<CasResult<bool>>(CallSuccess, true));
        }

        public void SetJson(string key, object obj)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Cas, StoreMode.Set, key, obj.ToJson(), (ulong)0, couchBaseProvider, null, new LogOptions<CasResult<bool>>(CallSuccess, true));
        }

        public void Set(string key, object obj, DateTime dt)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Cas, StoreMode.Set, key, obj, dt, (ulong)0, couchBaseProvider, null, new LogOptions<CasResult<bool>>(CallSuccess, true));
        }

        public void SetJson(string key, object obj, DateTime dt)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Cas, StoreMode.Set, key, obj.ToJson(), dt, (ulong)0, couchBaseProvider, null, new LogOptions<CasResult<bool>>(CallSuccess, true));
        }

        public void Set(string key, object obj, TimeSpan ts)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Cas, StoreMode.Set, key, obj, ts, (ulong)0, couchBaseProvider, null, new LogOptions<CasResult<bool>>(CallSuccess, true));
        }

        public void SetJson(string key, object obj, TimeSpan ts)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Cas, StoreMode.Set, key, obj.ToJson(), ts, (ulong)0, couchBaseProvider, null, new LogOptions<CasResult<bool>>(CallSuccess, true));
        }

        public void Remove(string key)
        {
            if (!AppConfig.CouchCacheEnable) return;
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(couchBaseProvider.Client.Remove, key, couchBaseProvider, null, new LogOptions<bool>(CoreBase.CallSuccess, true));
        }
        #endregion
    }
}
