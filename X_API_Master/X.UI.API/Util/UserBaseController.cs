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
            var loginState = LoginServiceHelper.UserIdentity(context.Token, uid, context.ClientId, actionContext.Request.RequestUri);
            context.UserInfo = new { loginState.CustomerNo, loginState.Zone, loginState.CustomerName }.ToJson();
            ControllerHelper.ApiCallMonitor(context);
            new RequestContext
            {
                Uid = uid,
                Zone = loginState.Zone,
                Version = context.Version,
                ClientType = (EnumClientType)context.ClientType,
                Ctoken = uid + context.Cid,
                Ptoken = uid + context.Tid,
                Token = context.Token,
                CustomerName = loginState.CustomerName,
                CustomerNo = loginState.CustomerNo,
                ApiRequestContext = context
            }.Update(null);
        }
    }
}