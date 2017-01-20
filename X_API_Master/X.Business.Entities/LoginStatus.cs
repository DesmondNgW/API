namespace X.Business.Entities
{
    /// <summary>
    /// LoginStatus
    /// </summary>
    public class LoginStatus
    {
        /// <summary>
        /// 用户标示
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 用户所属分区
        /// </summary>
        public int Zone { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 用户状态存储的分区
        /// </summary>
        public int StatusZone { get; set; }
    }
}
