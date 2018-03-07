using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using X.Util.Core.Configuration;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    /// <summary>
    /// WCF ClientPool Provider
    /// </summary>
    /// <typeparam name="TChannel"></typeparam>
    public sealed class WcfProxyPoolProvider<TChannel> : IClientProvider<TChannel>, IDisposable
    {
        #region 构造函数
        public readonly LogDomain EDomain = LogDomain.Core;
        public readonly IProxyPooled<TChannel> ProxyPooledManager;
        public readonly CoreServiceModel ServiceModel = ServiceModelTool.GetServiceModel<TChannel>();
        private IProxyPooled<TChannel> GetProxyPooledManager()
        {
            return new ProxyPooledManager<TChannel>(new ProxyPooledRequest<TChannel>
            {
                PoolSize = Math.Max(ServiceModel.MaxPoolSize, 20),
                EndpointAddress = EndpointAddress,
                CreateChannel = CoreChannelFactory.CreateChannel,
                InitChannel = InitChannel,
                CloseChannel = CloseChannel,
                ChannelLifeCycle = new TimeSpan(0, 0, 0),
                ChannelRecycle = new TimeSpan(0, 1, 0),
                ChannelIsVail = Core<TChannel>.IsValid4CommunicationObject
            });
        }

        public WcfProxyPoolProvider()
        {
            ProxyPooledManager = GetProxyPooledManager();
        }

        public WcfProxyPoolProvider(LogDomain eDomain)
        {
            EDomain = eDomain;
            ProxyPooledManager = GetProxyPooledManager();
        }
        #endregion

        #region 内部实现
        private static TChannel _instance;
        private OperationContextScope _scope;
        private void InitChannel(TChannel channel, Action<TChannel> releaseClient)
        {
            Core<TChannel>.InitChannel(channel, (sender, e) =>
            {
                var obj = (TChannel)sender;
                Core<TChannel>.Dispose(obj, EDomain);
                releaseClient(obj);
            });
        }

        private void CloseChannel(TChannel channel)
        {
            Core<TChannel>.Dispose(channel, EDomain, true);
        }

        /// <summary>
        /// 初始化单个ChannelFactory
        /// </summary>
        private CoreChannel<TChannel> CoreChannelFactory
        {
            get { return Core<CoreChannel<TChannel>>.Instance(() => new CoreChannel<TChannel>(ServiceModel), EndpointAddress, Core<CoreChannel<TChannel>>.IsValid4CommunicationObject); }
        }
        #endregion

        #region 对外公开方法和属性
        public string EndpointAddress
        {
            get { return ServiceModel.EndpointAddress; }
        }

        public LogDomain Domain
        {
            get { return EDomain; }
        }

        public TChannel Client
        {
            get
            {
                _instance = ProxyPooledManager.GetClient();
                _scope = new OperationContextScope((IClientChannel)_instance);
                var header = MessageHeader.CreateHeader("clientip", "http://tempuri.org", IpBase.GetIp());
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                return _instance;
            }
        }

        public void ReleaseClient()
        {
            if (_scope != null) _scope.Dispose();
            ProxyPooledManager.ReleaseClient(_instance);
        }

        public void Dispose(LogDomain eDomain)
        {
            if (_scope != null) _scope.Dispose();
            Core<TChannel>.Dispose(_instance, eDomain, true);
            ProxyPooledManager.ReleaseClient(_instance);
        }

        public void Dispose()
        {
            Dispose(EDomain);
        }
        #endregion
    }
}
