using System;

namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 账户状态
    /// </summary>
    public enum EnumAccountState
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Applying
        /// </summary>
        Applying = 1,
        /// <summary>
        /// ApplyFailed
        /// </summary>
        ApplyFailed = 2,
        /// <summary>
        /// Frozen
        /// </summary>
        Frozen = 3,
        /// <summary>
        /// Lost
        /// </summary>
        Lost = 4,
        /// <summary>
        /// Closed
        /// </summary>
        Closed = 5,
        /// <summary>
        /// Driblet
        /// </summary>
        Driblet = 6
    }

    /// <summary>
    /// 用户响应Dto
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Uid
        /// </summary>
        public string Uid { get; set; }
        
        /// <summary>
        /// 账户登录次数
        /// </summary>
        public int AccLoginTimes { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public EnumAccountState AccountState { get; set; }
        
        /// <summary>
        /// 加密证件号
        /// </summary>
        public string CertificateNoCrypt { get; set; }
        
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 证件到期时间
        /// </summary>
        public DateTime? IdEndDate { get; set; }
        
        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        
        /// <summary>
        /// 开户时间
        /// </summary>
        public DateTime? OpenTime { get; set; }
        
        /// <summary>
        /// 风险等级
        /// </summary>
        public string Risk { get; set; }

        /// <summary>
        /// 风险等级名称
        /// </summary>
        public string RiskName { get; set; }
    }
}
