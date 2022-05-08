using X.Util.Entities;

namespace X.DB.Entitis
{
    public class UserInfo : MongoBaseModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 绑定手机
        /// </summary>
        public bool BindTelephone { get; set; }

        /// <summary>
        /// BindEmail
        /// </summary>
        public bool BindEmail { get; set; }

        /// <summary>
        /// isEnable
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
