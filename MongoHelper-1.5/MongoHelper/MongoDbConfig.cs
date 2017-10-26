using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// 连接字符串格式如下mongodb://host1[:port1][,host2[:port2],…[,hostN[:portN]]]?username=username&amp;password=password&amp;connectName=connectName&amp;credential=ScramSha1
        /// mongodb://127.0.0.1:27017,127.0.0.1:27017?username=username
        /// 加载配置文件（也可以在使用时加载）
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Config(string connectionString)
        {
            var match = RegConnectionString.Match(connectionString);
            if (match.Groups.Count <= 0) return;
            var info = ParseQueryString(match.Groups[3].Value);
            var userName = info.ContainsKey("username") ? info["username"] : string.Empty;
            var password = info.ContainsKey("password") ? info["password"] : string.Empty;
            var credentialDb = info.ContainsKey("db") ? info["db"] : string.Empty;
            var credential = (MongoDbCredential)Enum.Parse(typeof(MongoDbCredential), info.ContainsKey("credential") 
                ? info["credential"] : "ScramSha1");
            var c = info.ContainsKey("connectName") ? info["connectName"] : string.Empty;
            var connectName = string.IsNullOrWhiteSpace(c) ? MongoDbConnection.DefaultConnectName : c;
            int maxConnectionPoolSize;
            int.TryParse(info.ContainsKey("maxConnectionPoolSize") ? info["maxConnectionPoolSize"] : string.Empty, out maxConnectionPoolSize);
            var list = match.Groups[1].Value.Split(',');
            if (list.Length <= 0) return;
            MongoDbConnection.Configure(list.Select(t => new Uri("mongodb://" + t)).ToList(), connectName, userName, password, maxConnectionPoolSize, credential, credentialDb);
        }

        private static Dictionary<string, string> ParseQueryString(string query)
        {
            var ret = new Dictionary<string, string>();
            var reg = new Regex("([^&]+?)=(.+?)(?:&|$)");
            var matches = reg.Matches(query);
            if (matches.Count <= 0) return ret;
            foreach (Match match in matches)
            {
                ret[match.Groups[1].ToString()] = match.Groups[2].ToString();
            }
            return ret;
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="maxConnectionPoolSize"></param>
        /// <param name="credential"></param>
        /// <param name="credentialDb"></param>
        public static void Config(List<Uri> servers, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize, MongoDbCredential credential = MongoDbCredential.ScramSha1, string credentialDb = "")
        {
            MongoDbConnection.Configure(servers, MongoDbConnection.DefaultConnectName, string.Empty, string.Empty, maxConnectionPoolSize, credential, credentialDb);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="connectName">连接名称</param>
        /// <param name="maxConnectionPoolSize"></param>
        /// <param name="credential"></param>
        /// <param name="credentialDb"></param>
        public static void Config(List<Uri> servers, string connectName, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize, MongoDbCredential credential = MongoDbCredential.ScramSha1, string credentialDb = "")
        {
            MongoDbConnection.Configure(servers, connectName, string.Empty, string.Empty, maxConnectionPoolSize, credential, credentialDb);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="maxConnectionPoolSize"></param>
        /// <param name="credential"></param>
        /// <param name="credentialDb"></param>
        public static void Config(List<Uri> servers, string userName, string password, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize, MongoDbCredential credential = MongoDbCredential.ScramSha1, string credentialDb = "")
        {
            MongoDbConnection.Configure(servers, MongoDbConnection.DefaultConnectName, userName, password, maxConnectionPoolSize, credential, credentialDb);
        }

        /// <summary>
        /// 设置Mongo数据库连接地址
        /// </summary>
        /// <param name="servers">数据库地址列表Uri格式mongodb://host:port/</param>
        /// <param name="connectName">连接名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="maxConnectionPoolSize"></param>
        /// <param name="credential"></param>
        /// <param name="credentialDb"></param>
        public static void Config(List<Uri> servers, string connectName, string userName, string password, int maxConnectionPoolSize = MongoDbConnection.MaxConnectionPoolSize, MongoDbCredential credential = MongoDbCredential.ScramSha1, string credentialDb = "")
        {
            MongoDbConnection.Configure(servers, connectName, userName, password, maxConnectionPoolSize, credential, credentialDb);
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
