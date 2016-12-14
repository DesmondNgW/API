namespace X.Util.Entities.Enums
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
}
