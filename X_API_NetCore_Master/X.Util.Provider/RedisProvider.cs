using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public sealed class RedisProvider : IProvider<IDatabase>
    {
        #region 构造函数
        public readonly string ServerName;
        public readonly int DbNum;

        public RedisProvider(int dbNum = -1) : this(dbNum, ConfigurationHelper.RedisDefaultServername) { }
        public RedisProvider(string serverName) : this(-1, serverName) { }
        public RedisProvider(int dbNum, string serverName)
        {
            DbNum = dbNum;
            ServerName = serverName;
        }
        #endregion

        #region 内部实现
        /// <summary>
        /// redis连接池管理
        /// </summary>
        private static ConnectionMultiplexer PooledClientManager(string serverName)
        {
            var connectionString = ConfigurationHelper.GetRedisConnectString(serverName);
            return Core<ConnectionMultiplexer>.Instance(GetRedisClientManager, connectionString, connectionString);
        }

        /// <summary>
        /// GetRedisClientManager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static ConnectionMultiplexer GetRedisClientManager(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException(connectionString);
            var connect = ConnectionMultiplexer.Connect(connectionString);

            connect.ConnectionFailed += (s, e) =>
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { e.EndPoint, e.FailureType }), e.Exception, LogDomain.Util);
            };

            connect.ConnectionRestored += (s, e) =>
            {
                Logger.Client.Warn(string.Format("ConnectionRestored: {0}.", e.EndPoint), LogDomain.Util);
            };

            connect.ErrorMessage += (s, e) =>
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { e.EndPoint }), new Exception(e.Message), LogDomain.Util);
            };

            connect.ConfigurationChanged += (s, e) =>
            {
                Logger.Client.Warn(string.Format("Configuration changed: {0}", e.EndPoint), LogDomain.Util);
            };

            connect.HashSlotMoved += (s, e) =>
            {
                Logger.Client.Warn(string.Format("HashSlotMoved: NewEndPoint: {0}, OldEndPoint: {1}.", e.NewEndPoint, e.OldEndPoint), LogDomain.Util);
            };

            connect.InternalError += (s, e) =>
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { e.EndPoint, e.ConnectionType }), e.Exception, LogDomain.Util);
            };
            return connect;
        }
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress
        {
            get
            {
                var redis = PooledClientManager(ServerName);
                return string.Join(",", redis.GetEndPoints().Select(p => p.ToString()).ToArray());
            }
        }

        public LogDomain Domain => LogDomain.ThirdParty;

        public IDatabase Client => PooledClientManager(ServerName).GetDatabase(DbNum);

        public ConnectionMultiplexer ConnectionMultiplexer => PooledClientManager(ServerName);
        #endregion
    }
}
