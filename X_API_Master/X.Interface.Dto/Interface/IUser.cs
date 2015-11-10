using X.Interface.Dto.HttpResponse;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 用户信息接口
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="context">登陆用户请求Dto</param>
        /// <returns>指定对象序列化</returns>
        ApiResult<bool> Logout(UserRequestDtoBase context);

        /// <summary>
        /// 获取登录用户对象
        /// </summary>
        /// <param name="context">登陆用户请求Dto</param>
        /// <returns>指定对象序列化</returns>
        ApiResult<UserDto> GetUserInfo(UserRequestDtoBase context);
    }
}
