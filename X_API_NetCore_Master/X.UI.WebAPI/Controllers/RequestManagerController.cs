using Microsoft.AspNetCore.Mvc;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.Util.Controller;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Provider;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RequestManagerController : ControllerBaseWithOutToken
    {
        [HttpGet(Name = "GetTimestamp")]
        public async Task<ApiResult<string>> GetTimestamp()
        {
            var provider = new InstanceProvider<RequestManagerService>(LogDomain.Ui);
            return await CoreAccess<IRequestManager>.TryCallAsync<ApiResult<string>>(provider.Client.GetTimestamp,
                provider, null, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "GetToken")]
        public async Task<ApiResult<string>> GetToken(string clientId, string ip)
        {
            var provider = new InstanceProvider<RequestManagerService>(LogDomain.Ui);
            return await CoreAccess<IRequestManager>.TryCallAsync<ApiResult<string>, string, string>(provider.Client.GetToken,
                clientId, ip, provider, null, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "GetRequestPre")]
        public async Task<ApiResult<RequestPreDto>> GetRequestPre(string clientId, string ip)
        {
            var provider = new InstanceProvider<RequestManagerService>(LogDomain.Ui);
            return await CoreAccess<IRequestManager>.TryCallAsync<ApiResult<RequestPreDto>, string, string>(provider.Client.GetRequestPre,
                clientId, ip, provider, null, new LogOptions<ApiResult<RequestPreDto>>(CoreService.CallSuccess));
        }
    }
}
