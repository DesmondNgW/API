using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace X.UI.API.Util
{
    /// <summary>
    /// 游客可访问的Action特性
    /// </summary>
    public class RequestAttribute : ActionFilterAttribute
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
            
        }
    }
}