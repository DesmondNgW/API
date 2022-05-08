using Microsoft.Extensions.DependencyInjection;
using X.Util.Core.Kernel;
using Microsoft.AspNetCore.Http;
using X.Util.Core;

namespace X.UI.Util
{
    /// <summary>
    /// UI扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 注入HttpContext
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void SetHttpContex(this IServiceCollection serviceCollection)
        {
            HttpContextHelper.SetServiceCollection(serviceCollection);
            InjectClientRequestContext(serviceCollection);
        }

        public static void InjectClientRequestContext(this IServiceCollection serviceCollection)
        {
            object ifactory = serviceCollection.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
            IHttpContextAccessor factory = (IHttpContextAccessor)ifactory;
            if (factory != null)
            {
                var clientRequestContext = new ClientRequestContext()
                {
                    HttpContextAccessor = factory,
                };
                clientRequestContext.Update(string.Empty, string.Empty);
            }
        }
    }
}
