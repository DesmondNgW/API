using System.Web.Http;
using Em.FundTrade.EncryptHelper;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.Interface.Other;
using X.UI.API.Util;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    public class KmsController : RequestBaseController, IKms
    {
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<DateTimeDto> Now()
        {
            var provider = new InstanceProvider<IKms>(typeof(KmsManagerService), ControllerHelper.EDomain);
            return CoreAccess<IKms>.Call(provider.Client.Now, provider, new LogOptions<ApiResult<DateTimeDto>>(ControllerHelper.CallSuccess));
        }

        /// <summary>
        /// 手机号加密
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>手机号加密结果</returns>
        [HttpGet]
        public EncryptResult MobileEncrypt(string mobile)
        {
            var provider = new InstanceProvider<IKms>(typeof(KmsManagerService), ControllerHelper.EDomain);
            return CoreAccess<IKms>.Call(provider.Client.MobileEncrypt, mobile, provider, new LogOptions<EncryptResult>(ControllerHelper.CallSuccess));
        }

        /// <summary>
        /// TestPost
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<object> TestPost([FromBody]object post)
        {
            return new ApiResult<object> {Data = post, Success = true};
        }
    }
}