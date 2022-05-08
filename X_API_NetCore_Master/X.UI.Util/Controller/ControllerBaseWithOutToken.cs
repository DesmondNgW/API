using Microsoft.AspNetCore.Mvc;
using X.UI.Util.Filter;

namespace X.UI.Util.Controller
{
    [ActionFilterWithOutToken]
    [GlobalExceptionFilter]
    public class ControllerBaseWithOutToken : ControllerBase
    {

    }
}
