using X.DB.Entitis;

namespace X.Business.Model
{
    public class User
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// UToken
        /// </summary>
        public string UToken { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// UserInfo
        /// </summary>
        public UserInfo UserInfo { get; set; }
    }
}
