using Couchbase;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
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

        public LogDomain Domain
        {
            get { return LogDomain.ThirdParty; }
        }

        public ICouchbaseClient Client
        {
            get
            {
                return Core<CouchbaseClient>.Instance(Init, ServerName, ConfigurationHelper.EndpointFile + ServerName);
            }
        }
        #endregion
    }
}
