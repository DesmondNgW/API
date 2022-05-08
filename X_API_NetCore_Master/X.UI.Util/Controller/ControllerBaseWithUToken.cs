using Microsoft.AspNetCore.Mvc;
using X.UI.Util.Filter;

namespace X.UI.Util.Controller
{
    [ActionFilterWithUToken]
    [GlobalExceptionFilter]
    public class ControllerBaseWithUToken : ControllerBase
    {
    }
}
