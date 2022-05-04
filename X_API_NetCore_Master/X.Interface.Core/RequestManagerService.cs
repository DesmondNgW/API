using System;
using X.Business.Util;
using X.Interface.Dto;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Core.Kernel;

namespace X.Interface.Core
{
    public class RequestManagerService : IRequestManager
    {
        public ApiResult<string> GetToken(string clientId, string ip)
        {
            var userAgent = HttpContextHelper.Current.Request.Headers["User-Agent"];
            return new ApiResult<string> { Success = true, Data = TokenHelper.GenerateToken(clientId, ip, userAgent) };
        }

        public ApiResult<string> GetTimestamp()
        {
            return new ApiResult<string> { Success = true, Data = DateTime.Now.GetMilliseconds().ToString() };
        }
    }
}
