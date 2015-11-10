using System;

namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// 路由对象Dto
    /// </summary>
    public class RouteInfoDto
    {
        /// <summary>
        /// 证件号
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// CustomerNo
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EastmoneyPassport { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// MobilePhone
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// UpdateTime
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// Zone
        /// </summary>
        public int Zone { get; set; }
    }
}
