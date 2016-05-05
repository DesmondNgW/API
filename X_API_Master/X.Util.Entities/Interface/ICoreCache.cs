using System;
using System.Collections.Generic;
using System.Reflection;

namespace X.Util.Entities.Interface
{
    public interface ICoreCache
    {
        /// <summary>
        /// Clear Cache
        /// </summary>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        /// <param name="cacheType"></param>
        void ClearCache(MethodBase method, ICollection<object> paramsList, EnumCacheType cacheType);

        /// <summary>
        /// Get Absolute Cache Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader"></param>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        /// <param name="cacheAppVersion"></param>
        /// <param name="debugWithoutCache"></param>
        /// <param name="addContext"></param>
        /// <param name="cacheType"></param>
        /// <param name="cacheTimeLevel"></param>
        /// <param name="cacheTimeExpire"></param>
        /// <returns></returns>
        CacheResult<T> GetAbsoluteCacheData<T>(Func<CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire);

        /// <summary>
        /// Get Sliding Cache Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader"></param>
        /// <param name="method"></param>
        /// <param name="paramsList"></param>
        /// <param name="cacheAppVersion"></param>
        /// <param name="debugWithoutCache"></param>
        /// <param name="addContext"></param>
        /// <param name="cacheType"></param>
        /// <param name="cacheTimeLevel"></param>
        /// <param name="cacheTimeExpire"></param>
        /// <returns></returns>
        CacheResult<T> GetSlidingCacheData<T>(Func<CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire);

        /// <summary>
        /// Get Sliding Xml Cache Data
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
        /// <param name="filepath"></param>
        /// <returns></returns>
        CacheResult<T> GetSlidingCacheData<T>(Func<CacheResult<T>> loader, MethodBase method, ICollection<object> paramsList, string cacheAppVersion, bool debugWithoutCache, bool addContext, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire, string filepath);
    }
}
