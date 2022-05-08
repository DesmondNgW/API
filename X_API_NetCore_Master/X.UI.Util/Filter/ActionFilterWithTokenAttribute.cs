using Microsoft.AspNetCore.Mvc.Filters;

namespace X.UI.Util.Filter
{
    public class ActionFilterWithTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            throw null;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            throw null;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            throw null;
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            throw null;
        }
    }
}
