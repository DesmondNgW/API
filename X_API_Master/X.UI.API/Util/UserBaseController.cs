using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using X.Interface.Core;
using X.Util.Core;
using X.Util.Entities;

namespace X.UI.API.Util
{
    /// <summary>
    /// 需要用户登录的Controller基类
    /// </summary>
    [User, ApiException]
    public class UserBaseController : ApiController
    {
        /// <summary>
        /// 用户身份验证（登录状态、记录用户相关状态、监控Api调用）
        /// </summary>
        /// <param name="actionContext"></param>
        public static void UserIdentity(HttpActionContext actionContext)
        {
            var context = ControllerHelper.GetApiRequestContext(actionContext);
            var uid = ControllerHelper.GetUid(actionContext);
            var statusZone = ServiceHelper.GetStatusZone(context.Token, uid);
            var loginState = ServiceHelper.GetLoginStatus(context.Token, uid, statusZone);
            if (Equals(loginState, null) || loginState.StatusZone != statusZone || loginState.Token != context.Token || loginState.Uid != uid) throw new InvalidOperationException("token过期或与uid不匹配");
            if (loginState.CustomerNo != AppConfig.CustomerNo && actionContext.Request.RequestUri.AbsolutePath.ToLower().StartsWith("/api/manager/")) throw new UnauthorizedAccessException("404");
            context.UserInfo = new { loginState.CustomerNo, loginState.Zone, loginState.CustomerName }.ToJson();
            ControllerHelper.ApiCallMonitor(context);
            ExecutionContext<RequestContext>.Init(new RequestContext
            {
                Uid = uid,
                Zone = loginState.Zone,
                Version = context.Version,
                ClientType = (EnumClientType)context.ClientType,
                Ctoken = uid + context.Cid,
                Ptoken = uid + context.Tid,
                Token = context.Token,
                CustomerName = loginState.CustomerName,
                CustomerNo = loginState.CustomerNo
            });
        }
    }
}