using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.Interface;
using X.UI.API.Util;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    public class UserController : UserBaseController, IUserManager
    {
        /// <summary>
        /// GetApiRequestContext
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<ApiRequestContext> GetApiRequestContext()
        {
            var provider = new InstanceProvider<IUserManager>(typeof(UserManagerService), ControllerHelper.EDomain);
            return CoreAccess<IUserManager>.Call(provider.Client.GetApiRequestContext, provider, new LogOptions<ApiResult<ApiRequestContext>>(ControllerHelper.CallSuccess));
        }
    }
}