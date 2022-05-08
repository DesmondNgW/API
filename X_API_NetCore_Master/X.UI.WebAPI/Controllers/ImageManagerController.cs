using Microsoft.AspNetCore.Mvc;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.Util.Controller;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageManagerController : ControllerBaseWithOutFilter
    {
        [HttpGet(Name = "VerifyCode")]
        public HttpResponseMessage VerifyCode()
        {
            var provider = new InstanceProvider<ImageManagerService>(LogDomain.Ui);
            return CoreAccess<IImageManager>.TryCall<HttpResponseMessage>(provider.Client.VerifyCode, provider, 
                new LogOptions<HttpResponseMessage>(CoreBase.CallSuccess));
        }

        [HttpGet(Name = "BitMap")]
        public HttpResponseMessage BitMap()
        {
            var provider = new InstanceProvider<ImageManagerService>(LogDomain.Ui);
            return CoreAccess<IImageManager>.TryCall<HttpResponseMessage>(provider.Client.BitMap, provider,
                new LogOptions<HttpResponseMessage>(CoreBase.CallSuccess));
        }

        [HttpGet(Name = "TextImage")]
        public HttpResponseMessage TextImage(string value)
        {
            var provider = new InstanceProvider<ImageManagerService>(LogDomain.Ui);
            return CoreAccess<IImageManager>.TryCall<HttpResponseMessage, string>(provider.Client.TextImage, value, provider,
                new LogOptions<HttpResponseMessage>(CoreBase.CallSuccess));
        }
    }
}
