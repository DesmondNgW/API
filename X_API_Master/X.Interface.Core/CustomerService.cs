using X.Business.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Interface.Core
{
    public class CustomerService : IUser
    {
        private const LogDomain EDomain = LogDomain.Business;
        public ApiResult<UserDto> GetUserInfo(UserRequestDtoBase context)
        {
            var provider = new InstanceProvider<CustomerBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetCustomerInfoByCustomerNo, ExecutionContext<RequestContext>.Current.CustomerNo, CoreBase.CallSuccess, provider.Close);
            return ServiceHelper.Convert(iresult, () =>
            {
                var dto = iresult.Result.AutoMapper<UserDto>();
                dto.Uid = context.Uid;
                return dto;
            });
        }

        public ApiResult<bool> Logout(UserRequestDtoBase context)
        {
            CustomerBusiness.ClearAllCache4Customer(ExecutionContext<RequestContext>.Current.CustomerNo);
            ServiceHelper.Clear(context.Uid);
            return new ApiResult<bool> { Success = true, Data = true };
        }
    }
}
