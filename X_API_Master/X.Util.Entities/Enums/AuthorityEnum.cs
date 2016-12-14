namespace X.Util.Entities.Enums
{
    /// <summary>
    /// 权限级别，高级别权限包含低级别所有权限
    /// </summary>
    public enum AuthorityLevel
    {
        BelowLow = 1,
        Low = 2,
        BelowNormal = 3,
        Normal = 4,
        Default = 4,
        AboveNormal = 5,
        High = 6,
        AboveHigh = 7
    }

    /// <summary>
    /// 权限类型，同级别权限根据权限类型区分
    /// </summary>
    public enum AuthorityType
    {
        Query = 1,
        Operate = 2,
        Audit = 3,
    }
}
