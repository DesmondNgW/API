using Microsoft.AspNetCore.Mvc;
using X.UI.Util.Controller;
using X.Util.Other;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageManagerController : ControllerBaseWithOutToken
    {
        [HttpGet(Name = "VerifyCode")]
        public void VerifyCode()
        {
            var vc = CaptchaHelper.CaptchaOptions();
            var result = CaptchaHelper.GetCaptchaByte(vc);
            Response.ContentType = "image/png";
            Response.StatusCode = 200;
            Response.Headers.Add("vc", CaptchaHelper.GetCaptchaCode("VerifyCode", vc.Value));
            Response.Body.WriteAsync(result, 0, result.Length);
        }

        [HttpGet(Name = "BitMap")]
        public void BitMap()
        {
            var result = CaptchaHelper.GetBitMapByte();
            Response.ContentType = "image/png";
            Response.StatusCode = 200;
            Response.Body.WriteAsync(result, 0, result.Length);
        }

        [HttpGet(Name = "TextImage")]
        public void TextImage(string value)
        {
            var opt = CaptchaHelper.TextImageOptions();
            opt.Value = value;
            var result = CaptchaHelper.GetTextImageByte(opt);
            Response.ContentType = "image/png";
            Response.StatusCode = 200;
            Response.Headers.Add("opt", CaptchaHelper.GetCaptchaCode("TextImage", opt.Value));
            Response.Body.WriteAsync(result, 0, result.Length);
        }
    }
}
