using Microsoft.AspNetCore.Mvc;
using X.UI.Util.Controller;
using X.UI.Util.Helper;
using X.Util.Other;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageManagerController : ControllerBaseWithOutToken
    {
        [HttpGet(Name = "VerifyCode")]
        public async void VerifyCode()
        {
            var vc = CaptchaHelper.CaptchaOptions();
            var result = CaptchaHelper.GetCaptchaByte(vc);
            await ResponseHelper.ResponseWrite(Response, "image/png", result, "vc", CaptchaHelper.GetCaptchaCode("VerifyCode", vc.Value));
        }

        [HttpGet(Name = "BitMap")]
        public async void BitMap()
        {
            var result = CaptchaHelper.GetBitMapByte();
            await ResponseHelper.ResponseWrite(Response, "image/png", result);
        }

        [HttpGet(Name = "TextImage")]
        public async void TextImage(string value)
        {
            var opt = CaptchaHelper.TextImageOptions();
            opt.Value = value;
            var result = CaptchaHelper.GetTextImageByte(opt);
            await ResponseHelper.ResponseWrite(Response, "image/png", result, "opt", CaptchaHelper.GetCaptchaCode("TextImage", opt.Value));
        }
    }
}
