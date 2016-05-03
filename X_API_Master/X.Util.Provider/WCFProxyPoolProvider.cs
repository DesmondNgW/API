﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using X.Util.Core;
using X.Util.Entities;

namespace X.Util.Provider
{
    /// <summary>
    /// WCF ClientPool Provider
    /// </summary>
    /// <typeparam name="TChannel"></typeparam>
    public sealed class WcfProxyPoolProvider<TChannel> : IWcfProvider<TChannel>
    {
        #region 构造函数
        public readonly LogDomain EDomain = LogDomain.Core;
        public readonly CoreServiceModel ServiceModel = ServiceModelTool.GetServiceModel<TChannel>();
        public WcfProxyPoolProvider() { }

        public WcfProxyPoolProvider(LogDomain eDomain)
        {
            EDomain = eDomain;
        }
        #endregion

        #region 内部实现
        private readonly Stopwatch _sw = new Stopwatch();
        private static TChannel _instance;
        private OperationContextScope _scope;
        private static TimeSpan ValidTime
        {
            get { return new TimeSpan(0, 10, 0); }
        }
        private bool _channelFromCache = true;
        /// <summary>
        /// 缓存Key
        /// </summary>
        private string CacheKey
        {
            get { return EndpointAddress + "_" + Size; }
        }

        /// <summary>
        /// 连接池Size
        /// </summary>
        private int Size
        {
            get { return Math.Max(ServiceModel.MaxPoolSize, 20); }
        }

        /// <summary>
        /// 初始化单个ChannelFactory
        /// </summary>
        private CoreChannel<TChannel> CoreChannelFactory
        {
            get { return Core<CoreChannel<TChannel>>.Instance(() => new CoreChannel<TChannel>(ServiceModel), EndpointAddress, Core<CoreChannel<TChannel>>.IsValid4CommunicationObject); }
        }
        /// <summary>
        /// 初始化ChannelFactory连接池
        /// </summary>
        /// <returns></returns>
        private CoreChannelFactoryPool<TChannel> InitCoreFactoryPool()
        {
            var result = new CoreChannelFactoryPool<TChannel> { ServiceUri = EndpointAddress, Size = Size, ContextQueue = new ConcurrentQueue<ContextChannel<TChannel>>() };
            try
            {
                for (var i = 0; i < ServiceModel.MaxPoolSize; i++)
                {
                    var channel = CoreChannelFactory.CreateChannel();
                    Core<TChannel>.InitChannel(channel, CloseChannel);
                    result.ContextQueue.Enqueue(new ContextChannel<TChannel> { ChannelClosedTime = DateTime.Now.Add(ValidTime), Channel = channel });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// ChannelFactory连接池实例
        /// </summary>
        private CoreChannelFactoryPool<TChannel> CoreFactoryPool
        {
            get { return Core<CoreChannelFactoryPool<TChannel>>.Instance(InitCoreFactoryPool, CacheKey); }
        }

        /// <summary>
        /// 关闭Channel实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseChannel(object sender, EventArgs e)
        {
            var channel = (TChannel)sender;
            Core<TChannel>.Dispose(channel, EDomain);
            ReleaseClient(channel);
        }

        /// <summary>
        /// 从连接池取Channel实例
        /// </summary>
        /// <returns></returns>
        private TChannel GetClient()
        {
            var channel = default(TChannel);
            try
            {
                ContextChannel<TChannel> contextChannel;
                if (CoreFactoryPool.ContextQueue.TryDequeue(out contextChannel) && contextChannel != null)
                {
                    var unCreateChannel = contextChannel.ChannelClosedTime > DateTime.Now && Core<TChannel>.IsValid4CommunicationObject(contextChannel.Channel);
                    channel = contextChannel.Channel;
                    if (!unCreateChannel)
                    {
                        channel = CoreChannelFactory.CreateChannel();
                        Core<TChannel>.InitChannel(channel, CloseChannel);
                    }
                    _channelFromCache = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
            }
            return channel;
        }
        /// <summary>
        /// 把Channel实例回收到连接池
        /// </summary>
        /// <param name="channel"></param>
        private void ReleaseClient(TChannel channel)
        {
            try
            {
                if (_channelFromCache) return;
                if (!Core<TChannel>.IsValid4CommunicationObject(channel))
                {
                    channel = CoreChannelFactory.CreateChannel();
                    Core<TChannel>.InitChannel(channel, CloseChannel);
                }
                CoreFactoryPool.ContextQueue.Enqueue(new ContextChannel<TChannel> { Channel = channel, ChannelClosedTime = DateTime.Now.Add(ValidTime) });
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
            }
        }
        #endregion

        #region 对外公开方法和属性
        /// <summary>
        /// 调用WCF的EndpointAddress
        /// </summary>
        public string EndpointAddress
        {
            get { return ServiceModel.EndpointAddress; }
        }

        /// <summary>
        /// Provider提供的Channel实例
        /// </summary>
        public TChannel Client
        {
            get
            {
                _instance = Core<TChannel>.Instance(GetClient, CacheKey, new TimeSpan(0, 1, 0), Core<TChannel>.IsValid4CommunicationObject);
                _scope = new OperationContextScope((IClientChannel)_instance);
                var header = MessageHeader.CreateHeader("clientip", "http://tempuri.org", CoreUtil.GetIp());
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                _scope.Dispose();
                _sw.Start();
                return _instance;
            }
        }
        /// <summary>
        /// 关闭Channel实例（不回收）
        /// </summary>
        public void Close(MethodBase method, LogDomain eDomain)
        {
            if (_scope != null) _scope.Dispose();
            _sw.Stop();
            Core<TChannel>.Close(method, _sw.ElapsedMilliseconds, eDomain, EndpointAddress);
            _sw.Reset();
            ReleaseClient(_instance);
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="eDomain"></param>
        public void Dispose(LogDomain eDomain)
        {
            if (_scope != null) _scope.Dispose();
            Core<TChannel>.Dispose(_instance, eDomain);
        }
        #endregion
    }
}
