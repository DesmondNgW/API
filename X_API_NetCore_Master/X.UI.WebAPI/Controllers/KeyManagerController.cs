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
    public class KeyManagerController : ControllerBaseWithToken
    {
        [HttpGet(Name = "GetPublicKey")]
        public ApiResult<PublicKeyDto> GetPublicKey(int size)
        {
            var provider = new InstanceProvider<KeyManagerService>(LogDomain.Ui);
            return CoreAccess<IKeyManager>.TryCall<ApiResult<PublicKeyDto>, int>(provider.Client.GetPublicKey, size,
                provider, new LogOptions<ApiResult<PublicKeyDto>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "Encrypt")]
        public ApiResult<string> Encrypt(string key, string content, string nonce, int size)
        {
            var provider = new InstanceProvider<KeyManagerService>(LogDomain.Ui);
            return CoreAccess<IKeyManager>.TryCall<ApiResult<string>, string, string, string, int>(provider.Client.Encrypt, 
                key, content, nonce, size, provider, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }

        [HttpGet(Name = "Decrypt")]
        public ApiResult<string> Decrypt(string key, string content, string nonce, int size)
        {
            var provider = new InstanceProvider<KeyManagerService>(LogDomain.Ui);
            return CoreAccess<IKeyManager>.TryCall<ApiResult<string>, string, string, string, int>(provider.Client.Decrypt,
                key, content, nonce, size, provider, new LogOptions<ApiResult<string>>(CoreService.CallSuccess));
        }
    }
}
