using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;

namespace MongoDbHelper
{
    public enum MongoDbCredential
    {
        MongoCr,
        ScramSha1,
        Plain
    }

    /// <summary>
    /// MongoDB链接
    /// </summary>
    internal class MongoDbConnection
    {
        private static readonly ConcurrentDictionary<string, MongoClient> MongoClientPools = new ConcurrentDictionary<string, MongoClient>();
        private static ConcurrentDictionary<string, MongoDbConfigure> _configures = new ConcurrentDictionary<string, MongoDbConfigure>();
        private static readonly object ClientLockObj = new object();
        internal const string DefaultConnectName = "def";
        internal const int MaxConnectionPoolSize = 8;

        /// <summary>
        /// MongoCredential
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="credentialDataBase"></param>
        /// <param name="credential"></param>
        /// <returns></returns>
        private static IEnumerable<MongoCredential> GetMongoCredential(string userName, string password, string credentialDataBase, MongoDbCredential credential = MongoDbCredential.ScramSha1)
        {
            if (string.IsNullOrEmpty(credentialDataBase)) credentialDataBase = "admin";
            switch (credential)
            {
                case MongoDbCredential.MongoCr:
                    return new List<MongoCredential> { MongoCredential.CreateMongoCRCredential(credentialDataBase, userName, password) };
                case MongoDbCredential.Plain:
                    return new List<MongoCredential> { MongoCredential.CreatePlainCredential(credentialDataBase, userName, password) };
                default:
                    return new List<MongoCredential> { MongoCredential.CreateCredential(credentialDataBase, userName, password) };
            }
        }

        /// <summary>
        /// GetMongoClientSettings
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        private static MongoClientSettings GetMongoClientSettings(MongoDbConfigure configure)
        {
            var setting = new MongoClientSettings
            {
                MaxConnectionPoolSize = configure.MaxConnectionPoolSize > 0 ? configure.MaxConnectionPoolSize : MaxConnectionPoolSize,
                WaitQueueSize = 5000,
                Servers = configure.Servers.Select(uri => new MongoServerAddress(uri.Host, uri.Port))
            };
            if (!string.IsNullOrEmpty(configure.UserName) && !string.IsNullOrEmpty(configure.Password))
            {
                setting.Credentials = GetMongoCredential(configure.UserName, configure.Password, configure.CredentialDataBase, configure.Credential);
            }
            return setting;
        }

        /// <summary>
        /// 配置文件
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="connectName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="credentialDataBase"></param>
        /// <param name="maxConnectionPoolSize"></param>
        /// <param name="credential"></param>
        public static void Configure(IEnumerable<Uri> servers, string connectName, string userName, string password, string credentialDataBase, int maxConnectionPoolSize = MaxConnectionPoolSize, MongoDbCredential credential = MongoDbCredential.ScramSha1)
        {
            if (_configures == null) _configures = new ConcurrentDictionary<string, MongoDbConfigure>();
            if (!_configures.ContainsKey(connectName))
            {
                _configures[connectName] = new MongoDbConfigure
                {
                    Servers = servers,
                    ConnectionName = connectName,
                    UserName = userName,
                    Password = password,
                    CredentialDataBase = credentialDataBase,
                    Credential = credential,
                    MaxConnectionPoolSize = maxConnectionPoolSize
                };
            }
        }

        private static string Sha1(string s)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var encryptedBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(s));
            var sb = new StringBuilder();
            foreach (var t in encryptedBytes) sb.AppendFormat("{0:x2}", t);
            return sb.ToString().ToLower();
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
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <param name="connectName"></param>
        /// <param name="reloadConfigure"></param>
        /// <returns></returns>
        public static MongoClient Connection(string userName, string password, string database, string connectName, Action reloadConfigure)
        {
            if (reloadConfigure != null) reloadConfigure.Invoke();
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database)) database = string.Empty;
            var key = Sha1(connectName + database);
            if (MongoClientPools.ContainsKey(key) && reloadConfigure == null) return MongoClientPools[key];
            lock (ClientLockObj)
            {
                if (MongoClientPools.ContainsKey(key) && reloadConfigure == null) return MongoClientPools[key];
                if (!_configures.ContainsKey(connectName) || _configures[connectName] == null) throw new Exception("MongoDb缺少配置信息");
                var setting = GetMongoClientSettings(_configures[connectName]);
                MongoClientPools[key] = new MongoClient(setting);
            }
            return MongoClientPools[key];
        }

        /// <summary>
        /// 连接池状态
        /// </summary>
        public static void ShowStatus()
        {
            foreach (var key in MongoClientPools.Keys)
            {
                var s = MongoClientPools[key].Settings.Servers.Aggregate(string.Empty, (current, sb) => current + ("," + sb.Host + ":" + sb.Port)).Substring(1);
                Console.WriteLine(@"connectName: {0}, Servers: {1}, Connnect: {2}", key, s, MongoClientPools[key] != null);
            }
        }
    }

    /// <summary>
    /// MongoDb配置
    /// </summary>
    internal class MongoDbConfigure
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// 服务器列表
        /// </summary>
        public IEnumerable<Uri> Servers { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证Db
        /// </summary>
        public string CredentialDataBase { get; set; }

        /// <summary>
        /// 最大连接池大小
        /// </summary>
        public int MaxConnectionPoolSize { get; set; }

        /// <summary>
        /// 验证协议类型
        /// </summary>
        public MongoDbCredential Credential { get; set; }
    }

}
