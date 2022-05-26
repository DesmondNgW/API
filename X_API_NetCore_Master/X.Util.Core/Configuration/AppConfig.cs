

namespace X.Util.Core.Configuration
{
    public class AppConfig
    {
        /// <summary>
        /// log4net 日志根目录
        /// </summary>
        public static string Log4NetBaseDirectory => ConfigurationHelper.GetAppSettingByName("Log4NetBaseDirectory", string.Empty);

        /// <summary>
        /// 配置用户编号用户Debug模式
        /// </summary>
        public static string CustomerNo => ConfigurationHelper.GetAppSettingByName("CustomerNo", string.Empty);
        /// <summary>
        /// 缓存key的版本号
        /// </summary>
        public static string CacheKeyVersion => ConfigurationHelper.GetAppSettingByName("CacheKeyVersion", "0.0.1");

        /// <summary>
        /// CookieDomain
        /// </summary>
        public static string CookieDomain => ConfigurationHelper.GetAppSetting("CookieDomain");

        /// <summary>
        /// RedisCacheEnable
        /// </summary>
        public static bool RedisCacheEnable => ConfigurationHelper.GetAppSettingByName("RedisCacheEnable", true);

        /// <summary>
        /// MongoDbEnable
        /// </summary>
        public static bool MongoDbEnable => ConfigurationHelper.GetAppSettingByName("MongoDbEnable", true);

        /// <summary>
        /// Debug模式
        /// </summary>
        public static bool Debug => ConfigurationHelper.GetAppSettingByName("Debug", true);
    }
}
