using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using X.Util.Core;
using X.Util.Entities;

namespace X.Util.Provider
{
    /// <summary>
    /// WCF client Provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WcfProvider<T>
    {
        #region 构造函数
        public readonly CoreServiceModel ServiceModel = ServiceModelTool.GetServiceModel<T>();
        public readonly LogDomain EDomain = LogDomain.Core;
        public WcfProvider() { }

        public WcfProvider(LogDomain eDomain)
        {
            EDomain = eDomain;
        }
        #endregion

        #region 内部实现
        private static TimeSpan ValidTime => new TimeSpan(2, 0, 0);
        private static T _instance;
        private readonly Stopwatch _sw = new Stopwatch();
        /// <summary>
        /// 缓存Key
        /// </summary>
        private string CacheKey => EndpointAddress + Thread.CurrentThread.ManagedThreadId % ServiceModel.MaxPoolSize;

        /// <summary>
        /// 初始化ChannelFactory
        /// </summary>
        /// <returns></returns>
        private T Init()
        {
            var channel = default(T);
            try
            {
                channel = Core<CoreChannel<T>>.Instance(() => new CoreChannel<T>(ServiceModel), EndpointAddress, Core<CoreChannel<T>>.IsValid4CommunicationObject).CreateChannel();
                Core<T>.InitChannel(channel, CloseChannel);
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, ex.ToString());
            }
            return channel;
        }

        /// <summary>
        /// 关闭ChannelFactory连接池实例回调方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseChannel(object sender, EventArgs e)
        {
            Core<T>.Dispose((T)sender, EDomain);
        }

        #endregion

        #region 对外公开方法和属性
        /// <summary>
        /// 调用WCF的EndpointAddress
        /// </summary>
        public string EndpointAddress => ServiceModel.EndpointAddress;

        /// <summary>
        /// Provider提供的Channel实例
        /// </summary>
        public T Instance
        {
            get
            {
                _instance = Core<T>.Instance(Init, CacheKey, ValidTime, Core<T>.IsValid4CommunicationObject);
                _sw.Start();
                return _instance;
            }
        }
        /// <summary>
        /// 关闭Channel连接
        /// </summary>
        public void Dispose(MethodBase method, LogDomain eDomain)
        {
            _sw.Stop();
            Core<T>.Close(method, _sw.ElapsedMilliseconds, eDomain, EndpointAddress);
            _sw.Reset();
        }
        #endregion
    }
}
