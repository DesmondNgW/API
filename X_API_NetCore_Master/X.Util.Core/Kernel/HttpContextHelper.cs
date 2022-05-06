using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace X.Util.Core.Kernel
{
    public class HttpContextHelper
    {
        private static IServiceCollection iServiceCollection;

        /// <summary>
        /// HttpContextAccessor 注入
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void SetServiceCollection(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpContextAccessor();
            iServiceCollection = serviceCollection;
        }

        public static HttpContext Current
        {
            get
            {
                object ifactory = iServiceCollection.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
                IHttpContextAccessor factory = (IHttpContextAccessor)ifactory;
                if (factory != null)
                {
                    return factory.HttpContext;
                }
                return default;
            }
        }
    }
}
