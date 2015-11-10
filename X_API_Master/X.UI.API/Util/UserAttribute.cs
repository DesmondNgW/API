using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using X.Util.Core;
using X.Util.Entities;

namespace X.UI.API.Util
{
    /// <summary>
    /// 需要用户登录的Action特性
    /// </summary>
    public class UserAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在调用操作方法之后发生
        /// </summary>
        /// <param name="actionExecutedContext">操作执行的上下文</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ControllerHelper.OnActionExecuted(actionExecutedContext);
        }

        /// <summary>
        ///在调用操作方法之前发生
        /// </summary>
        /// <param name="actionContext">操作上下文</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            UserBaseController.UserIdentity(actionContext);
            var customerNo = ExecutionContext<RequestContext>.Current.CustomerNo;
            ControllerHelper.CacheInit(!string.IsNullOrEmpty(customerNo) && customerNo.Equals(AppConfig.CustomerNo) && (actionContext.Request.Headers.CacheControl != null && actionContext.Request.Headers.CacheControl.NoCache) || "no-cache".Equals(actionContext.Request.Headers.Pragma.ToString()));
        }
    }
}