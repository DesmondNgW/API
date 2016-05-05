﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Provider
{
    /// <summary>
    /// WCF client Provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WcfProvider<T> : IWcfProvider<T>
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
        private static TimeSpan ValidTime
        {
            get { return new TimeSpan(2, 0, 0); }
        }
        private static T _instance;
        private OperationContextScope _scope;
        private readonly Stopwatch _sw = new Stopwatch();
        /// <summary>
        /// 缓存Key
        /// </summary>
        private string CacheKey
        {
            get { return EndpointAddress + StringConvert.SysRandom.Next(0, ServiceModel.MaxPoolSize); }
        }

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
                Logger.Client.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, ex.ToString());
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
        public string EndpointAddress
        {
            get { return ServiceModel.EndpointAddress; }
        }

        public T Client
        {
            get
            {
                _instance = Core<T>.Instance(Init, CacheKey, ValidTime, Core<T>.IsValid4CommunicationObject);
                _scope = new OperationContextScope((IClientChannel)_instance);
                var header = MessageHeader.CreateHeader("clientip", "http://tempuri.org", CoreUtil.GetIp());
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
                return _instance;
            }
        }

        public void StartElapsed()
        {
            _sw.Start();
        }

        public void LogElapsed(MethodBase method, LogDomain eDomain)
        {
            if (_scope != null) _scope.Dispose();
            _sw.Stop();
            Core<T>.Close(method, _sw.ElapsedMilliseconds, eDomain, EndpointAddress);
            _sw.Reset();
        }

        public void Dispose(LogDomain eDomain)
        {
            if (_scope != null) _scope.Dispose();
            Core<T>.Dispose(_instance, eDomain);
        }
        #endregion
    }
}
