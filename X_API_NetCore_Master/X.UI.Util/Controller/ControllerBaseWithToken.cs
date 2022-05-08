using Microsoft.AspNetCore.Mvc;
using X.UI.Util.Filter;

namespace X.UI.Util.Controller
{
    [ActionFilterWithToken]
    [GlobalExceptionFilter]
    public class ControllerBaseWithToken : ControllerBase
    {
    }
}
