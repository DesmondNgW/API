using Em.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Cache;
using System.Linq;
using System.Web;

namespace X.Util.Extend.Core
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum EnumCacheType
    {
        None,
        Runtime,
        MemCache,
        Redis,
        RedisBoth,
        MemBoth
    }
    /// <summary>
    /// 缓存过期时间级别
    /// </summary>
    public enum EnumCacheTimeLevel
    {
        Second,
        Minute,
        Hour,
        Day,
        Month,
        Year
    }

    public class CoreCache
    {
        #region 内部实现
        private static void AddContextCacheKeys(string key)
        {
            var token = ExecutionContext<RequestContext>.Current.Ctoken;
            if (string.IsNullOrEmpty(token)) return;
            var list = (LocalCache.Get(token) as IList<string>) ?? new List<string>();
            if (list.Contains(key)) return;
            list.Add(key);
            LocalCache.Set(token, list, DateTime.Now.AddMinutes(5));
        }

        private static string GetCacheKeyPrefix(MethodBase method)
        {
            var cacheKeyPrefix = method.DeclaringType?.FullName + "|" + method.Name;
            return method.GetParameters().OrderBy(p => p.Position).Aggregate(cacheKeyPrefix, (current, p) => current + ("_" + p.Name));
        }

        private static DateTime GetCacheExpire(EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            switch (cacheTimeLevel)
            {
                case EnumCacheTimeLevel.Year:
                    return DateTime.Now.AddYears(cacheTimeExpire);
                case EnumCacheTimeLevel.Month:
                    return DateTime.Now.AddMonths(cacheTimeExpire);
                case EnumCacheTimeLevel.Day:
                    return DateTime.Now.AddDays(cacheTimeExpire);
                case EnumCacheTimeLevel.Hour:
                    return DateTime.Now.AddHours(cacheTimeExpire);
                case EnumCacheTimeLevel.Minute:
                    return DateTime.Now.AddMinutes(cacheTimeExpire);
                case EnumCacheTimeLevel.Second:
                default:
                    return DateTime.Now.AddSeconds(cacheTimeExpire);
            }
        }

        private static CacheKey GetCacheKey(MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool addContext)
        {
            var result = new CacheKey {CacheKeyPrefix = GetCacheKeyPrefix(method)};
            var cacheKeyName = string.Empty;
            if (paramsList != null && paramsList.Count > 0)
            {
                cacheKeyName = paramsList.Aggregate(cacheKeyName, (current, o) => current + ("_" + o.ToJson()));
            }
            result.CacheKeyName = cacheKeyName;
            result.CacheKeyVersion = AppConfig.CacheKeyVersion;
            result.CacheAppVersion = cacheAppVersion;
            result.FullCacheKeyName = result.CacheKeyPrefix + result.CacheKeyName + "_" + result.CacheKeyVersion;
            if (addContext) AddContextCacheKeys(result.FullCacheKeyName);
            return result;
        }
        #endregion

        #region Api
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        public static void ClearCache(MethodBase method, ICollection<object> paramsList)
        {
            var cacheKeyName = string.Empty;
            if (paramsList != null && paramsList.Count > 0)
            {
                cacheKeyName = paramsList.Aggregate(cacheKeyName, (current, o) => current + ("_" + o.ToJson()));
            }
            var key = GetCacheKeyPrefix(method) + cacheKeyName + "_" + AppConfig.CacheKeyVersion;
            LocalCache.Remove(key);
            CouchCache.Default.Remove(key);
        }

        /// <summary>
        /// 绝对过期缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader"></param>
        /// <param name="method">缓存所在方法</param>
        /// <param name="paramsList">缓存所在方法的参数列表</param>
        /// <param name="cacheAppVersion">缓存值的版本</param>
        /// <param name="debugWithoutCache">debug模式去缓存</param>
        /// <param name="addContext">缓存加入上下文Context</param>
        /// <param name="cacheType">缓存类型</param>
        /// <param name="cacheTimeLevel">缓存过期时间级别</param>
        /// <param name="cacheTimeExpire">缓存过期时间</param>
        /// <returns></returns>
        public static ResultInfo<T> GetAbsoluteCacheData<T>(Func<ResultInfo<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            var result = default(ResultInfo<T>);
            var cacheKey = GetCacheKey(method, paramsList, cacheAppVersion, addContext);
            if (debugWithoutCache && HttpContext.Current.IsDebuggingEnabled || EnumCacheType.None.Equals(cacheType)) return loader();
            var expire = GetCacheExpire(cacheTimeLevel, cacheTimeExpire);
            var local = GetCacheExpire(cacheTimeLevel, cacheTimeExpire / 3);
            switch (cacheType)
            {
                case EnumCacheType.Runtime:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    break;
                case EnumCacheType.MemCache:
                    result = CacheData.Default.GetCouchCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    break;
                case EnumCacheType.Redis:
                    result = CacheData.Default.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    break;
                case EnumCacheType.MemBoth:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, local, () => CacheData.Default.GetCouchCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader));
                    break;
                case EnumCacheType.RedisBoth:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, local, () => CacheData.Default.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader));
                    break;
                case EnumCacheType.None:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 滑动过期缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader"></param>
        /// <param name="method">缓存所在方法</param>
        /// <param name="paramsList">缓存所在方法的参数列表</param>
        /// <param name="cacheAppVersion">缓存值的版本</param>
        /// <param name="debugWithoutCache">debug模式去缓存</param>
        /// <param name="addContext">缓存加入上下文Context</param>
        /// <param name="cacheType">缓存类型</param>
        /// <param name="cacheTimeLevel">缓存过期时间级别</param>
        /// <param name="cacheTimeExpire">缓存过期时间</param>
        /// <returns></returns>
        public static ResultInfo<T> GetSlidingCacheData<T>(Func<ResultInfo<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            var result = default(ResultInfo<T>);
            var cacheKey = GetCacheKey(method, paramsList, cacheAppVersion, addContext);
            if (debugWithoutCache && HttpContext.Current.IsDebuggingEnabled || EnumCacheType.None.Equals(cacheType)) return loader();
            var expire = GetCacheExpire(cacheTimeLevel, cacheTimeExpire) - DateTime.Now;
            var local = GetCacheExpire(cacheTimeLevel, cacheTimeExpire/3);
            switch (cacheType)
            {
                case EnumCacheType.Runtime:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    break;
                case EnumCacheType.MemCache:
                    result = CacheData.Default.GetCouchCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    break;
                case EnumCacheType.Redis:
                    result = CacheData.Default.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    break;
                case EnumCacheType.MemBoth:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, local, () => CacheData.Default.GetCouchCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader));
                    break;
                case EnumCacheType.RedisBoth:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, local, () => CacheData.Default.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader));
                    break;
                case EnumCacheType.None:
                    break;
            }
            return result;
        }

        #endregion
    }
}
