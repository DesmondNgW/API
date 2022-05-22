﻿using Microsoft.AspNetCore.Mvc;
using X.Interface.Dto;
using X.UI.Util.Controller;
using X.UI.Util.Helper;
using X.UI.Util.Model;
using X.Util.Core;
using X.Util.Extend.Cryption;
using X.Util.Other;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBaseWithOutToken
    {
        /// <summary>
        /// Md5-Test
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet(Name = "ApiMd5Test")]
        public ApiResult<string> ApiMd5Test(string text)
        {
            return new ApiResult<string>()
            {
                Data = BaseCryption.Md5Bit32(text),
                Success = true
            };
        }

        /// <summary>
        /// WebHtml-Test
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet(Name = "ApiWebHtmlTest")]
        public ApiResult<string> ApiWebHtmlTest(string url)
        {
            var cookie = string.Empty;
            return new ApiResult<string>()
            {
                Data = HttpRequestBase.GetHttpInfo(url, "utf8", string.Empty, null, cookie).ToJson(),
                Success = true
            };
        }

        /// <summary>
        /// RsaEnTest
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet(Name = "ApiRsaEnTest")]
        public ApiResult<string> ApiRsaEnTest(string text)
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            var en = RsaCryption.Encrypt(id, text, nonce);
            return new ApiResult<string>()
            {
                Data = en,
                Success = true
            };
        }

        /// <summary>
        /// RsaDeTest
        /// </summary>
        /// <param name="rsa"></param>
        /// <returns></returns>
        [HttpGet(Name = "ApiRsaDeTest")]
        public ApiResult<string> ApiRsaDeTest(string rsa)
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            var de = RsaCryption.Decrypt(id, rsa, nonce);
            return new ApiResult<string>()
            {
                Data = de,
                Success = true
            };
        }

        /// <summary>
        /// 农历
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [HttpGet(Name = "ChineseCalendarTest")]
        public ApiResult<string> ChineseCalendarTest(string dt)
        {
            var calendar = new ChineseCalendar(dt.Convert2DateTime(default));
            return new ApiResult<string>()
            {
                Data = calendar.ToJson(),
                Success = true
            };
        }

        /// <summary>
        /// GetTcpConnectionTest
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetTcpConnectionTest")]
        public ApiResult<List<TcpConnectionInfo>> GetTcpConnectionTest(string ip, int port)
        {
            return new ApiResult<List<TcpConnectionInfo>>()
            {
                Success = true,
                Data = MonitorHelper.GetTcpConnection(ip, port)
            };
        }
    }
}