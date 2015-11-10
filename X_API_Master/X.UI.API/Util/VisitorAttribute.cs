using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using X.Util.Core;
using X.Util.Entities;

namespace X.UI.API.Util
{
    /// <summary>
    /// 游客可访问的Action特性
    /// </summary>
    public class VisitorAttribute : ActionFilterAttribute
    {
        private const string Token = "X.UI.API.Util.UnLoginActionRuleAttribute.UnLoginActionToken";
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
            var context = ControllerHelper.GetApiRequestContext(actionContext);
            ControllerHelper.ApiCallMonitor(context);
            ExecutionContext<RequestContext>.Init(new RequestContext
            {
                Uid = Token,
                Zone = 1,
                Version = context.Version,
                ClientType = (EnumClientType)context.ClientType,
                Ctoken = Token + context.Cid,
                Ptoken = Token + context.Tid,
                Token = context.Token,
            });
            ControllerHelper.CacheInit((actionContext.Request.Headers.CacheControl != null && actionContext.Request.Headers.CacheControl.NoCache) || "no-cache".Equals(actionContext.Request.Headers.Pragma.ToString()));
        }
    }
}