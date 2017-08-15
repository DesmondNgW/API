using System;
using System.Web;
using X.Business.Helper;
using X.Interface.Dto;
using X.Interface.Dto.Interface;
using X.Util.Core;

namespace X.Interface.Core
{
    public class RequestManagerService : IRequestManager
    {
        public ApiResult<string> GetToken(string clientId, string ip)
        {
            var userAgent = new HttpContextWrapper(HttpContext.Current).Request.UserAgent;
            return new ApiResult<string> { Success = true, Data = TokenHelper.GenerateToken(clientId, ip, userAgent) };
        }

        public ApiResult<string> GetTimestamp()
        {
            return new ApiResult<string> { Success = true, Data = DateTime.Now.GetMilliseconds().ToString() };
        }
    }
}
