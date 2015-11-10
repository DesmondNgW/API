using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.API.Util;
using X.Util.Core;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserController : UserBaseController
    {
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="context">登陆用户请求Dto</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<bool> Logout([FromUri] UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<IUser>(typeof(CustomerService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.Logout, context, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取登录用户对象
        /// </summary>
        /// <param name="context">登陆用户请求Dto</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<UserDto> GetUserInfo([FromUri] UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<IUser>(typeof(CustomerService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetUserInfo, context, ControllerHelper.CallSuccess, provider.Close);
        }
    }
}
