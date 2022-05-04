using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace X.Util.Core.Kernel
{
    public class HttpContextHelper
    {
        private static IServiceCollection iServiceCollection;

        public static void SetServiceCollection(IServiceCollection serviceCollection)
        {
            iServiceCollection = serviceCollection;
        }

        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor factory = (IHttpContextAccessor)iServiceCollection.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
                if (factory != null)
                {
                    factory.Update(string.Empty, string.Empty);
                }
                return factory.HttpContext;
            }
        }

        public static HttpContext CallContext
        {
            get
            {
                return ExecutionContext<IHttpContextAccessor>.Current.HttpContext;
            }
        }
    }
}
