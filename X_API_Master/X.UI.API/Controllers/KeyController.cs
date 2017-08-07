using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
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
    public class KeyController : ApiController, IKeyManager
    {
        /// <summary>
        /// 获取指定长度的RSA公钥(一般1024)
        /// </summary>
        /// <param name="size">秘钥长度</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            var provider = new InstanceProvider<IKeyManager>(typeof(KeyManagerService), ControllerHelper.EDomain);
            return CoreAccess<IKeyManager>.Call(provider.Client.GetPublicKey, size, provider, new LogOptions<ApiResult<PublicKeyDto>>(ControllerHelper.CallSuccess));
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<string> GetToken(string clientId)
        {
            var provider = new InstanceProvider<IKeyManager>(typeof(KeyManagerService), ControllerHelper.EDomain);
            return CoreAccess<IKeyManager>.Call(provider.Client.GetToken, clientId, provider, new LogOptions<ApiResult<string>>(ControllerHelper.CallSuccess));
        }
    }
}