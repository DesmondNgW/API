using Microsoft.AspNetCore.Mvc;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
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
            var provider = new InstanceProvider<RequestManagerService>();
            return provider.Client.GetTimestamp();
        }

        [HttpGet(Name = "GetToken")]
        public ApiResult<string> GetToken(string clientId, string ip)
        {
            var provider = new InstanceProvider<RequestManagerService>();
            return provider.Client.GetToken(clientId, ip);
        }

        [HttpGet(Name = "GetPublicKey")]
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            var provider = new InstanceProvider<KeyManagerService>();
            return provider.Client.GetPublicKey(size);
        }


    }
}
