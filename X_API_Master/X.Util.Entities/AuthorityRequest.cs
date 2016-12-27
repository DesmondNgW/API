using X.Util.Entities.Enums;

namespace X.Util.Entities
{
    /// <summary>
    /// {UserId} {ModuleOperateCode} {Module}
    /// {UserId} {ScopeOperateCode} {Scope}
    /// </summary>
    public class AuthorityRequest
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// ModuleId 模块Id
        /// </summary>
        public Module Module { get; set; }

        /// <summary>
        /// 操作代码
        /// </summary>
        public ModuleOperateCode ModuleOperateCode { get; set; }
    }
}
