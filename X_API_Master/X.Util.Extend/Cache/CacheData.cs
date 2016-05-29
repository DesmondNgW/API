﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Caching;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Extend.Cache
{
    public class CacheData : ICacheData
    {
        #region 构造函数
        private readonly ICouchCache _couch = CouchCache.Default;
        private readonly IRedisCache _redis = RedisCache.Default;
        private const string Prefix = "X.Util.Extend.Cache.CacheData";
        private static readonly string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\CacheDependency.xml");//缓存依赖文件--用于删除本地缓存
        private CacheData() { }
        public static readonly ICacheData Default = new CacheData();
        public CacheData(string couchName, string redisName)
        {
            _couch = new CouchCache(couchName);
            _redis = new RedisCache(redisName);
        }
        #endregion

        #region CacheData Api
        private static void AddHitLevel(string key)
        {
            var value = CallContext.GetData(key);
            if (value == null) CallContext.SetData(key, 1);
            else
            {
                CallContext.SetData(key, value.GetHashCode() + 1);
            }
        }

        public static void LogHitLevel(string key, Dictionary<int, string> dictionary, MethodBase method, LogDomain domain)
        {
            var value = CallContext.GetData(key);
            var level = value == null ? 0 : value.GetHashCode();
            Logger.Client.Debug(method, domain, null, string.Empty, string.Format("{0}.{1} GetCacheData hit {2}, cache key is {3}.", method.GetDeclaringFullName(), method.Name, dictionary.ContainsKey(level) ? dictionary[level] : "level-" + level, key));
        }

        public static CacheResult<T> GetRuntimeCacheData<T>(string key, string version, DateTime dt, Func<CacheResult<T>> loader)
        {
            var setting = RuntimeCache.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(key);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
                AddHitLevel(key);
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.AppVersion = version;
                setting.CacheTime = DateTime.Now;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (setting.Succeed) RuntimeCache.Set(key, setting, dt, new CacheDependency(Path));
            }
            return setting;
        }

        public static CacheResult<T> GetRuntimeCacheData<T>(string key, string version, TimeSpan ts, Func<CacheResult<T>> loader, string filepath)
        {
            var setting = RuntimeCache.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(key);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
                AddHitLevel(key);
                CallContext.SetData(key, 1);
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.AppVersion = version;
                setting.CacheTime = DateTime.Now;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (setting.Succeed) RuntimeCache.Set(key, setting, ts, new CacheDependency(string.IsNullOrEmpty(filepath) ? Path : filepath));
            }
            return setting;
        }

        public CacheResult<T> GetCouchCacheData<T>(string key, string version, DateTime dt, Func<CacheResult<T>> loader)
        {
            var lockKey = key + "couchlock";
            var setting = _couch.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
                AddHitLevel(key);
                CallContext.SetData(key, 1);
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.AppVersion = version;
                setting.CacheTime = DateTime.Now;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _couch.Set(key, setting, dt);
            }
            return setting;
        }

        public CacheResult<T> GetCouchCacheData<T>(string key, string version, TimeSpan ts, Func<CacheResult<T>> loader)
        {
            var lockKey = key + "couchlock";
            var setting = _couch.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
                AddHitLevel(key);
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.AppVersion = version;
                setting.CacheTime = DateTime.Now;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _couch.Set(key, setting, ts);
            }
            return setting;
        }

        public CacheResult<T> GetRedisCacheData<T>(string key, string version, DateTime dt, Func<CacheResult<T>> loader)
        {
            var lockKey = key + "redislock";
            var setting = _redis.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
                AddHitLevel(key);
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.AppVersion = version;
                setting.CacheTime = DateTime.Now;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _redis.Set(key, setting, dt);
            }
            return setting;
        }

        public CacheResult<T> GetRedisCacheData<T>(string key, string version, TimeSpan ts, Func<CacheResult<T>> loader)
        {
            var lockKey = key + "redislock";
            var setting = _redis.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
                AddHitLevel(key);
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.AppVersion = version;
                setting.CacheTime = DateTime.Now;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _redis.Set(key, setting, ts);
            }
            return setting;
        }
        #endregion
    }
}
