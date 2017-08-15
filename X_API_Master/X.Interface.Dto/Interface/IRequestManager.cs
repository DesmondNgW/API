namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 秘钥接口
    /// </summary>
    public interface IRequestManager
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="clientId">秘钥长度</param>
        /// <param name="ip"></param>
        /// <returns>指定对象序列化</returns>
        ApiResult<string> GetToken(string clientId, string ip);

        /// <summary>
        /// GetTimestamp
        /// </summary>
        /// <returns></returns>
        ApiResult<string> GetTimestamp();
    }
}
