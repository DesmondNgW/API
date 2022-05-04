using X.Util.Entities.Enum;

namespace X.Util.Entities
{
    public class CacheOptions
    {
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
    }
}
