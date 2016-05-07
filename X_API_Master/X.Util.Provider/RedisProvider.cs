using ServiceStack.Redis;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public sealed class RedisProvider : IProvider<IRedisClient>
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.RedisDefaultServername;
        public RedisProvider() { }
        public RedisProvider(string serverName)
        {
            ServerName = serverName;
        }
        #endregion

        #region 内部实现
        /// <summary>
        /// 缓冲池管理
        /// </summary>
        private static PooledRedisClientManager PooledClientManager(string serverName)
        {
            return Core<PooledRedisClientManager>.Instance(ConfigurationHelper.GetRedisClientManager, serverName, ConfigurationHelper.EndpointFile + serverName);
        }
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress
        {
            get { return ServerName; }
        }

        public LogDomain Domain
        {
            get { return LogDomain.ThirdParty; }
        }

        public IRedisClient Client
        {
            get
            {
                return PooledClientManager(ServerName).GetClient();
            }
        }

        public IRedisClient ReadOnlyClient
        {
            get
            {
                return PooledClientManager(ServerName).GetReadOnlyClient();
            }
        }
        #endregion
    }
}
