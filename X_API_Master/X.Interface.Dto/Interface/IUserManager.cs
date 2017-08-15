using X.Interface.Dto.HttpResponse;
using X.Util.Entities;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// GetApiRequestContext
        /// </summary>
        /// <returns>指定对象序列化</returns>
        ApiResult<ApiRequestContext> GetApiRequestContext();
    }
}
