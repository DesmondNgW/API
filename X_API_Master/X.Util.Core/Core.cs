using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using X.Util.Entities;

namespace X.Util.Core
{
    public class Core<T>
    {
        #region 内部实现
        private static volatile IDictionary<string, StatusCacheResult<T>> _cacheResult = new Dictionary<string, StatusCacheResult<T>>();
        private const string CoreDefaultCachekey = "CoreDefaultCacheKey";
        private const string LockerPrefix = "X.Util.Core.Prefix";
        private const int DayOfHour = 24;

        /// <summary>
        /// 缓存存在性
        /// </summary>
        /// <returns></returns>
        private static bool ContainsCache(string key, Func<T, bool> validState = null)
        {
            if (Equals(validState, null)) validState = T => true;
            try
            {
                return _cacheResult[key].Result != null && DateTime.Now < _cacheResult[key].ExpiryDate && validState(_cacheResult[key].Result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// CommunicationObject对象的状态是否可用
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool IsValid4CommunicationObject(T channel)
        {
            try
            {
                var communicationObject = (ICommunicationObject) channel;
                var state = communicationObject != null && communicationObject.State != CommunicationState.Closed && communicationObject.State != CommunicationState.Faulted && communicationObject.State != CommunicationState.Closing;
                if (state) communicationObject.Open();
                return state;
            }
            catch (Exception e)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), LogDomain.Util, null, string.Empty, e.ToString());
                return false;
            }
        }

        /// <summary>
        /// ICommunicationObject
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="closeChannel"></param>
        public static void InitChannel(T channel, EventHandler closeChannel)
        {
            var ico = (ICommunicationObject)channel;
            ico.Faulted += closeChannel;
            ico.Closed += closeChannel;
            ico.Closing += closeChannel;
            if (CommunicationState.Created.Equals(ico.State)) ico.Open();
        }

        /// <summary>
        /// CreateChannel for nettcp
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static T CreateNetTcpChannel(string uri)
        {
            return new ChannelFactory<T>(new NetTcpBinding(SecurityMode.None)
            {
                ReceiveTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                CloseTimeout = new TimeSpan(0, 0, 10),
                TransferMode = TransferMode.Streamed,
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                {
                    MaxDepth = int.MaxValue,
                    MaxStringContentLength = int.MaxValue,
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                }
            }, new EndpointAddress(uri)).CreateChannel();
        }

        /// <summary>
        /// 获取有效可用的Channel
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static T GetNetTcpChannel(string uri, T channel)
        {
            return IsValid4CommunicationObject(channel) ? channel : CreateNetTcpChannel(uri);
        }
        #endregion

        #region Debug模式，记录方法调用时间

        public static void Close(MethodBase method, long elapsedMilliseconds, LogDomain edomain, string address = null)
        {
            if (method.DeclaringType != null) Logger.Debug(method, edomain, null, address, string.Format(@"{0}.{1} finished, used {2} ms.", method.DeclaringType.FullName, method.Name, elapsedMilliseconds));
        }

        public static void Dispose(T channel, LogDomain edomain)
        {
            var communicate = (ICommunicationObject)channel;
            if (Equals(communicate, null)) return;
            try
            {
                communicate.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod(), edomain, null, string.Empty, ex.ToString());
                communicate.Abort();
            }
        }
        #endregion

        #region Get Instance
        public static T Instance(Func<T> init, string cacheKey, TimeSpan validTime, Func<T, bool> validState = null)
        {
            cacheKey += typeof (T).FullName;
            if (ContainsCache(cacheKey, validState)) return _cacheResult[cacheKey].Result;
            lock (CoreUtil.Getlocker(LockerPrefix + cacheKey))
            {
                if (!ContainsCache(cacheKey, validState))
                    _cacheResult[cacheKey] = new StatusCacheResult<T>
                    {
                        Result = init(),
                        CacheKey = cacheKey,
                        ExpiryDate = DateTime.Now.Add(validTime)
                    };
            }
            return _cacheResult[cacheKey].Result;
        }

        public static T Instance(Func<string, T> init, string argument, string cacheKey, TimeSpan validTime, Func<T, bool> validState = null)
        {
            var key = cacheKey + typeof (T).FullName;
            if (ContainsCache(key, validState)) return _cacheResult[key].Result;
            lock (CoreUtil.Getlocker(LockerPrefix + key))
            {
                if (!ContainsCache(key, validState))
                    _cacheResult[key] = new StatusCacheResult<T>
                    {
                        Result = init(argument),
                        CacheKey = key,
                        ExpiryDate = DateTime.Now.Add(validTime)
                    };
            }
            return _cacheResult[key].Result;
        }

        public static T Instance(Func<string, T> init, string argument, string cacheKey, Func<T, bool> validState = null)
        {
            return Instance(init, argument, cacheKey, new TimeSpan(DayOfHour, 0, 0), validState);
        }

        public static T Instance(Func<T> init, string cacheKey, Func<T, bool> validState = null)
        {
            return Instance(init, cacheKey, new TimeSpan(DayOfHour, 0, 0), validState);
        }

        public static T Instance(Func<string, T> init, string argument, TimeSpan validTime, Func<T, bool> validState = null)
        {
            return Instance(init, argument, CoreDefaultCachekey, validTime, validState);
        }

        public static T Instance(Func<T> init, TimeSpan validTime, Func<T, bool> validState = null)
        {
            return Instance(init, CoreDefaultCachekey, validTime, validState);
        }

        public static T Instance(Func<string, T> init, string argument, Func<T, bool> validState = null)
        {
            return Instance(init, argument, CoreDefaultCachekey, new TimeSpan(DayOfHour, 0, 0), validState);
        }

        public static T Instance(Func<T> init, Func<T, bool> validState = null)
        {
            return Instance(init, CoreDefaultCachekey, new TimeSpan(DayOfHour, 0, 0), validState);
        }
        #endregion
    }
}
