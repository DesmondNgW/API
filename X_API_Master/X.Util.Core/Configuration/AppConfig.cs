﻿
namespace X.Util.Core.Configuration
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
        /// 本地缓存依赖文件目录
        /// </summary>
        public static string CacheDependencyBaseDirectory
        {
            get { return ConfigurationHelper.GetAppSettingByName("CacheDependencyBaseDirectory", string.Empty); }
        }

        /// <summary>
        /// 配置用户编号用户Debug模式
        /// </summary>
        public static string CustomerNo
        {
            get { return ConfigurationHelper.GetAppSettingByName("CustomerNo", string.Empty); }
        }
        /// <summary>
        /// 缓存key的版本号
        /// </summary>
        public static string CacheKeyVersion
        {
            get { return ConfigurationHelper.GetAppSettingByName("CacheKeyVersion", "0.0.1"); }
        }

        /// <summary>
        /// CookieDomain
        /// </summary>
        public static string CookieDomain
        {
            get { return ConfigurationHelper.GetAppSetting("CookieDomain"); }
        }

        /// <summary>
        /// CouchCacheEnable
        /// </summary>
        public static bool CouchCacheEnable
        {
            get { return ConfigurationHelper.GetAppSettingByName("CouchCacheEnable", true); }
        }

        /// <summary>
        /// RedisCacheEnable
        /// </summary>
        public static bool RedisCacheEnable
        {
            get { return ConfigurationHelper.GetAppSettingByName("RedisCacheEnable", true); }
        }

        /// <summary>
        /// MongoDbEnable
        /// </summary>
        public static bool MongoDbEnable
        {
            get { return ConfigurationHelper.GetAppSettingByName("MongoDbEnable", true); }
        }
    }
}
