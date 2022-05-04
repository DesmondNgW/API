using System;
using X.Util.Entities.Interface;

namespace X.Util.Core.Kernel
{
    /// <summary>
    /// 从自定义Attribute中生成IContext对象
    /// </summary>
    public abstract class ContextResultAttribute : Attribute
    {
        public abstract IContext<TResult, TChannel> GetContext<TResult, TChannel>(IProvider<TChannel> channel, Func<TResult, bool> callSuccess);
    }

    /// <summary>
    /// 从自定义Attribute中生成IContext对象
    /// </summary>
    public abstract class ContextAttribute : Attribute
    {
        public abstract IContext<TChannel> GetContext<TChannel>(IProvider<TChannel> channel);
    }
}
