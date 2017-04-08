using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Caching;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Entities.Interface;

namespace X.Util.Extend.Cache
{
    public class CacheData : ICacheData
    {
        #region 构造函数
        private readonly ICouchCache _couch = CouchCache.Default;
        private readonly IRedisCache _redis = RedisCache.Default;
        private const string Prefix = "X.Util.Extend.Cache.CacheData";
        
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
            Logger.Client.Debug(string.Format("{0}.{1} Get CacheData hit {2}, cache key is {3}.", method.DeclaringType, method.Name, dictionary.ContainsKey(level) ? dictionary[level] : "level-" + level, key), domain);
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
                if (setting.Succeed) RuntimeCache.Set(key, setting, dt);
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
                if (setting.Succeed) RuntimeCache.Set(key, setting, ts, string.IsNullOrEmpty(filepath) ? null : new CacheDependency(filepath));
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
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15));
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
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15));
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
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15));
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
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15));
                _redis.Set(key, setting, ts);
            }
            return setting;
        }
        #endregion

        #region Tmp CacheData
        private static CacheResult<T> GetTmpCacheData<T>(Func<CacheResult<T>, CacheResult<T>> loader, CacheResult<T> obj, string version)
        {
            var setting = loader(obj);
            if (Equals(setting, null)) return obj;
            setting.AppVersion = version;
            setting.CacheTime = DateTime.Now;
            setting.Succeed = setting.Result != null && setting.Succeed;
            return setting;
        }

        public void SetTmpCacheData<T>(string key, string version, TimeSpan delay, Func<CacheResult<T>, CacheResult<T>> loader, EnumCacheType cacheType)
        {
            CacheResult<T> obj;
            CacheResult<T> setting;
            switch (cacheType)
            {
                case EnumCacheType.Runtime:
                    obj = RuntimeCache.Get<CacheResult<T>>(key);
                    setting = GetTmpCacheData(loader, obj, version);
                    if (setting != null)
                    {
                        RuntimeCache.Set(key, setting, DateTime.Now.Add(delay));
                    }
                    else
                    {
                        RuntimeCache.Remove(key);
                    }
                    break;
                case EnumCacheType.MemCache:
                    obj = _couch.Get<CacheResult<T>>(key);
                    setting = GetTmpCacheData(loader, obj, version);
                    if (setting != null)
                    {
                        _couch.Set(key, setting, DateTime.Now.Add(delay));
                    }
                    else
                    {
                        _couch.Remove(key);
                    }
                    break;
                case EnumCacheType.Redis:
                    obj = _redis.Get<CacheResult<T>>(key);
                    setting = GetTmpCacheData(loader, obj, version);
                    if (setting != null)
                    {
                        _redis.Set(key, setting, DateTime.Now.Add(delay));
                    }
                    else
                    {
                        _redis.Remove(key);
                    }
                    break;
                case EnumCacheType.MemBoth:
                    obj = RuntimeCache.Get<CacheResult<T>>(key) ?? _couch.Get<CacheResult<T>>(key);
                    setting = GetTmpCacheData(loader, obj, version);
                    if (setting != null)
                    {
                        RuntimeCache.Set(key, setting, DateTime.Now.Add(delay));
                        _couch.Set(key, setting, DateTime.Now.Add(delay));
                    }
                    else
                    {
                        RuntimeCache.Remove(key);
                        _couch.Remove(key);
                    }
                    break;
                case EnumCacheType.RedisBoth:
                    obj = RuntimeCache.Get<CacheResult<T>>(key) ?? _redis.Get<CacheResult<T>>(key);
                    setting = GetTmpCacheData(loader, obj, version);
                    if (setting != null)
                    {
                        RuntimeCache.Set(key, setting, DateTime.Now.Add(delay));
                        _redis.Set(key, setting, DateTime.Now.Add(delay));
                    }
                    else
                    {
                        RuntimeCache.Remove(key);
                        _redis.Remove(key);
                    }
                    break;
                case EnumCacheType.None:
                    break;
            }
        }
        #endregion
    }
}
