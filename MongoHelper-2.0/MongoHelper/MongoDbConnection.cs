using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        private static readonly object ConfigLockObj = new object();
        private static readonly object ClientLockObj = new object();
        internal const string DefaultConnectName = "def";
        internal const int MaxConnectionPoolSize = 8;

        /// <summary>
        /// 添加Mongo数据库配置
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="connectName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="credentialDataBase"></param>
        /// <param name="maxConnectionPoolSize"></param>
        public static void Configure(List<Uri> servers, string connectName, string userName = "", string password = "", string credentialDataBase = "admin", int maxConnectionPoolSize = MaxConnectionPoolSize)
        {
            if (Configures.ContainsKey(connectName) && Configures[connectName] != null) return;
            lock (ConfigLockObj)
            {
                if (Configures.ContainsKey(connectName) && Configures[connectName] != null) return;
                var config = new MongoDbConfigure
                {
                    Servers = servers,
                    MongoClientSettings = new MongoClientSettings
                    {
                        MaxConnectionPoolSize = maxConnectionPoolSize > 0 ? maxConnectionPoolSize : MaxConnectionPoolSize,
                        WaitQueueSize = 5000,
                        Servers = servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port))
                    },
                    UserName = userName,
                    Password = password,
                    CredentialDataBase = string.IsNullOrEmpty(credentialDataBase) ? "admin" : credentialDataBase,
                    MongoClients = new ConcurrentDictionary<string, MongoClient>()
                };
                Configures[connectName] = config;
            }
        }

        private static string Sha1(string s)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(s));
            var sb = new StringBuilder();
            foreach (byte t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
        }

        private static string GetMongoClientKey(string userName, string password, string database)
        {
            return Sha1(userName + "_" + password + "_" + database);
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
        /// <param name="database">数据库</param>
        /// <param name="connectName">链接数据库名称</param>
        /// <param name="reloadConfigure">重新加载配置文件</param>
        /// <param name="maxConnectionPoolSize">最大连接池</param>
        /// <returns>
        /// 返回数据库连接地址
        /// </returns>
        public static MongoClient Connection(string userName, string password, string database, string connectName, Action reloadConfigure, int maxConnectionPoolSize = MaxConnectionPoolSize)
        {
            var setting = Configures[connectName];
            if (setting == null && reloadConfigure != null) reloadConfigure.Invoke();
            if (setting == null || setting.MongoClientSettings == null) throw new Exception("MongoDb缺少配置信息");
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                userName = setting.UserName;
                password = setting.Password;
            }
            if (string.IsNullOrEmpty(database)) database = setting.CredentialDataBase ?? "admin";
            var key = GetMongoClientKey(userName, password, database);
            if (setting.MongoClients.ContainsKey(key)) return setting.MongoClients[key];
            lock (ClientLockObj)
            {
                if (setting.MongoClients.ContainsKey(key)) return setting.MongoClients[key];
                try
                {
                    setting.MongoClientSettings.MaxConnectionPoolSize = maxConnectionPoolSize > 0 ? maxConnectionPoolSize : MaxConnectionPoolSize;
                    setting.MongoClientSettings.WaitQueueSize = 5000;
                    setting.MongoClientSettings.VerifySslCertificate = false;
                    if (setting.Servers != null && setting.Servers.Count > 0) setting.MongoClientSettings.Servers = setting.Servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port));
                    setting.MongoClientSettings.Credentials = new List<MongoCredential>
                    {
                        MongoCredential.CreateCredential(database, userName, password)
                    };
                    setting.MongoClients[key] = new MongoClient(setting.MongoClientSettings);
                    Configures[connectName] = setting;
                    return setting.MongoClients[key];
                }
                catch (Exception ex)
                {
                    throw (new Exception(string.Format("Servers: {0} \r\n Exceptions: {1}", setting.Servers.ToJson(), ex)));
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
                Console.WriteLine(@"connectName: {0}, Servers: {1}, Connnect: {2}", key, Configures[key].Servers.ToJson(), Configures[key].MongoClients != null && Configures[key].MongoClients.Count > 0);
            }
        }
    }

    /// <summary>
    /// MongoClient配置类
    /// </summary>
    internal class MongoDbConfigure
    {
        public List<Uri> Servers { get; set; }

        public ConcurrentDictionary<string, MongoClient> MongoClients { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string CredentialDataBase { get; set; }

        public MongoClientSettings MongoClientSettings { get; set; }
    }
}
