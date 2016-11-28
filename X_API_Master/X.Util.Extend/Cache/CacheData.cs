﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Caching;
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
        /// <summary>
        /// 增加命中缓存的等级
        /// </summary>
        /// <param name="key"></param>
        private static void CacheLevelUp(string key)
        {
            var value = CallContext.GetData(key);
            CallContext.SetData(key, value == null ? 1 : value.GetHashCode() + 1);
        }

        /// <summary>
        /// 记录命中缓存的等级
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dictionary"></param>
        /// <param name="method"></param>
        /// <param name="domain"></param>
        public static void LogCacheLevel(string key, Dictionary<int, string> dictionary, MethodBase method, LogDomain domain)
        {
            var value = CallContext.GetData(key);
            var level = value == null ? 0 : value.GetHashCode();
            Logger.Client.Debug(method, domain, null, string.Empty, string.Format("{0}.{1} Get CacheData hit {2}, cache key is {3}.", method.DeclaringType, method.Name, dictionary.ContainsKey(level) ? dictionary[level] : "level-" + level, key));
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
                CacheLevelUp(key);
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
                CacheLevelUp(key);
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
                CacheLevelUp(key);
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
                CacheLevelUp(key);
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
                CacheLevelUp(key);
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
                CacheLevelUp(key);
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

        #region Tmp CacheData
        public void SetTmpCacheData<T>(string key, string version, TimeSpan delay, Func<CacheResult<T>> loader, EnumCacheType cacheType)
        {
            var setting = loader();
            if (Equals(setting, null)) return;
            setting.AppVersion = version;
            setting.CacheTime = DateTime.Now;
            setting.Succeed = setting.Result != null && setting.Succeed;
            switch (cacheType)
            {
                case EnumCacheType.Runtime:
                    RuntimeCache.Set(key, setting, DateTime.Now.Add(delay), new CacheDependency(Path));
                    break;
                case EnumCacheType.MemCache:
                    _couch.Set(key, setting, DateTime.Now.Add(delay));
                    break;
                case EnumCacheType.Redis:
                    _redis.Set(key, setting, DateTime.Now.Add(delay));
                    break;
                case EnumCacheType.MemBoth:
                    RuntimeCache.Set(key, setting, DateTime.Now.Add(delay), new CacheDependency(Path));
                    _couch.Set(key, setting, DateTime.Now.Add(delay));
                    break;
                case EnumCacheType.RedisBoth:
                    RuntimeCache.Set(key, setting, DateTime.Now.Add(delay), new CacheDependency(Path));
                    _redis.Set(key, setting, DateTime.Now.Add(delay));
                    break;
                case EnumCacheType.None:
                    break;
            }
        }
        #endregion
    }
}
