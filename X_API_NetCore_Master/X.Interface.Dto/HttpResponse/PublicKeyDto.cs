namespace X.Interface.Dto.HttpResponse
{
    /// <summary>
    /// RSA公钥Dto
    /// </summary>
    public class PublicKeyDto
    {
        /// <summary>
        /// RSA公钥参数Exponent
        /// </summary>
        public string Exponent { get; set; }

        /// <summary>
        /// RSA公钥参数Modulus
        /// </summary>
        public string Modulus { get; set; }

        /// <summary>
        /// RSA秘钥key
        /// </summary>
        public string RsaKey { get; set; }

        /// <summary>
        /// 秘钥大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Nonce { get; set; }
    }
}
