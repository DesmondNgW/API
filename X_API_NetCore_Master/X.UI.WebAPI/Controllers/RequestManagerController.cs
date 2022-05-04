using Microsoft.AspNetCore.Mvc;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
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

        [HttpGet(Name = "GetPublicKey")]
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            var provider = new InstanceProvider<KeyManagerService>(LogDomain.Ui);
            return CoreAccess<KeyManagerService>.TryCall<ApiResult<PublicKeyDto>, int>(provider.Client.GetPublicKey, size,
                provider, new LogOptions<ApiResult<PublicKeyDto>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "Encrypt")]
        public ApiResult<string> Encrypt(string key, string content, string nonce, int size)
        {
            var provider = new InstanceProvider<KeyManagerService>(LogDomain.Ui);
            return CoreAccess<KeyManagerService>.TryCall<ApiResult<string>, string, string, string, int>(provider.Client.Encrypt, 
                key, content, nonce, size, provider, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "Decrypt")]
        public ApiResult<string> Decrypt(string key, string content, string nonce, int size)
        {
            var provider = new InstanceProvider<KeyManagerService>(LogDomain.Ui);
            return CoreAccess<KeyManagerService>.TryCall<ApiResult<string>, string, string, string, int>(provider.Client.Decrypt,
                key, content, nonce, size, provider, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }
    }
}
