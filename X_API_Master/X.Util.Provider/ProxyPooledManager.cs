using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    public class ProxyPooledManager<TChannel> : IProxyPooled<TChannel>
    {
        public readonly ProxyPooledRequest<TChannel> Request;
        public readonly string CacheKey;

        public ProxyPooledManager(ProxyPooledRequest<TChannel> request)
        {
            if (request == null) throw new ArgumentNullException();
            if (request.PoolSize <= 0) throw new Exception("PoolSize must big than 0.");
            if (string.IsNullOrEmpty(request.EndpointAddress)) throw new Exception("EndpointAddress is uri.");
            if (request.CreateChannel == null) throw new Exception("CreateChannel is method.");
            if (request.ChannelIsVail == null) request.ChannelIsVail = channel => true;
            Request = request;
            CacheKey = string.Format("{0}_{1}", Request.EndpointAddress, Request.PoolSize);
        }

        private CoreChannelFactoryPool<TChannel> InitCoreFactoryPool()
        {
            var result = new CoreChannelFactoryPool<TChannel> { ServiceUri = Request.EndpointAddress, Size = Request.PoolSize, ContextQueue = new ConcurrentQueue<ContextChannel<TChannel>>(), EventWait = new ManualResetEvent(false) };
            try
            {
                for (var i = 0; i < Request.PoolSize; i++)
                {
                    var channel = Request.CreateChannel();
                    if (Request.InitChannel != null) Request.InitChannel(channel, ReleaseClient);
                    result.ContextQueue.Enqueue(new ContextChannel<TChannel> { ChannelClosedTime = DateTime.Now.Add(Request.ChannelLifeCycle), Channel = channel });
                }
            }
            catch (Exception ex)
            {
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
            }
            return result;
        }

        public CoreChannelFactoryPool<TChannel> CoreFactoryPool
        {
            get { return Core<CoreChannelFactoryPool<TChannel>>.Instance(InitCoreFactoryPool, CacheKey); }
        }

        public TChannel GetClient()
        {
            var now = DateTime.Now;
            while (true)
            {
                var channel = default(TChannel);
                try
                {
                    ContextChannel<TChannel> contextChannel;
                    if (CoreFactoryPool.ContextQueue.TryDequeue(out contextChannel) && contextChannel != null)
                    {
                        var channelIsVail = contextChannel.ChannelClosedTime > DateTime.Now && Request.ChannelIsVail(contextChannel.Channel);
                        if (channelIsVail) return contextChannel.Channel;
                        channel = Request.CreateChannel();
                        if (Request.InitChannel != null) Request.InitChannel(channel, ReleaseClient);
                        return channel;
                    }
                    CoreFactoryPool.EventWait.Reset();
                    CoreFactoryPool.EventWait.WaitOne();
                    if (!((DateTime.Now - now).TotalSeconds > 60)) continue;
                    if (CoreFactoryPool.ContextQueue.Count < 2 * Request.PoolSize) ReleaseClient(channel);
                    return channel;
                }
                catch (Exception ex)
                {
                    CoreFactoryPool.EventWait.Set();
                    Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
                    return channel;
                }
            }
        }

        public void ReleaseClient(TChannel channel)
        {
            try
            {
                if (!Request.ChannelIsVail(channel)) channel = Request.CreateChannel();
                CoreFactoryPool.ContextQueue.Enqueue(new ContextChannel<TChannel> { Channel = channel, ChannelClosedTime = DateTime.Now.Add(Request.ChannelLifeCycle) });
                CoreFactoryPool.EventWait.Set();
            }
            catch (Exception ex)
            {
                CoreFactoryPool.EventWait.Set();
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
            }
        }
    }
}
