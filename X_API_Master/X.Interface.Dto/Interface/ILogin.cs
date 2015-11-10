using X.Interface.Dto.HttpRequest;
using X.Interface.Dto.HttpResponse;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 登录接口
    /// </summary>
    public interface ILogin
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginDto">登陆请求Dto</param>
        /// <returns>指定对象序列化</returns>
        ApiResult<UserDto> Login(LoginRequestDto loginDto);
    }
}
