﻿using Microsoft.AspNetCore.Mvc;
using X.Interface.Core;
using X.Interface.Dto;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Provider;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RequestManagerController : ControllerBase
    {
        [HttpGet(Name = "GetTimestamp")]
        public ApiResult<string> GetTimestamp()
        {
            var provider = new InstanceProvider<RequestManagerService>(LogDomain.Ui);
            return CoreAccess<RequestManagerService>.TryCall<ApiResult<string>>(provider.Client.GetTimestamp,
                provider, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "GetToken")]
        public ApiResult<string> GetToken(string clientId, string ip)
        {
            var provider = new InstanceProvider<RequestManagerService>(LogDomain.Ui);
            return CoreAccess<RequestManagerService>.TryCall<ApiResult<string>, string, string>(provider.Client.GetToken, 
                clientId, ip,provider, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }
    }
}
