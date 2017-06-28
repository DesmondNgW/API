using System;
using X.Util.Entities.Enums;

namespace X.Util.Entities.Interface
{
    public interface ICacheData
    {
        /// <summary>
        /// get Couchbase cache Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <param name="dt"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        CacheResult<T> GetCouchCacheData<T>(string key, string version, DateTime dt, Func<CacheResult<T>> loader);

        /// <summary>
        /// get Couchbase cache Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <param name="ts"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        CacheResult<T> GetCouchCacheData<T>(string key, string version, TimeSpan ts, Func<CacheResult<T>> loader);

        /// <summary>
        /// get redis cache Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <param name="dt"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        CacheResult<T> GetRedisCacheData<T>(string key, string version, DateTime dt, Func<CacheResult<T>> loader);

        /// <summary>
        /// get redis cache Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <param name="ts"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        CacheResult<T> GetRedisCacheData<T>(string key, string version, TimeSpan ts, Func<CacheResult<T>> loader);

        /// <summary>
        /// 临时有效数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="version"></param>
        /// <param name="delay"></param>
        /// <param name="loader"></param>
        /// <param name="cacheType"></param>
        void SetTmpCacheData<T>(string key, string version, TimeSpan delay, Func<CacheResult<T>, CacheResult<T>> loader, EnumCacheType cacheType);

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cacheType"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        T GetCacheDbData<T>(string key, EnumCacheType cacheType, DateTime? expire = null);

        /// <summary>
        /// 设置绝对过期缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expire"></param>
        /// <param name="cacheType"></param>
        void SetCacheDbData<T>(string key, T obj, DateTime expire, EnumCacheType cacheType);

        /// <summary>
        /// 设置滑动过期缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="ts"></param>
        /// <param name="cacheType"></param>
        void SetCacheDbData<T>(string key, T obj, TimeSpan ts, EnumCacheType cacheType);

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheType"></param>
        void Remove(string key, EnumCacheType cacheType);
    }
}
