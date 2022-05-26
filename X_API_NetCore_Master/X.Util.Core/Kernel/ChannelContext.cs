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
            if (Channel is IClientProvider<TChannel> channel) channel.ReleaseClient();
        }

        public void OnException(ActionContext<TResult> context)
        {
            if (!(Channel is IClientProvider<TChannel> channel)) return;
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
            if (Channel is IClientProvider<TChannel> channel) channel.ReleaseClient();
        }

        public void OnException(ActionContext context)
        {
            if (!(Channel is IClientProvider<TChannel> channel)) return;
            channel.Dispose(channel.Domain);
        }

        public int Priority => int.MaxValue;
    }
}
