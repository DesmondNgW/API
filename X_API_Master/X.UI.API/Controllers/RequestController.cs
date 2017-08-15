﻿using System.Drawing;
using System.Web;
using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.Interface;
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
    public class RequestController : RequestBaseController, IRequestManager
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="ip"></param>
        /// <param name="userAgent"></param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<string> GetToken(string clientId, string ip, string userAgent)
        {
            if (string.IsNullOrEmpty(ip)) ip = IpBase.GetIp();
            if (string.IsNullOrEmpty(userAgent)) userAgent = new HttpContextWrapper(HttpContext.Current).Request.UserAgent;
            var provider = new InstanceProvider<IRequestManager>(typeof(RequestManagerService), ControllerHelper.EDomain);
            return CoreAccess<IRequestManager>.Call(provider.Client.GetToken, clientId, ip, userAgent, provider, new LogOptions<ApiResult<string>>(ControllerHelper.CallSuccess));
        }

        /// <summary>
        /// GetTimestamp
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<string> GetTimestamp()
        {
            var provider = new InstanceProvider<IRequestManager>(typeof(RequestManagerService), ControllerHelper.EDomain);
            return CoreAccess<IRequestManager>.Call(provider.Client.GetTimestamp, provider, new LogOptions<ApiResult<string>>(ControllerHelper.CallSuccess));
        }
    }
}