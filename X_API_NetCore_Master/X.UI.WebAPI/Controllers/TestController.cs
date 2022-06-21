using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.Interface.Dto;
using X.UI.Util.Controller;
using X.UI.WebAPI.Test;
using System.Linq;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBaseWithOutToken
    {
        [HttpGet(Name = "Test")]
        public async Task<object> Test(EnumTestMethodItem methodItem)
        {
            return await Task.Run(() =>
            {
                object? ret = default;
                foreach (var methodInfo in from item in ((EnumTestMethodItem[])Enum.GetValues(typeof(EnumTestMethodItem))).Where(item => item == methodItem)
                                           let methodInfo = typeof(TestMethodHelper).GetMethod(item.ToString())
                                           select methodInfo)
                {
                    ret = methodInfo?.Invoke(null, null);
                    break;
                }
                return (ret != default) ? ret : throw new ArgumentOutOfRangeException(nameof(methodItem));
            });
        }
    }
}
