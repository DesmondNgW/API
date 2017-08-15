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
        private const string UnLoginActionId = "X.UI.API.Util.UnLoginActionRuleAttribute.UnLoginActionId";
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
            new RequestContext
            {
                Uid = UnLoginActionId,
                Zone = 1,
                Version = context.Version,
                ClientType = (EnumClientType)context.ClientType,
                Ctoken = UnLoginActionId + context.Cid,
                Ptoken = UnLoginActionId + context.Tid,
                Token = context.Token,
                RequestId = ControllerHelper.GetRequestId(actionContext)
            }.Update(null);
            ControllerHelper.CacheInit((actionContext.Request.Headers.CacheControl != null && actionContext.Request.Headers.CacheControl.NoCache) || "no-cache".Equals(actionContext.Request.Headers.Pragma.ToString()));
        }
    }
}