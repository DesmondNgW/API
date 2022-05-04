

namespace X.Util.Entities.Interface
{
    public interface IProvider<out T>
    {
        /// <summary>
        /// 实例
        /// </summary>
        T Client { get; }

        /// <summary>
        /// LogDomain
        /// </summary>
        LogDomain Domain { get; }

        /// <summary>
        /// 远程uri地址
        /// </summary>
        string EndpointAddress { get; }
    }

    public interface IClientProvider<out T> : IProvider<T>
    {
        /// <summary>
        /// 销毁实例
        /// </summary>
        void Dispose(LogDomain eDomain);

        /// <summary>
        /// 回收实例
        /// </summary>
        void ReleaseClient();
    }
}
