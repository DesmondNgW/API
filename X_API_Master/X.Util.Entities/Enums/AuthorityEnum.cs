namespace X.Util.Entities.Enums
{
    /// <summary>
    /// 模块操作代码
    /// </summary>
    public enum ModuleOperateCode
    {
        Create = 1,
        Update = 2,
        Retrieve = 3,
        Delete = 4,
        ConfirmCreate = 101,
        ConfirmUpdate = 102,
        ConfirmRetrieve = 103,
        ConfirmDelete = 104
    }

    /// <summary>
    /// 作用域操作代码
    /// </summary>
    public enum ScopeOperateCode
    {
        None = 0,
        Access = 1
    }
}
