using X.Interface.Dto;
using X.Interface.Dto.Interface;
using X.Util.Core.Kernel;
using X.Util.Entities;

namespace X.Interface.Core
{
    public class UserManagerService : IUserManager
    {
        public ApiResult<ApiRequestContext> GetApiRequestContext()
        {
            return new ApiResult<ApiRequestContext>
            {
                Data = ExecutionContext<RequestContext>.Current.ApiRequestContext,
                Success = true
            };
        }
    }
}
