using System;
using System.Runtime.Remoting.Messaging;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core
{
    /// <summary>
    /// 方法的委托调用（记录日志以及异常处理）
    /// </summary>
    public sealed class CoreAccess<TChannel>
    {
        public static TResult Call<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func();
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func();
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync(LogDomain eDomain, Action func, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                ((Action)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync(LogDomain eDomain, Action func, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                try
                {
                    ((Action)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                try
                {
                    var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1>(LogDomain eDomain, Action<T1> func, T1 t1, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                ((Action<T1>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1>(LogDomain eDomain, Action<T1> func, T1 t1, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                try
                {
                    ((Action<T1>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                try
                {
                    var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2>(LogDomain eDomain, Action<T1, T2> func, T1 t1, T2 t2, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                func.BeginInvoke(t1, t2, result =>
                {
                    ((Action<T1, T2>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }, null);
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
        }

        public static void TryCallAsync<T1, T2>(LogDomain eDomain, Action<T1, T2> func, T1 t1, T2 t2, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, result =>
            {
                try
                {
                    ((Action<T1, T2>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, result =>
            {
                var iresult = ((Func<T1, T2, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3>(LogDomain eDomain, Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                ((Action<T1, T2, T3>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3>(LogDomain eDomain, Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                try
                {
                    ((Action<T1, T2, T3>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                var iresult = ((Func<T1, T2, T3, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4>(LogDomain eDomain, Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                ((Action<T1, T2, T3, T4>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4>(LogDomain eDomain, Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5>(LogDomain eDomain, Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                ((Action<T1, T2, T3, T4, T5>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5>(LogDomain eDomain, Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            channel.LogElapsed(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, iresult);
            }
            else Logger.Client.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
                result = iresult;
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                var wcf = channel as IWcfProvider<TChannel>;
                if (wcf != null) wcf.Dispose(eDomain);
                Logger.Client.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                channel.LogElapsed(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                }
                else Logger.Client.Error(method, eDomain, result);
                if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    channel.LogElapsed(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, eDomain, result);
                    }
                    else Logger.Client.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Client.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    var wcf = channel as IWcfProvider<TChannel>;
                    if (wcf != null) wcf.Dispose(eDomain);
                    Logger.Client.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }
    }
}
