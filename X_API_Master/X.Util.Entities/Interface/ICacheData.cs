﻿using System;

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
    }
}