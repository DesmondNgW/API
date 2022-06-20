using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.Interface.Dto;
using X.UI.Util.Controller;

namespace X.UI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBaseWithOutToken
    {
        public async Task Test(string text)
        {
            await Task.Run(() => { });
        }
    }
}
