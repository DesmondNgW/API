using Grpc.Core;
using Grpc.Net.Client;
using System;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    /// <summary>
    /// GrpcProxyPoolProvider
    /// </summary>
    /// <typeparam name="TChannel"></typeparam>
    public sealed class GrpcProxyPoolProvider<TChannel> : IClientProvider<TChannel>, IDisposable where TChannel : ClientBase<TChannel>, new()
    {
        #region 构造函数
        public readonly LogDomain EDomain = LogDomain.Core;
        public readonly IProxyPooled<TChannel> ProxyPooledManager;
        public readonly GrpcChannelOptionsModel GrpcModel = ConfigurationHelper.GetGrpcChannelByName(typeof(TChannel).FullName);
        private IProxyPooled<TChannel> GetProxyPooledManager()
        {
            return new ProxyPooledManager<TChannel>(new ProxyPooledRequest<TChannel>
            {
                PoolSize = 20,
                EndpointAddress = EndpointAddress,
                CreateChannel = CreateChannel,
                ChannelLifeCycle = new TimeSpan(0, 0, 0),
                ChannelRecycle = new TimeSpan(0, 1, 0),
            });
        }

        public GrpcProxyPoolProvider()
        {
            ProxyPooledManager = GetProxyPooledManager();
        }

        public GrpcProxyPoolProvider(LogDomain eDomain)
        {
            EDomain = eDomain;
            ProxyPooledManager = GetProxyPooledManager();
        }
        #endregion

        #region 内部实现
        private static TChannel _instance;
        /// <summary>
        /// CreateChannel
        /// </summary>
        /// <returns></returns>
        private TChannel CreateChannel()
        {
            var channel = GrpcChannel;
            return (TChannel)Activator.CreateInstance(typeof(TChannel), new object[] { channel });
        }

        /// <summary>
        /// 初始化GrpcChannel
        /// </summary>
        private GrpcChannel GrpcChannel
        {
            get { return Core<GrpcChannel>.Instance(() => GrpcChannel.ForAddress(GrpcModel.GrpcAddress, GrpcModel.GrpcChannelOptions), EndpointAddress); }
        }
        #endregion

        #region 对外公开方法和属性
        public string EndpointAddress => GrpcModel.GrpcAddress;

        public LogDomain Domain => EDomain;

        public TChannel Client
        {
            get
            {
                _instance = ProxyPooledManager.GetClient();
                return _instance;
            }
        }

        public void ReleaseClient()
        {
            ProxyPooledManager.ReleaseClient(_instance);
        }

        public void Dispose(LogDomain eDomain)
        {
            ProxyPooledManager.ReleaseClient(_instance);
        }

        public void Dispose()
        {
            Dispose(EDomain);
        }
        #endregion
    }


}
