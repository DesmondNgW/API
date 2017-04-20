using System;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core.Kernel
{
    public class ChannelContext<TResult, TChannel> : IContext<TResult, TChannel>
    {
        private readonly IProvider<TChannel> _channel;

        public ChannelContext(IProvider<TChannel> channel)
        {
            _channel = channel;
        }

        public IProvider<TChannel> Channel
        {
            get { return _channel; }
        }

        public Func<TResult> Calling(ActionContext<TResult> context, Func<TResult> caller)
        {
            return caller;
        }

        public void Called(ActionContext<TResult> context)
        {
            var channel = Channel as IClientProvider<TChannel>;
            if (channel != null) channel.ReleaseClient();
        }

        public void OnException(ActionContext<TResult> context)
        {
            var channel = Channel as IClientProvider<TChannel>;
            if (channel == null) return;
            channel.Dispose(channel.Domain);
        }

        public int Priority { get { return int.MaxValue; } }
    }

    public class ChannelContext<TChannel> : IContext<TChannel>
    {
        private readonly IProvider<TChannel> _channel;

        public ChannelContext(IProvider<TChannel> channel)
        {
            _channel = channel;
        }

        public IProvider<TChannel> Channel
        {
            get { return _channel; }
        }

        public Action Calling(ActionContext context, Action caller)
        {
            return caller;
        }

        public void Called(ActionContext context)
        {
            var channel = Channel as IClientProvider<TChannel>;
            if (channel != null) channel.ReleaseClient();
        }

        public void OnException(ActionContext context)
        {
            var channel = Channel as IClientProvider<TChannel>;
            if (channel == null) return;
            channel.Dispose(channel.Domain);
        }

        public int Priority { get { return int.MaxValue; } }
    }
}
