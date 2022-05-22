using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using X.Interface.Dto;
using X.UI.Util.Controller;
using X.Util.Core;
using X.Util.Core.Kernel;
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
        public ApiResult<List<Tuple<string, string, string>>> GetTcpConnectionTest(string ip, int port)
        {
            var ret = new ApiResult<List<Tuple<string, string, string>>>()
            {
                Success = true,
                Data = new List<Tuple<string, string, string>>()
            };
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveTcpConnections();
            foreach (var t in connections)
            {
                if (IpBase.IpValid(ip) && !t.RemoteEndPoint.Address.ToString().Contains(ip)) continue;
                if (port > 0 && t.RemoteEndPoint.Port != port) continue;
                ret.Data.Add(new Tuple<string, string, string>(t.LocalEndPoint.ToString(), t.RemoteEndPoint.ToString(),
                    t.State.ToString()));
            }
            return ret;
        }
    }
}
