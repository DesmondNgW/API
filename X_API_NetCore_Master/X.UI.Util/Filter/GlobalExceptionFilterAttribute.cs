using Microsoft.AspNetCore.Mvc.Filters;
using X.UI.Util.Helper;

namespace X.UI.Util.Filter
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            RequestContextHelper.GlobalException(context);
        }
    }
}
