namespace X.Util.Entities
{
    public enum EnumClientType
    {
        Web = 0,
        Tablet = 1,
        Mobile = 2,
        ThirdParty = 3
    }

    /// <summary>
    /// 请求上下文
    /// </summary>
    public class BusinessRequestContext
    {
        #region 来自Context
        /// <summary>
        /// 
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UToken { get; set; }
        /// <summary>
        /// API版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        public EnumClientType ClientType { get; set; }

        /// <summary>
        /// ApiRequestContext
        /// </summary>
        public string ApiRequestContext { get; set; }

        /// <summary>
        /// Current Cache token key
        /// </summary>
        public string Ctoken { get; set; }

        /// <summary>
        /// Prev Cache token key
        /// </summary>
        public string Ptoken { get; set; }

        /// <summary>
        /// Last Thread Id 
        /// </summary>
        public string Cid { get; set; }
        #endregion

        #region 用户信息相关
        /// <summary>
        /// 用户编号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string CustomerName { get; set; }
        #endregion
    }
}
