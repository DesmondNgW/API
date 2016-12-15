using X.Util.Entities.Enums;

namespace X.Util.Entities
{
    /// <summary>
    /// {UserId} {OperateCode} {TargetCode} at {ModuleId}
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
        /// 模块下操作对像
        /// </summary>
        public OperateTargetCode TargetCode { get; set; }

        /// <summary>
        /// 操作代码
        /// </summary>
        public OperateCode OperateCode { get; set; }
    }
}
