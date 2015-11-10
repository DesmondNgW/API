
namespace X.Util.Core
{
    public class AppConfig
    {
        /// <summary>
        /// 配置用户编号用户Debug模式
        /// </summary>
        public static string CustomerNo => ConfigurationHelper.GetAppSetting(nameof(CustomerNo));

        /// <summary>
        /// 缓存key的版本号
        /// </summary>
        public static string CacheKeyVersion => ConfigurationHelper.GetAppSettingByName(nameof(CacheKeyVersion), "0.0.1");

        /// <summary>
        /// 路由缓存的版本号
        /// </summary>
        public static string RouterCacheApp => ConfigurationHelper.GetAppSettingByName(nameof(RouterCacheApp), "0.0.1");

        /// <summary>
        /// 基金缓存的版本号
        /// </summary>
        public static string FundCacheApp => ConfigurationHelper.GetAppSettingByName(nameof(FundCacheApp), "0.0.1");

        /// <summary>
        /// 基金白名单的版本号
        /// </summary>
        public static string WhiteListCacheApp => ConfigurationHelper.GetAppSettingByName(nameof(WhiteListCacheApp), "0.0.1");

        /// <summary>
        /// 用户缓存版本号
        /// </summary>
        public static string CustomerCacheApp => ConfigurationHelper.GetAppSettingByName(nameof(CustomerCacheApp), "0.0.1");
    }
}
