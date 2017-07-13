namespace X.Util.Entities.Enums
{
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

    /// <summary>
    /// 处理模式（普通、定时）
    /// </summary>
    public enum ProcessingMode
    {
        Common,
        Timer
    }
}
