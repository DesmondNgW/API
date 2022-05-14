using System;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Core.Kernel;

namespace X.Interface.Core
{
    public class RequestManagerService : IRequestManager
    {
        public ApiResult<string> GetToken(string clientId, string ip)
        {
            var userAgent = ServiceCollectionHelper.Current.Request.Headers["User-Agent"];
            return new ApiResult<string> { Success = true, Data = TokenHelper.GenerateToken(clientId, ip, userAgent) };
        }

        public ApiResult<string> GetTimestamp()
        {
            return new ApiResult<string> { Success = true, Data = DateTime.Now.GetMilliseconds().ToString() };
        }

        public ApiResult<RequestPreDto> GetRequestPre(string clientId, string ip)
        {
            var userAgent = ServiceCollectionHelper.Current.Request.Headers["User-Agent"];
            return new ApiResult<RequestPreDto>()
            {
                Success = true,
                Data = new RequestPreDto()
                {
                    Timestamp = DateTime.Now.GetMilliseconds(),
                    Token = TokenHelper.GenerateToken(clientId, ip, userAgent),
                    Version = "1.0.0"
                }
            };
        }
    }
}
