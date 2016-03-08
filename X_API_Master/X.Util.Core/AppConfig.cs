
namespace X.Util.Core
{
    public class AppConfig
    {
        /// <summary>
        /// log4net 日志根目录
        /// </summary>
        public static string Log4NetBaseDirectory
        {
            get { return ConfigurationHelper.GetAppSettingByName("Log4NetBaseDirectory", string.Empty); }
        }

        /// <summary>
        /// 配置用户编号用户Debug模式
        /// </summary>
        public static string CustomerNo
        {
            get { return ConfigurationHelper.GetAppSetting("CustomerNo"); }
        }
        /// <summary>
        /// 缓存key的版本号
        /// </summary>
        public static string CacheKeyVersion
        {
            get { return ConfigurationHelper.GetAppSettingByName("CacheKeyVersion", "0.0.1"); }
        }
    }
}
