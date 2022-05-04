

namespace X.Util.Entities.Enum
{
    /// <summary>
    /// 缓存过期类型
    /// </summary>
    public enum EnumCacheExpireType
    {
        Sliding,
        Absolute
    }

    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum EnumCacheType
    {
        None,
        Runtime,
        Redis,
        RedisBoth,
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
}
