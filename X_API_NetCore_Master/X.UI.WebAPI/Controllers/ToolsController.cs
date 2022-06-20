using Microsoft.AspNetCore.Mvc;
using X.Interface.Dto;
using X.Interface.Dto.HttpRequest;
using X.UI.Util;
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
    public class ToolsController : ControllerBaseWithOutToken
    {
        /// <summary>
        /// Md5-Test
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet(Name = "Md5")]
        public async Task<ApiResult<string>> Md5(string text)
        {
            return await Task.Run(() => new ApiResult<string>()
            {
                Data = BaseCryption.Md5Bit32(text),
                Success = true
            });
        }

        /// <summary>
        /// WebHtml-Test
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet(Name = "WebHtml")]
        public async Task<ApiResult<string>> WebHtml(string url)
        {
            var cookie = string.Empty;
            return await Task.Run(() => new ApiResult<string>()
            {
                Data = HttpRequestBase.GetHttpInfo(url, "utf8", string.Empty, null, cookie).ToJson(),
                Success = true
            });
        }

        /// <summary>
        /// RsaEnTest
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet(Name = "RsaEncrypt")]
        public async Task<ApiResult<string>> RsaEncrypt(string text)
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            return await Task.Run(() => new ApiResult<string>()
            {
                Data = RsaCryption.Encrypt(id, text, nonce),
                Success = true
            });
        }

        /// <summary>
        /// RsaDeTest
        /// </summary>
        /// <param name="rsa"></param>
        /// <returns></returns>
        [HttpGet(Name = "RsaDecrypt")]
        public async Task<ApiResult<string>> RsaDecrypt(string rsa)
        {
            const string id = "RSATest";
            const string nonce = "RSATestNonce";
            return await Task.Run(() => new ApiResult<string>()
            {
                Data = RsaCryption.Decrypt(id, rsa, nonce),
                Success = true
            });
        }

        /// <summary>
        /// 农历
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [HttpGet(Name = "ChineseCalendar")]
        public async Task<ApiResult<string>> ChineseCalendar(string dt)
        {
            return await Task.Run(() => new ApiResult<string>()
            {
                Data = new ChineseCalendar(dt.Convert2DateTime(default)).ToJson(),
                Success = true
            });
        }

        /// <summary>
        /// GetTcpConnectionTest
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetTcpConnection")]
        public async Task<ApiResult<List<TcpConnectionInfo>>> GetTcpConnection(string ip, int port)
        {
            return await Task.Run(() => new ApiResult<List<TcpConnectionInfo>>()
            {
                Success = true,
                Data = MonitorHelper.GetTcpConnection(ip, port)
            });
        }

        /// <summary>
        /// GetRealPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetRealPath")]
        public async Task<ApiResult<string>> GetRealPath(string path)
        {
            return await Task.Run(() => new ApiResult<string>()
            {
                Success = true,
                Data = ApiData.GetRealPath(path)
            });
        }

        /// <summary>
        /// PostContent
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns></returns>
        [HttpPost(Name = "PostContent")]
        public async Task<ApiResult<string>> PostContent(ApiRequestDto requestDto)
        {
            return await Task.Run(() => new ApiResult<string>()
            {
                Success = true,
                Data = ApiData.PostContentByJson(requestDto.Uri, requestDto.Postdata.GetJsonByJsonDocument(), "application/json", requestDto.ExtendHeaders)
            });
        }

        ///// <summary>
        ///// GetContent
        ///// </summary>
        ///// <param name="requestDto"></param>
        ///// <returns></returns>
        [HttpPost(Name = "GetContent")]
        public async Task<ApiResult<string>> GetContent(ApiRequestDto requestDto)
        {
            return await Task.Run(() => new ApiResult<string>()
            {
                Success = true,
                Data = ApiData.GetContent(requestDto.Uri, requestDto.Arguments, "application/json", requestDto.ExtendHeaders)
            });
        }

        /// <summary>
        /// GetChinese
        /// </summary>
        /// <param name="chinese"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetChinese")]
        public async Task<ApiResult<List<string>>> GetChinese(string chinese)
        {
            return await Task.Run(() => new ApiResult<List<string>>()
            {
                Success = true,
                Data = ChineseConvert.Get(chinese)
            });
        }
    }
}
