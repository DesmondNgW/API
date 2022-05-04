using System;

namespace X.Util.Entities
{
    public class ProxyPooledRequest<TChannel>
    {
        public int PoolSize { get; set; }

        public string EndpointAddress { get; set; }

        public Func<TChannel> CreateChannel { get; set; }

        public Action<TChannel> CloseChannel { get; set; }

        public Action<TChannel, Action<TChannel>> InitChannel { get; set; }

        public Func<TChannel, bool> ChannelIsVail { get; set; }

        /// <summary>
        /// 通道生命周期,0=>使用一次
        /// </summary>
        public TimeSpan ChannelLifeCycle { get; set; }

        /// <summary>
        /// 通道回收周期
        /// </summary>
        public TimeSpan ChannelRecycle { get; set; }
    }
}
