using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MongoDbHelper
{
    /// <summary>
    /// MongoDB配置类
    /// </summary>
    public class MongoDbConfig
    {
        private static readonly Regex RegConnectionString = new Regex(@"^mongodb://([^?]+)(\?(.*))?$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 连接字符串格式如下mongodb://host1[:port1][,host2[:port2],…[,hostN[:portN]]]?username=username&amp;password=password&amp;connectName=connectName
        /// mongodb://127.0.0.1:27017,127.0.0.1:27017?username=username
        /// 加载配置文件（也可以在使用时加载）
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Config(string connectionString)
        {
            var match = RegConnectionString.Match(connectionString);
            if (match.Groups.Count <= 0) return;
            var info = HttpUtility.ParseQueryString(match.Groups[2].Value);
            var userName = info["username"] ?? string.Empty;
            var password = info["password"] ?? string.Empty;
            var credentialDb = info["db"] ?? string.Empty;
            var connectName = string.IsNullOrWhiteSpace(info["connectName"]) ? MongoDbConnection.DefaultConnectName : info["connectName"];
            int maxConnectionPoolSize;
            int.TryParse(info["maxConnectionPoolSize"], out maxConnectionPoolSize);
            var list = match.Groups[1].Value.Split(',');
            if (list.Length <= 0) return;
            MongoDbConnection.Configure(list.Select(t => new Uri("mongodb://" + t)).ToList(), connectName, userName, password, credentialDb, maxConnectionPoolSize);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="credentialDataBase">用户名密码所在db</param>
        /// <param name="maxConnectionPoolSize"></param>
        public static void Config(List<Uri> servers, string credentialDataBase = "admin", int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            MongoDbConnection.Configure(servers, MongoDbConnection.DefaultConnectName, string.Empty, string.Empty, credentialDataBase, maxConnectionPoolSize);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="connectName">连接名称</param>
        /// <param name="credentialDataBase">用户名密码所在db</param>
        /// <param name="maxConnectionPoolSize"></param>
        public static void Config(List<Uri> servers, string connectName, string credentialDataBase = "admin", int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            MongoDbConnection.Configure(servers, connectName, string.Empty, string.Empty, credentialDataBase, maxConnectionPoolSize);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="credentialDataBase">用户名密码所在db</param>
        /// <param name="maxConnectionPoolSize"></param>
        public static void Config(List<Uri> servers, string userName, string password, string credentialDataBase = "admin", int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            MongoDbConnection.Configure(servers, MongoDbConnection.DefaultConnectName, userName, password, credentialDataBase, maxConnectionPoolSize);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="connectName">连接名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="credentialDataBase">用户名密码所在db</param>
        /// <param name="maxConnectionPoolSize"></param>
        public static void Config(List<Uri> servers, string connectName, string userName, string password, string credentialDataBase = "admin", int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize)
        {
            MongoDbConnection.Configure(servers, connectName, userName, password, credentialDataBase, maxConnectionPoolSize);
        }

        /// <summary>
        /// 查看数据库连接状态
        /// </summary>
        public static void ShowStatus()
        {
            MongoDbConnection.ShowStatus();
        }
    }
}
