using Em.Entities;
using System;
using System.Web.Caching;
using X.Util.Core;

namespace X.Util.Extend.Cache
{
    public class CacheData
    {
        #region 构造函数
        private static CouchCache _couch = CouchCache.Default;
        private static RedisCache _redis = RedisCache.Default;
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
        public static ResultInfo<T> GetRuntimeCacheData<T>(string key, string version, DateTime dt, Func<ResultInfo<T>> loader)
        {
            var setting = RuntimeCache.Get<ResultInfo<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<ResultInfo<T>>(key);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
                var iresult = loader();
                setting = iresult;
                if (setting != null)
                {
                    setting.preValue = version;
                    setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                    if (setting.Succeed) RuntimeCache.Set(key, setting, dt, new CacheDependency(Path));
                }
            }
            return setting;
        }

        public static ResultInfo<T> GetRuntimeCacheData<T>(string key, string version, TimeSpan ts, Func<ResultInfo<T>> loader)
        {
            var setting = RuntimeCache.Get<ResultInfo<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<ResultInfo<T>>(key);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
                var iresult = loader();
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.preValue = version;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (setting.Succeed) RuntimeCache.Set(key, setting, ts, new CacheDependency(Path));
            }
            return setting;
        }

        public ResultInfo<T> GetCouchCacheData<T>(string key, string version, DateTime dt, Func<ResultInfo<T>> loader)
        {
            var lockKey = key + "couchlock";
            var setting = _couch.Get<ResultInfo<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<ResultInfo<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
                var iresult = loader();
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.preValue = version;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _couch.Set(key, setting, dt);
            }
            return setting;
        }

        public ResultInfo<T> GetCouchCacheData<T>(string key, string version, TimeSpan ts, Func<ResultInfo<T>> loader)
        {
            var lockKey = key + "couchlock";
            var setting = _couch.Get<ResultInfo<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<ResultInfo<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
                var iresult = loader();
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.preValue = version;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _couch.Set(key, setting, ts);
            }
            return setting;
        }

        public ResultInfo<T> GetRedisCacheData<T>(string key, string version, DateTime dt, Func<ResultInfo<T>> loader)
        {
            var lockKey = key + "redislock";
            var setting = _redis.Get<ResultInfo<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<ResultInfo<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
                var iresult = loader();
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.preValue = version;
                setting.Succeed = iresult != null && iresult.Result != null && iresult.Succeed;
                if (!setting.Succeed) return setting;
                RuntimeCache.Set(lockKey, setting, DateTime.Now.AddSeconds(15), new CacheDependency(Path));
                _redis.Set(key, setting, dt);
            }
            return setting;
        }

        public ResultInfo<T> GetRedisCacheData<T>(string key, string version, TimeSpan ts, Func<ResultInfo<T>> loader)
        {
            var lockKey = key + "redislock";
            var setting = _redis.Get<ResultInfo<T>>(key);
            if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
            lock (CoreUtil.Getlocker(Prefix + key))
            {
                setting = RuntimeCache.Get<ResultInfo<T>>(lockKey);
                if (setting != null && setting.Result != null && setting.Succeed && Equals(setting.preValue, version)) return setting;
                var iresult = loader();
                setting = iresult;
                if (Equals(setting, null)) return null;
                setting.preValue = version;
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
