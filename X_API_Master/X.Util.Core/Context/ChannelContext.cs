using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core.Context
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

        public void Calling(ActionContext<TResult> context) { }

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

        public void Calling(ActionContext context) { }

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
    }
}
