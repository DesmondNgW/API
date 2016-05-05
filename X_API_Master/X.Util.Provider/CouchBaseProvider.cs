using Couchbase;
using System.Diagnostics;
using System.Reflection;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public sealed class CouchBaseProvider : IProvider<ICouchbaseClient>
    {
        #region 构造函数
        public readonly string ServerName = ConfigurationHelper.CouchDefaultServername;
        public CouchBaseProvider() { }
        public CouchBaseProvider(string serverName)
        {
            ServerName = serverName;
        }
        #endregion

        #region 内部实现
        private readonly Stopwatch _sw = new Stopwatch();
        private static CouchbaseClient _client = default(CouchbaseClient);
        /// <summary>
        /// 初始化CouchbaseClient
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private static CouchbaseClient Init(string serverName)
        {
            return new CouchbaseClient(ConfigurationHelper.CouchbaseConfiguration(serverName));
        }
        #endregion

        #region 对外公开属性和方法
        public string EndpointAddress
        {
            get { return ServerName; }
        }
        /// <summary>
        /// Provider提供的CouchbaseClient实例
        /// </summary>
        public ICouchbaseClient Client
        {
            get
            {
                _client = Core<CouchbaseClient>.Instance(Init, ServerName, ConfigurationHelper.EndpointFile + ServerName);
                _sw.Start();
                return _client;
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void LogElapsed(MethodBase method, LogDomain eDomain)
        {
            _sw.Stop();
            Core<CouchbaseClient>.Close(method, _sw.ElapsedMilliseconds, eDomain);
            _sw.Reset();
        }
        #endregion
    }
}
