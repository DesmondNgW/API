﻿using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Em.FundTrade.EncryptHelper;
using ThoughtWorks.QRCode.Codec;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.API.Util;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Extend.Core;
using X.Util.Other;
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

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<DateTime> Now()
        {
            return new ApiResult<DateTime> {Success = true, Data = DateTime.Now};
        }

        /// <summary>
        /// 验证码图片
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
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

        /// <summary>
        /// 1*1像素位图
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage BitMap()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(CaptchaHelper.GetBitMapByte())
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;
        }

        /// <summary>
        /// 文本转图像
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage TextImage()
        {
            var opt = CaptchaHelper.TextImageOptions();
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(CaptchaHelper.GetTextImageByte(opt))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            result.Content.Headers.Add("opt", CaptchaHelper.GetCaptchaCode("TextImage", opt.Value));
            return result;
        }

        /// <summary>
        /// 二维码
        /// </summary>
        /// <returns>二进制字节</returns>
        [HttpGet]
        public HttpResponseMessage QrCode()
        {
            var qrCodeEncoder = new QRCodeEncoder();
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(QrCodeHelper.Encoder(qrCodeEncoder, "1234567890", Encoding.UTF8))
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;
        }

        /// <summary>
        /// 手机号加密
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>手机号加密结果</returns>
        [HttpGet]
        public EncryptResult MobileEncrypt(string mobile)
        {
            var provider = new InstanceProvider<EncryptHelper>(ControllerHelper.EDomain);
            return CoreAccess<EncryptHelper>.Call(EncryptHelper.Instance.MobileEncrypt, mobile, provider, new LogOptions<EncryptResult>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// WeekOfYear
        /// </summary>
        /// <returns>WeekOfYear</returns>
        [HttpGet]
        public ApiResult<int> WeekOfYear()
        {
            var gc = new GregorianCalendar();
            var weekOfYear = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            return new ApiResult<int> {Success = true, Data = weekOfYear};
        }

        /// <summary>
        /// DayOfYear
        /// </summary>
        /// <returns>DayOfYear</returns>
        [HttpGet]
        public ApiResult<int> DayOfYear()
        {
            return new ApiResult<int> { Success = true, Data = DateTime.Now.DayOfYear };
        }
    }
}