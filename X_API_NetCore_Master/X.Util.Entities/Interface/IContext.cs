using System;

namespace X.Util.Entities.Interface
{
    public interface IContext<TResult, out TChannel>
    {
        IProvider<TChannel> Channel { get; }

        Func<TResult> Calling(ActionContext<TResult> context, Func<TResult> caller);

        void Called(ActionContext<TResult> context);

        void OnException(ActionContext<TResult> context);

        int Priority { get; }
    }

    public interface IContext<out TChannel>
    {
        IProvider<TChannel> Channel { get; }

        Action Calling(ActionContext context, Action caller);

        void Called(ActionContext context);

        void OnException(ActionContext context);

        int Priority { get; }
    }
}
