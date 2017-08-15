﻿using System.Web.Http;

namespace X.UI.API.Util
{
    /// <summary>
    /// 无需登录的Controller基类
    /// </summary>
    [Request, ApiException]
    public class RequestBaseController : ApiController
    {
    }
}