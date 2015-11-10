using System;
using System.IO;
using System.Web.Http;
using X.Interface.Dto;
using X.Interface.Dto.HttpRequest;
using X.Interface.Other;
using X.UI.API.Util;
using X.Util.Core;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    public class ManagerController : UserBaseController
    {
        [HttpPost]
        public ApiResult<bool> DeleteLog([FromBody] DeleteLogFilesRequestDto dto)
        {
            var provider = new InstanceProvider<ManagerService>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/../", "XApiLog");
            CoreAccess.CallAsync(ControllerHelper.EDomain, provider.Instance.DeleteHistoryLog, dto, path, provider.Close, null);
            return new ApiResult<bool> { Success = true, Data = true };
        }

        [HttpPost]
        public ApiResult<bool> ClearCustomerCache([FromBody] UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<ManagerService>();
            CoreAccess.CallAsync(ControllerHelper.EDomain, provider.Instance.ClearCustomerCache, context, provider.Close, null);
            return new ApiResult<bool> { Success = true, Data = true };
        }

        [HttpPost]
        public ApiResult<bool> ClearFundCache([FromBody] UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<ManagerService>();
            CoreAccess.CallAsync(ControllerHelper.EDomain, provider.Instance.ClearFundCache, context, provider.Close, null);
            return new ApiResult<bool> { Success = true, Data = true };
        }

        [HttpPost]
        public ApiResult<bool> ClearRouterCache([FromBody] UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<ManagerService>();
            CoreAccess.CallAsync(ControllerHelper.EDomain, provider.Instance.ClearCustomerCache, context, provider.Close, null);
            return new ApiResult<bool> { Success = true, Data = true };
        }

        [HttpPost]
        public ApiResult<bool> ClearWhiteListCache([FromBody] UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<ManagerService>();
            CoreAccess.CallAsync(ControllerHelper.EDomain, provider.Instance.ClearWhiteListCache, context, provider.Close, null);
            return new ApiResult<bool> { Success = true, Data = true };
        }
    }
}
