using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpRequest;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.API.Util;
using X.Util.Core;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginController : VisitorBaseController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginDto">登陆请求Dto</param>
        /// <returns>指定对象序列化</returns>
        [HttpPost]
        public ApiResult<UserDto> Login([FromBody] LoginRequestDto loginDto)
        {
            var provider = new InstanceProvider<ILogin>(typeof(LoginService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.Login, loginDto, ControllerHelper.CallSuccess, provider.Close);
        }
    }
}