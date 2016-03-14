using System;
using System.Web.Caching;
using X.Util.Core;
using X.Util.Entities;

namespace X.Util.Extend.Cache
{
    public class CacheData
    {
        #region 构造函数
        private readonly CouchCache _couch = CouchCache.Default;
        private readonly RedisCache _redis = RedisCache.Default;
        private const string Prefix = "X.Util.Extend.Cache.CacheData";
        private static readonly string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\CacheDependency.xml");//缓存依赖文件--用于删除本地缓存
        public CacheData() { }
        public static readonly CacheData Default = new CacheData();
        public CacheData(string couchName, string redisName)
        {
            _couch = new CouchCache(couchName);
            _redis = new RedisCache(redisName);
        }
        #endregion

        #region CacheData Api
        public static CacheResult<T> GetRuntimeCacheData<T>(string key, string version, DateTime dt, Func<CacheResult<T>> loader)
        {
            var setting = RuntimeCache.Get<CacheResult<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<CacheResult<T>>(key);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.AppVersion, version)) return setting;
                var iresult = loader();
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
