using Microsoft.AspNetCore.Mvc.Filters;
using X.UI.Util.Helper;

namespace X.UI.Util.Filter
{
    public class ActionFilterWithTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RequestContextHelper.FilterActionExecutingContext(context, true);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            RequestContextHelper.AddResponseHeaders(context);
        }
    }
}
