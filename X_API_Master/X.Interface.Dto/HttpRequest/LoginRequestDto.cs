namespace X.Interface.Dto.HttpRequest
{
    /// <summary>
    /// 登陆请求Dto
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码（RSA加密）
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// RSa加密的key
        /// </summary>
        public string RsaKey { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Nonce { get; set; }
        /// <summary>
        /// RSa秘钥大小
        /// </summary>
        public int Size { get; set; }
    }
}
