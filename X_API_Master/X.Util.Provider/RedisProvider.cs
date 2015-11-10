using System.Diagnostics;
using ServiceStack.Redis;
using X.Util.Core;
using System.Reflection;

namespace X.Util.Provider
{
    public sealed class RedisProvider
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
        private readonly Stopwatch _sw = new Stopwatch();
        private static IRedisClient _client = default(RedisClient);
        /// <summary>
        /// 缓冲池管理
        /// </summary>
        private static PooledRedisClientManager PooledClientManager(string serverName)
        {
            return Core<PooledRedisClientManager>.Instance(ConfigurationHelper.GetRedisClientManager, serverName, ConfigurationHelper.EndpointFile + serverName);
        }
        #endregion

        #region 对外公开属性和方法
        /// <summary>
        /// Provider提供的RedisClient实例
        /// </summary>
        public IRedisClient Client
        {
            get
            {
                _client = PooledClientManager(ServerName).GetClient();
                _sw.Start();
                return _client;
            }
        }

        public IRedisClient ReadOnlyClient
        {
            get
            {
                _client = PooledClientManager(ServerName).GetReadOnlyClient();
                _sw.Start();
                return _client;
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close(MethodBase method, LogDomain eDomain)
        {
            _sw.Stop();
            Core<RedisClient>.Close(method, _sw.ElapsedMilliseconds, eDomain);
            _sw.Reset();
        }
        #endregion
    }
}
