using System.Net.Http;
using System.Web.Http;
using X.Interface.Dto.Interface;
using X.Interface.Other;
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
    public class ImageController : ApiController
    {
        /// <summary>
        /// 验证码图片
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage VerifyCode()
        {
            var provider = new InstanceProvider<IImageManager>(typeof(ImageManagerService), ControllerHelper.EDomain);
            return CoreAccess<IImageManager>.Call(provider.Client.VerifyCode, provider, new LogOptions<HttpResponseMessage>(ControllerHelper.CallSuccess, false));
        }

        /// <summary>
        /// 1*1像素位图
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage BitMap()
        {
            var provider = new InstanceProvider<IImageManager>(typeof(ImageManagerService), ControllerHelper.EDomain);
            return CoreAccess<IImageManager>.Call(provider.Client.BitMap, provider, new LogOptions<HttpResponseMessage>(ControllerHelper.CallSuccess, false));
        }

        /// <summary>
        /// 文本转图像
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage TextImage(string value)
        {
            var provider = new InstanceProvider<IImageManager>(typeof(ImageManagerService), ControllerHelper.EDomain);
            return CoreAccess<IImageManager>.Call(provider.Client.TextImage, value, provider, new LogOptions<HttpResponseMessage>(ControllerHelper.CallSuccess, false));
        }

        /// <summary>
        /// 二维码
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage QrCode(string value)
        {
            var provider = new InstanceProvider<IImageManager>(typeof(ImageManagerService), ControllerHelper.EDomain);
            return CoreAccess<IImageManager>.Call(provider.Client.QrCode, value, provider, new LogOptions<HttpResponseMessage>(ControllerHelper.CallSuccess, false));

        }
    }
}