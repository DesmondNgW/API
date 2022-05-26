using System;
using X.Util.Entities;
using X.Util.Entities.Enum;
using X.Util.Entities.Interface;

namespace X.Util.Extend.Core
{
    public class CacheContext<TResult, TChannel> : IContext<TResult, TChannel>
    {
        private readonly IProvider<TChannel> _channel;
        public readonly CacheOptions Options;
        public readonly Func<TResult, bool> CallSuccess;

        public CacheContext(IProvider<TChannel> channel, Func<TResult, bool> callSuccess, CacheOptions options)
        {
            _channel = channel;
            Options = options;
            CallSuccess = callSuccess ?? (t => true);
        }

        public IProvider<TChannel> Channel => _channel;

        private Func<CacheResult<TResult>> Convert(Func<TResult> caller)
        {
            return () =>
            {
                var iresult = caller();
                return new CacheResult<TResult>
                {
                    Succeed = CallSuccess(iresult),
                    Result = iresult
                };
            };
        }

        private static Func<TResult> Convert(Func<CacheResult<TResult>> caller)
        {
            return () =>
            {
                var iresult = caller();
                return iresult != null ? iresult.Result : default;
            };
        }


        public Func<TResult> Calling(ActionContext<TResult> context, Func<TResult> caller)
        {
            switch (Options.CacheExpireType)
            {
                case EnumCacheExpireType.Absolute:
                    return Convert(() => CoreCache.Default.GetAbsoluteCacheData(Convert(caller),
                        context.Request.Method,
                        context.Request.ActionArguments,
                        Options.CacheAppVersion,
                        Options.DebugWithoutCache,
                        Options.AddContext,
                        Options.CacheType,
                        Options.CacheTimeLevel,
                        Options.CacheTimeExpire));
                case EnumCacheExpireType.Sliding:
                    return Convert(() => CoreCache.Default.GetSlidingCacheData(Convert(caller),
                        context.Request.Method,
                        context.Request.ActionArguments,
                        Options.CacheAppVersion,
                        Options.DebugWithoutCache,
                        Options.AddContext,
                        Options.CacheType,
                        Options.CacheTimeLevel,
                        Options.CacheTimeExpire));
            }
            return caller;
        }

        public void Called(ActionContext<TResult> context) { }

        public void OnException(ActionContext<TResult> context) { }

        public int Priority => 0;
    }
}
