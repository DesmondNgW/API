using Microsoft.AspNetCore.Mvc.Filters;
using X.UI.Util.Helper;
using X.Util.Core;

namespace X.UI.Util.Filter
{
    public class ActionFilterWithOutTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RequestContextHelper.FilterActionExecutingContext(context, false);
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
