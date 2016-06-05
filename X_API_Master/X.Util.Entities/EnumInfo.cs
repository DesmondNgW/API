namespace X.Util.Entities
{
    public enum LogDomain
    {
        Util,
        ThirdParty,
        Core,
        CoreExtend,
        Cache,
        Business,
        Interface,
        Db,
        Ui
    }
    /// <summary>
    /// 日志监控模块
    /// </summary>
    public enum LogMonitorDomain
    {
        Trade,
        User,
        Query,
        Pay,
        Test,
        Other
    }
    public enum LogType
    {
        Error,
        Info,
        Debug
    }

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

    public enum FileBaseMode
    {
        Create,
        Append
    }

    public enum MongoCredentialType
    {
        MongoCr,
        ScramSha1,
        Plain
    }
}
