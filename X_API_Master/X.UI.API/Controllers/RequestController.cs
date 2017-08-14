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
    /// <summary>
    /// 秘钥管理
    /// </summary>
    [ApiException]
    public class RequestController : ApiController, IRequestManager
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<string> GetToken(string clientId)
        {
            var provider = new InstanceProvider<IRequestManager>(typeof(RequestManagerService), ControllerHelper.EDomain);
            return CoreAccess<IRequestManager>.Call(provider.Client.GetToken, clientId, provider, new LogOptions<ApiResult<string>>(ControllerHelper.CallSuccess));
        }

        /// <summary>
        /// GetTimestamp
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<string> GetTimestamp()
        {
            var provider = new InstanceProvider<IRequestManager>(typeof(RequestManagerService), ControllerHelper.EDomain);
            return CoreAccess<IRequestManager>.Call(provider.Client.GetTimestamp, provider, new LogOptions<ApiResult<string>>(ControllerHelper.CallSuccess));
        }
    }
}