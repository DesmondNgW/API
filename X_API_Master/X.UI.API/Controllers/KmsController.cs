using System;
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
    /// 秘钥管理
    /// </summary>
    [ApiException]
    public class KmsController : ApiController
    {
        /// <summary>
        /// 获取指定长度的RSA公钥(一般1024)
        /// </summary>
        /// <param name="size">秘钥长度</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            var provider = new InstanceProvider<IKeyManager>(typeof(KeyManagerService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetPublicKey, size, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<string> GetToken()
        {
            var provider = new InstanceProvider<IKeyManager>(typeof(KeyManagerService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetToken, ControllerHelper.CallSuccess, provider.Close);
        }

        [HttpGet]
        public ApiResult<DateTime> Now()
        {
            return new ApiResult<DateTime> {Success = true, Data = DateTime.Now};
        }
    }
}