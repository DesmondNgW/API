using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using X.Interface.Dto.Interface;
using X.Util.Other;

namespace X.Interface.Other
{
    public class ImageManagerService : IImageManager
    {
        public HttpResponseMessage VerifyCode()
        {
            var vc = CaptchaHelper.CaptchaOptions();
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(CaptchaHelper.GetCaptchaByte(vc))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            result.Content.Headers.Add("vc", CaptchaHelper.GetCaptchaCode("VerifyCode", vc.Value));
            return result;
        }

        public HttpResponseMessage BitMap()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(CaptchaHelper.GetBitMapByte())
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;
        }

        public HttpResponseMessage TextImage(string value)
        {
            var opt = CaptchaHelper.TextImageOptions();
            opt.Value = value;
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(CaptchaHelper.GetTextImageByte(opt))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            result.Content.Headers.Add("opt", CaptchaHelper.GetCaptchaCode("TextImage", opt.Value));
            return result;
        }

        public HttpResponseMessage QrCode(string value)
        {
            var qrCodeEncoder = new QRCodeEncoder();
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(QrCodeHelper.Encoder(qrCodeEncoder, value, Encoding.UTF8))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;
        }
    }
}
