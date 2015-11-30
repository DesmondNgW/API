using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MongoDbHelper
{
    /// <summary>
    /// MongoDB配置类
    /// </summary>
    public class MongoDbConfig
    {
        private static readonly Regex RegConnectionString = new Regex(@"^mongodb://(\[.*\])*(.+)$", RegexOptions.IgnoreCase);
        
        /// <summary>
        /// 连接字符串格式如下mongodb://[username:password:connectName]host1[:port1][,host2[:port2],…[,hostN[:portN]]]
        /// mongodb://[test:test:def]127.0.0.1:27017,127.0.0.1:27017
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Config(string connectionString)
        {
            var match = RegConnectionString.Match(connectionString);
            if (match.Groups.Count <= 0) return;
            var info = match.Groups[1].Value.Split(',');
            var userName = info.Length == 3 ? info[0] : string.Empty;
            var password = info.Length == 3 ? info[1] : string.Empty;
            var connectName = info.Length == 3 && !string.IsNullOrEmpty(info[2]) ? info[2] : MongoDbConnection.DefaultConnectName;
            var list = match.Groups[2].Value.Split(',');
            if (list.Length <= 0) return;
            MongoDbConnection.Configure(list.Select(t => new Uri("mongodb://" + t)).ToList(), connectName, userName, password);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        public static void Config(List<Uri> servers)
        {
            MongoDbConnection.Configure(servers, MongoDbConnection.DefaultConnectName, string.Empty, string.Empty);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="connectName">连接名称</param>
        public static void Config(List<Uri> servers, string connectName)
        {
            MongoDbConnection.Configure(servers, connectName, string.Empty, string.Empty);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public static void Config(List<Uri> servers, string userName, string password)
        {
            MongoDbConnection.Configure(servers, MongoDbConnection.DefaultConnectName, userName, password);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="connectName">连接名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public static void Config(List<Uri> servers, string connectName, string userName, string password)
        {
            MongoDbConnection.Configure(servers, connectName, userName, password);
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
