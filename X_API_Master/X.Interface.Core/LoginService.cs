using System;
using System.Text.RegularExpressions;
using X.Business.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpRequest;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Core;
using X.Util.Extend.Cryption;
using X.Util.Provider;

namespace X.Interface.Core
{
    public class LoginService : ILogin
    {
        private const LogDomain EDomain = LogDomain.Business;
        public ApiResult<UserDto> Login(LoginRequestDto loginDto)
        {
            if (!Regex.IsMatch(loginDto.LoginName, "^\\w{8,}$")) throw new ArgumentException(ErrorMessage.UserNameFormat);
            var routerProvider = new InstanceProvider<RouterBusiness>();
            var iresult = CoreAccess.Call(EDomain, routerProvider.Instance.GetRouteInfoByLoginInfo, loginDto.LoginName, CoreBase.CallSuccess, routerProvider.Close);
            if (Equals(iresult, null) || iresult.Zone.Equals(0)) throw new Exception(ErrorMessage.RouteError);
            ExecutionContext<RequestContext>.Update("Zone", iresult.Zone);
            var decrypt = RsaCryption.Decrypt(loginDto.RsaKey, loginDto.Password, loginDto.Nonce, loginDto.Size);
            if(string.IsNullOrEmpty(decrypt)) throw new Exception(ErrorMessage.KeyError);
            if (!Regex.IsMatch(decrypt, "^\\w{8,20}$")) throw new ArgumentException(ErrorMessage.PasswordFormat);
            var customerProvider = new InstanceProvider<CustomerBusiness>();
            var loginResult = CoreAccess.Call(EDomain, customerProvider.Instance.Login_Default, loginDto.LoginName, BaseCryption.Md5Bit32(decrypt), CoreBase.CallSuccess, customerProvider.Close);
            return ServiceHelper.Convert(loginResult, () =>
            {
                var dto = loginResult.Result.AutoMapper<UserDto>();
                dto.Uid = ServiceHelper.SetLoginStatus(loginResult.Result.CustomerNo, loginResult.Result.CustomerName, iresult.Zone.GetHashCode());
                return dto;
            });
        }
    }
}
