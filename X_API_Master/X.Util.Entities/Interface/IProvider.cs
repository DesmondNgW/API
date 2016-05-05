using System.Reflection;

namespace X.Util.Entities.Interface
{
    public interface IProvider<out T>
    {
        /// <summary>
        /// 实例
        /// </summary>
        T Client { get; }

        /// <summary>
        /// 记录耗时
        /// </summary>
        /// <param name="method"></param>
        /// <param name="eDomain"></param>
        void LogElapsed(MethodBase method, LogDomain eDomain);

        /// <summary>
        /// 远程uri地址
        /// </summary>
        string EndpointAddress { get; }
    }

    public interface IWcfProvider<out T> : IProvider<T>
    {
        /// <summary>
        /// 回收实例
        /// </summary>
        void Dispose(LogDomain eDomain);
    }
}
