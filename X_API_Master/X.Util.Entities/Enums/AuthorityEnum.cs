namespace X.Util.Entities.Enums
{
    public enum OperateTargetCode
    {
        Default = 0
    }

    /// <summary>
    /// 操作代码
    /// </summary>
    public enum OperateCode
    {
        None = 0,
        Create = 1,
        Update = 2,
        Retrieve = 3,
        Delete = 4,
        ConfirmCreate = 101,
        ConfirmUpdate = 102,
        ConfirmRetrieve = 103,
        ConfirmDelete = 104,
    }
}
