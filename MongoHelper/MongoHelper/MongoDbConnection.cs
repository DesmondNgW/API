using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbHelper
{
    /// <summary>
    /// MongoDB链接
    /// </summary>
    internal class MongoDbConnection
    {
        private static readonly ConcurrentDictionary<string, MongoDbConfigure> Configures = new ConcurrentDictionary<string, MongoDbConfigure>();
        private static readonly object LockObj = new object();
        internal const string DefaultConnectName = "def";
        internal const int MaxConnectionPoolSize = 8;

        /// <summary>
        /// 添加Mongo数据库配置
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="connectName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="maxConnectionPoolSize"></param>
        public static void Configure(List<Uri> servers, string connectName, string userName = "", string password = "", int maxConnectionPoolSize = MaxConnectionPoolSize)
        {
            if (Configures.ContainsKey(connectName) && Configures[connectName] != null) return;
            lock (LockObj)
            {
                if (Configures.ContainsKey(connectName) && Configures[connectName] != null) return;
                var config = new MongoDbConfigure
                {
                    Servers = servers,
                    MongoClientSettings = new MongoClientSettings()
                    {
                        MaxConnectionPoolSize = maxConnectionPoolSize > 0 ? maxConnectionPoolSize : MaxConnectionPoolSize,
                        WaitQueueSize = 5000,
                        Servers = servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port))
                    }
                };
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password)) config.MongoClientSettings.Credentials = new List<MongoCredential> { MongoCredential.CreateMongoCRCredential("admin", userName, password) };
                config.MongoClient = new MongoClient(config.MongoClientSettings);
                Configures[connectName] = config;
            }
        }

        /// <summary>
        /// 连接数据库(兼容旧版本)
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static MongoClient Connection(string conn = DefaultConnectName)
        {
            return Connection(string.Empty, string.Empty, string.Empty, conn, null);
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="collection">表collection</param>
        /// <param name="connectName">链接数据库名称</param>
        /// <param name="reloadConfigure">重新加载配置文件</param>
        /// <param name="maxConnectionPoolSize">最大连接池</param>
        /// <returns>
        /// 返回数据库连接地址
        /// </returns>
        public static MongoClient Connection(string userName, string password, string collection, string connectName, Action reloadConfigure, int maxConnectionPoolSize = MaxConnectionPoolSize)
        {
            var setting = Configures[connectName];
            if (setting == null && reloadConfigure != null) reloadConfigure.Invoke();
            if (setting != null && setting.MongoClient != null) return setting.MongoClient;
            lock (LockObj)
            {
                if (setting != null && setting.MongoClient != null) return Configures[connectName].MongoClient;
                try
                {
                    if (setting == null) throw new Exception("MongoDb缺少配置信息");
                    if (string.IsNullOrWhiteSpace(collection)) collection = "admin";
                    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password)) setting.MongoClientSettings.Credentials = new List<MongoCredential> { MongoCredential.CreateMongoCRCredential(collection, userName, password) };
                    setting.MongoClientSettings.MaxConnectionPoolSize = maxConnectionPoolSize > 0 ? maxConnectionPoolSize : MaxConnectionPoolSize;
                    setting.MongoClientSettings.WaitQueueSize = 5000;
                    if (setting.Servers != null && setting.Servers.Count > 0) setting.MongoClientSettings.Servers = setting.Servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port));
                    setting.MongoClient = new MongoClient(setting.MongoClientSettings);
                    Configures[connectName] = setting;
                    return setting.MongoClient;
                }
                catch (Exception ex)
                {
                    throw (new Exception(string.Format("Servers: {0} \r\n Exceptions: {1}", setting != null ? setting.Servers.ToJson() : "null", ex)));
                }
            }
        }

        /// <summary>
        /// 连接池状态
        /// </summary>
        public static void ShowStatus()
        {
            foreach (var key in Configures.Keys)
            {
                Console.WriteLine(string.Format(@"connectName: {0}, Servers: {1}, Connnect: {2}", key, Configures[key].Servers.ToJson(), Configures[key].MongoClient != null));
            }
        }
    }

    /// <summary>
    /// MongoClient配置类
    /// </summary>
    internal class MongoDbConfigure
    {
        public List<Uri> Servers { get; set; }

        public MongoClient MongoClient { get; set; }

        public MongoClientSettings MongoClientSettings
        {
            get { return new MongoClientSettings(); }
            set { }
        }
    }
}
