using X.Util.Entities.Enums;

namespace X.Util.Entities
{
    /// <summary>
    /// {UserId} {OperateCode} {TargetCode} at {ModuleId}
    /// </summary>
    public class AuthorityRequest
    {
        public string UserId { get; set; }
        
        public string ModuleId { get; set; }

        public OperateTargetCode TargetCode { get; set; }

        public OperateCode AuthorityCode { get; set; }
    }
}
