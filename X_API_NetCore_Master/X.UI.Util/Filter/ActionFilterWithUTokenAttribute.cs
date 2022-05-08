using Microsoft.AspNetCore.Mvc.Filters;
using X.UI.Util.Helper;

namespace X.UI.Util.Filter
{
    public class ActionFilterWithUTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RequestContextHelper.FilterActionExecuting(context, true, true);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            RequestContextHelper.FilterActionExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            RequestContextHelper.FilterResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            RequestContextHelper.FilterResultExecuted(context);
        }
    }
}
