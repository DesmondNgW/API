namespace X.Util.Entities.Interface
{
    public interface IContext<TResult, out TChannel>
    {
        IProvider<TChannel> Channel { get; }

        void Calling(ActionContext<TResult> context);

        void Called(ActionContext<TResult> context);

        void OnException(ActionContext<TResult> context);

        int Priority { get; }
    }

    public interface IContext<out TChannel>
    {
        IProvider<TChannel> Channel { get; }

        void Calling(ActionContext context);

        void Called(ActionContext context);

        void OnException(ActionContext context);

        int Priority { get; }
    }
}
