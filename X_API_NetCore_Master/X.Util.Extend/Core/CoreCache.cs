using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X.Util.Core;
using X.Util.Core.Cache;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enum;
using X.Util.Entities.Interface;
using X.Util.Extend.Cache;
using X.Util.Extend.Cryption;

namespace X.Util.Extend.Core
{
    public class CoreCache : ICoreCache
    {
        #region 构造函数
        public static ICoreCache Default = new CoreCache();
        private readonly ICacheData _cacheData = CacheData.Default;
        private CoreCache() { }
        public CoreCache(string redisName)
        {
            _cacheData = new CacheData(redisName);
        }
        #endregion

        #region 内部实现
        private static void AddContextCacheKeys(string key)
        {
            var token = ExecutionContext<BusinessRequestContext>.Current.Ctoken;
            if (string.IsNullOrEmpty(token)) return;
            var list = LocalCache.Default.Get<IList<string>>(token) ?? new List<string>();
            if (list.Contains(key)) return;
            list.Add(key);
            LocalCache.Default.Set(token, list, DateTime.Now.AddMinutes(5));
        }

        private static string GetCacheKeyPrefix(MethodBase method)
        {
            var cacheKeyPrefix = method.GetDeclaringFullName() + "|" + method.Name;
            return method.GetParameters().OrderBy(p => p.Position).Aggregate(cacheKeyPrefix, (current, p) => current + ("_" + p.Name));
        }

        private static string GetCacheKeyName(ICollection<object> paramsList)
        {
            var cacheKeyName = string.Empty;
            if (paramsList != null && paramsList.Count > 0)
            {
                cacheKeyName = paramsList.Aggregate(cacheKeyName, (current, o) => current + ("_" + o.ToJson()));
            }
            return cacheKeyName;
        }

        private static string FormatCacheKey(string key)
        {
            key = key.ReplaceRegex("\\s", string.Empty);
            if (key.Length >= 250) key = BaseCryption.Sha512(key);
            return key;
        }

        private static DateTime GetCacheExpire(EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            DateTime expire;
            return cacheTimeLevel == EnumCacheTimeLevel.Year && (expire = DateTime.Now.AddYears(cacheTimeExpire)) != DateTime.MinValue
                || cacheTimeLevel == EnumCacheTimeLevel.Month && (expire = DateTime.Now.AddMonths(cacheTimeExpire)) != DateTime.MinValue
                || cacheTimeLevel == EnumCacheTimeLevel.Day && (expire = DateTime.Now.AddDays(cacheTimeExpire)) != DateTime.MinValue
                || cacheTimeLevel == EnumCacheTimeLevel.Hour && (expire = DateTime.Now.AddHours(cacheTimeExpire)) != DateTime.MinValue
                || cacheTimeLevel == EnumCacheTimeLevel.Minute && (expire = DateTime.Now.AddMinutes(cacheTimeExpire)) != DateTime.MinValue
                || cacheTimeLevel == EnumCacheTimeLevel.Second && (expire = DateTime.Now.AddSeconds(cacheTimeExpire)) != DateTime.MinValue
                ? expire : DateTime.MinValue;
        }

        private static CacheKey GetCacheKey(MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool addContext)
        {
            var result = new CacheKey
            {
                CacheKeyPrefix = GetCacheKeyPrefix(method),
                CacheKeyName = GetCacheKeyName(paramsList),
                CacheKeyVersion = AppConfig.CacheKeyVersion,
                CacheAppVersion = cacheAppVersion
            };
            result.FullCacheKeyName = FormatCacheKey(result.CacheKeyPrefix + result.CacheKeyName + "_" + result.CacheKeyVersion);
            if (addContext) AddContextCacheKeys(result.FullCacheKeyName);
            return result;
        }
        #endregion

        #region Api

        #region 清除缓存

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        /// <param name="cacheType"></param>
        public void ClearCache(MethodBase method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            var key = GetCacheKeyPrefix(method) + GetCacheKeyName(paramsList) + "_" + AppConfig.CacheKeyVersion;
            key = FormatCacheKey(key);
            _cacheData.Remove(key, cacheType);
        }

        #region Action
        public void ClearCache(Action method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T>(Action<T> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2>(Action<T1, T2> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3>(Action<T1, T2, T3> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        #endregion

        #region Function
        public void ClearCache<T>(Func<T> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2>(Func<T1, T2> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3>(Func<T1, T2, T3> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4>(Func<T1, T2, T3, T4> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        public void ClearCache<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> method, ICollection<object> paramsList, EnumCacheType cacheType)
        {
            ClearCache(method.Method, paramsList, cacheType);
        }
        #endregion 
        #endregion

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
        public CacheResult<T> GetAbsoluteCacheData<T>(Func<CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            var result = default(CacheResult<T>);
            var cacheKey = GetCacheKey(method, paramsList, cacheAppVersion, addContext);
            if (debugWithoutCache && AppConfig.Debug || EnumCacheType.None.Equals(cacheType)) return loader();
            var expire = GetCacheExpire(cacheTimeLevel, cacheTimeExpire);
            var local = GetCacheExpire(cacheTimeLevel, cacheTimeExpire / 3);
            switch (cacheType)
            {
                case EnumCacheType.Runtime:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RuntimeCache" }, { 1, "InterfaceData" } }, method, LogDomain.Cache);
                    break;
                case EnumCacheType.Redis:
                    result = _cacheData.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RedisCache" }, { 1, "InterfaceData" } }, method, LogDomain.Cache);
                    break;
                case EnumCacheType.RedisBoth:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, local, () => _cacheData.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader));
                    CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RuntimeCache" }, { 1, "RedisCache" }, { 2, "InterfaceData" } }, method, LogDomain.Cache);
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
        public CacheResult<T> GetSlidingCacheData<T>(Func<CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            var result = default(CacheResult<T>);
            var cacheKey = GetCacheKey(method, paramsList, cacheAppVersion, addContext);
            if (debugWithoutCache && AppConfig.Debug || EnumCacheType.None.Equals(cacheType)) return loader();
            var expire = GetCacheExpire(cacheTimeLevel, cacheTimeExpire) - DateTime.Now;
            var local = GetCacheExpire(cacheTimeLevel, cacheTimeExpire / 3);
            switch (cacheType)
            {
                case EnumCacheType.Runtime:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RuntimeCache" }, { 1, "InterfaceData" } }, method, LogDomain.Cache);
                    break;
                case EnumCacheType.Redis:
                    result = _cacheData.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
                    CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RedisCache" }, { 1, "InterfaceData" } }, method, LogDomain.Cache);
                    break;
                case EnumCacheType.RedisBoth:
                    result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, local, () => _cacheData.GetRedisCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader));
                    CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RuntimeCache" }, { 1, "RedisCache" }, { 2, "InterfaceData" } }, method, LogDomain.Cache);
                    break;
                case EnumCacheType.None:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 数据源是本地xml的缓存依赖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader"></param>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        /// <param name="cacheAppVersion"></param>
        /// <param name="debugWithoutCache"></param>
        /// <param name="addContext"></param>
        /// <param name="cacheTimeLevel"></param>
        /// <param name="cacheTimeExpire"></param>
        /// <returns></returns>
        public CacheResult<T> GetSlidingCacheData<T>(Func<CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire)
        {
            var cacheKey = GetCacheKey(method, paramsList, cacheAppVersion, addContext);
            if (debugWithoutCache && AppConfig.Debug) return loader();
            var expire = GetCacheExpire(cacheTimeLevel, cacheTimeExpire) - DateTime.Now;
            var result = CacheData.GetRuntimeCacheData(cacheKey.FullCacheKeyName, cacheKey.CacheAppVersion, expire, loader);
            CacheData.LogCacheLevel(cacheKey.FullCacheKeyName, new Dictionary<int, string> { { 0, "RuntimeCache" }, { 1, "XmlFile" } }, method, LogDomain.Cache);
            return result;
        }

        /// <summary>
        /// 临时数据（用于数据库未更新前）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delay"></param>
        /// <param name="loader"></param>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        /// <param name="cacheType"></param>
        /// <param name="cacheAppVersion"></param>
        public void SetTmpCacheData<T>(TimeSpan delay, Func<CacheResult<T>, CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, EnumCacheType cacheType, string cacheAppVersion)
        {
            var cacheKey = GetCacheKey(method, paramsList, cacheAppVersion, false);
            _cacheData.SetTmpCacheData(cacheKey.FullCacheKeyName, cacheAppVersion, delay, loader, cacheType);
        }
        #endregion
    }
}
