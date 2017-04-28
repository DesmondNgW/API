using System;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Entities.Interface;

namespace X.Util.Extend.Core
{
    public class CacheContextAttribute : ContextResultAttribute
    {
        public CacheContextAttribute(EnumCacheExpireType cacheExpireType, EnumCacheType cacheType, EnumCacheTimeLevel cacheTimeLevel, int cacheTimeExpire, string cacheAppVersion, bool debugWithoutCache, bool addContext)
        {
            CacheExpireType = cacheExpireType;
            CacheType = cacheType;
            CacheTimeLevel = cacheTimeLevel;
            CacheTimeExpire = cacheTimeExpire;
            CacheAppVersion = cacheAppVersion;
            DebugWithoutCache = debugWithoutCache;
            AddContext = addContext;
        }

        /// <summary>
        /// 缓存过期类型
        /// </summary>
        public EnumCacheExpireType CacheExpireType { get; set; }

        /// <summary>
        /// 缓存类型
        /// </summary>
        public EnumCacheType CacheType { get; set; }

        /// <summary>
        /// 缓存级别
        /// </summary>
        public EnumCacheTimeLevel CacheTimeLevel { get; set; }

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public int CacheTimeExpire { get; set; }

        /// <summary>
        /// 缓存版本号
        /// </summary>
        public string CacheAppVersion { get; set; }

        /// <summary>
        /// debug 模式无缓存
        /// </summary>
        public bool DebugWithoutCache { get; set; }

        /// <summary>
        /// AddContext 加入上下文中，
        /// </summary>
        public bool AddContext { get; set; }


        public override IContext<TResult, TChannel> GetContext<TResult, TChannel>(IProvider<TChannel> channel, Func<TResult, bool> callSuccess)
        {
            return new CacheContext<TResult, TChannel>(channel, callSuccess, new CacheOptions
            {
                CacheExpireType = CacheExpireType,
                CacheType = CacheType,
                CacheTimeLevel = CacheTimeLevel,
                CacheTimeExpire = CacheTimeExpire,
                CacheAppVersion = CacheAppVersion,
                DebugWithoutCache = DebugWithoutCache,
                AddContext = AddContext,
            });
        }
    }
}
