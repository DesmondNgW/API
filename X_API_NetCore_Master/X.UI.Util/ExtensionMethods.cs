using Microsoft.Extensions.DependencyInjection;
using X.Util.Core.Kernel;

namespace X.UI.Util
{
    /// <summary>
    /// UI扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 注入ServiceCollection
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void SetServiceCollection(this IServiceCollection serviceCollection)
        {
            ServiceCollectionHelper.SetServiceCollection(serviceCollection);
        }
    }
}
