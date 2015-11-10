using X.Interface.Dto.HttpResponse;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 秘钥接口
    /// </summary>
    public interface IKeyManager
    {
        /// <summary>
        /// 获取指定长度的RSA公钥(一般1024)
        /// </summary>
        /// <param name="size">秘钥长度</param>
        /// <returns>指定对象序列化</returns>
        ApiResult<PublicKeyDto> GetPublicKey(int size);

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns>指定对象序列化</returns>
        ApiResult<string> GetToken();
    }
}
