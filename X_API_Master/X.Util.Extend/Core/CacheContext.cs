using System;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Extend.Core
{
    public class CacheContext<TResult, TChannel> : IContext<TResult, TChannel>
    {
        private readonly IProvider<TChannel> _channel;
        public readonly CacheOptions Options;

        public CacheContext(IProvider<TChannel> channel, CacheOptions options)
        {
            _channel = channel;
            Options = options;
        }

        public IProvider<TChannel> Channel
        {
            get { return _channel; }
        }

        public Func<TResult> Calling(ActionContext<TResult> context, Func<TResult> caller)
        {
            throw new NotImplementedException();
        }

        public void Called(ActionContext<TResult> context) { }

        public void OnException(ActionContext<TResult> context) { }

        public int Priority { get { return 0; } }
    }
}
