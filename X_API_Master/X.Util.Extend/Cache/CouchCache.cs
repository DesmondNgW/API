﻿using Enyim.Caching.Memcached;
using System;
using Couchbase;
using X.Util.Core;
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
            return CoreAccess<ICouchbaseClient>.TryCall(EDomain, couchBaseProvider.Client.Get, key, CoreBase.CallSuccess, couchBaseProvider);
        }

        public string GetJson(string key)
        {
            return (string) Get(key);
        }

        public object Get(string key, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(EDomain, couchBaseProvider.Client.Get, key, dt, CoreBase.CallSuccess, couchBaseProvider);
        }

        public string GetJson(string key, DateTime dt)
        {
            return (string)Get(key, dt);
        }

        public T Get<T>(string key)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(EDomain, couchBaseProvider.Client.Get<T>, key, CoreBase.CallSuccess, couchBaseProvider);
        }

        public T GetJson<T>(string key)
        {
            var o = GetJson(key);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public T Get<T>(string key, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            return CoreAccess<ICouchbaseClient>.TryCall(EDomain, couchBaseProvider.Client.Get<T>, key, dt, CoreBase.CallSuccess, couchBaseProvider);
        }

        public T GetJson<T>(string key, DateTime dt)
        {
            var o = GetJson(key, dt);
            return o != null ? o.FromJson<T>() : default(T);
        }

        public bool TryGet<T>(string key, out T obj)
        {
            obj = Get<T>(key);
            return !Equals(obj, default(T));
        }

        public bool TryGetJson<T>(string key, out T obj)
        {
            obj = GetJson<T>(key);
            return !Equals(obj, default(T));
        }

        public bool TryGet<T>(string key, DateTime dt, out T obj)
        {
            obj = Get<T>(key, dt);
            return !Equals(obj, default(T));
        }

        public bool TryGetJson<T>(string key, DateTime dt, out T obj)
        {
            obj = GetJson<T>(key, dt);
            return !Equals(obj, default(T));
        }

        public void Set(string key, object obj)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Cas, StoreMode.Set, key, obj, (ulong)0, CallSuccess, couchBaseProvider, null, false);
        }

        public void SetJson(string key, object obj)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Cas, StoreMode.Set, key, obj.ToJson(), (ulong)0, CallSuccess, couchBaseProvider, null, false);
        }

        public void Set(string key, object obj, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Cas, StoreMode.Set, key, obj, dt, (ulong)0, CallSuccess, couchBaseProvider, null, false);
        }

        public void SetJson(string key, object obj, DateTime dt)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Cas, StoreMode.Set, key, obj.ToJson(), dt, (ulong)0, CallSuccess, couchBaseProvider, null, false);
        }

        public void Set(string key, object obj, TimeSpan ts)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Cas, StoreMode.Set, key, obj, ts, (ulong)0, CallSuccess, couchBaseProvider, null, false);
        }

        public void SetJson(string key, object obj, TimeSpan ts)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Cas, StoreMode.Set, key, obj.ToJson(), ts, (ulong)0, CallSuccess, couchBaseProvider, null, false);
        }

        public void Remove(string key)
        {
            var couchBaseProvider = new CouchBaseProvider(ServerName);
            CoreAccess<ICouchbaseClient>.TryCallAsync(EDomain, couchBaseProvider.Client.Remove, key, CoreBase.CallSuccess, couchBaseProvider, null, false);
        }
        #endregion
    }
}
