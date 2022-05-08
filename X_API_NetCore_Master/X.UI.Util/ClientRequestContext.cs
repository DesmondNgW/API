using Microsoft.AspNetCore.Http;
using X.Util.Core.Kernel;

namespace X.UI.Util
{
    public class ClientRequestContext
    {
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public static ClientRequestContext Context
        {
            get
            {
                return ExecutionContext<ClientRequestContext>.Current;
            }
        }

        public static IHttpContextAccessor CueentHttpContextAccessor
        {
            get
            {
                return Context.HttpContextAccessor;
            }
        }

        public static HttpContext CueentHttpContext
        {
            get
            {
                return CueentHttpContextAccessor.HttpContext;
            }
        }
    }
}
