using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace X.Util.Core.Kernel
{
    public class ServiceCollectionHelper
    {
        private static IServiceCollection iServiceCollection;

        /// <summary>
        /// 注入Service【HttpContextAccessor-MemoryCache】
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void SetServiceCollection(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddMemoryCache();
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
