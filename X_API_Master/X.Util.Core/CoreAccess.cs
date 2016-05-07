using System;
using System.Runtime.Remoting.Messaging;
using X.Util.Entities.Interface;

namespace X.Util.Core
{
    /// <summary>
    /// 方法的委托调用（记录日志以及异常处理）
    /// </summary>
    public sealed class CoreAccess<TChannel>
    {
        public static TResult Call<TResult>(Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func();
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult>(Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
            try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func();
                result = iresult;
                stopElapsed();
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), channel.Client, func.Method), callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync(Action func, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(result =>
            {
                ((Action)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync(Action func, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(result =>
            {
                var client = channel as IClientProvider<TChannel>;
                try
                {
                    ((Action)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed();
                    if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action)Delegate.CreateDelegate(typeof(Action), channel.Client, func.Method), channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult>(Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(result =>
            {
                var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult>(Func<TResult> func, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(result =>
            {
                var client = channel as IClientProvider<TChannel>;
                try
                {
                    var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed();
                    if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), channel.Client, func.Method), callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1>(Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1>(Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
            try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1);
                result = iresult;
                stopElapsed();
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, TResult>)Delegate.CreateDelegate(typeof(Func<T1, TResult>), channel.Client, func.Method), t1, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1>(Action<T1> func, T1 t1, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, result =>
            {
                ((Action<T1>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1>(Action<T1> func, T1 t1, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, result =>
            {
                var client = channel as IClientProvider<TChannel>;
                try
                {
                    ((Action<T1>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1>)Delegate.CreateDelegate(typeof(Action<T1>), channel.Client, func.Method), t1, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1>(Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, result =>
            {
                var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1>(Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, result =>
            {
                var client = channel as IClientProvider<TChannel>; 
                try
                {
                    var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, TResult>)Delegate.CreateDelegate(typeof(Func<T1, TResult>), channel.Client, func.Method), t1, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
            try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, TResult>), channel.Client, func.Method), t1, t2, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2>(Action<T1, T2> func, T1 t1, T2 t2, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            var client = channel as IClientProvider<TChannel>;
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, result =>
            {
                ((Action<T1, T2>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2>(Action<T1, T2> func, T1 t1, T2 t2, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2>)Delegate.CreateDelegate(typeof(Action<T1, T2>), channel.Client, func.Method), t1, t2, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, result =>
            {
                var iresult = ((Func<T1, T2, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, TResult>), channel.Client, func.Method), t1, t2, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, TResult>), channel.Client, func.Method), t1, t2, t3, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3>(Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, result =>
            {
                ((Action<T1, T2, T3>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3>(Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3>), channel.Client, func.Method), t1, t2, t3, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, result =>
            {
                var iresult = ((Func<T1, T2, T3, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, TResult>), channel.Client, func.Method), t1, t2, t3, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, TResult>), channel.Client, func.Method), t1, t2, t3, t4, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                ((Action<T1, T2, T3, T4>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4>), channel.Client, func.Method), t1, t2, t3, t4, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, TResult>), channel.Client, func.Method), t1, t2, t3, t4, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                ((Action<T1, T2, T3, T4, T5>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5>), channel.Client, func.Method), t1, t2, t3, t4, t5, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11,callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, callSuccess,channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14,callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            stopElapsed();
            var client = channel as IClientProvider<TChannel>;
            if (client != null) client.ReleaseClient();
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, iresult);
            }
            else Logger.Client.Error(method, channel.Domain, iresult);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            var result = default(TResult);
            var client = channel as IClientProvider<TChannel>;
			try
            {
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
                var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
                var iresult = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
                result = iresult;
                stopElapsed(); if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                if (client == null) return result;
                client.Dispose(channel.Domain);
                return TryCall((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, callSuccess, channel, needLogInfo);
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, IProvider<TChannel> channel, Action callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, channel, callBack, needLogInfo);
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                stopElapsed();
                var client = channel as IClientProvider<TChannel>;
                if (client != null) client.ReleaseClient();
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                }
                else Logger.Client.Error(method, channel.Domain, result);
                if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, IProvider<TChannel> channel, Action<TResult> callBack, bool needLogInfo = true)
        {
            var method = Logger.Client.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, channel.EndpointAddress);
            if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", typeof(TChannel).FullName, func.Method.Name));
            var stopElapsed = Logger.Client.GetStopElapsed(func.Method, channel.Domain, channel.EndpointAddress);
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                var client = channel as IClientProvider<TChannel>;
				try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    stopElapsed(); if (client != null) client.ReleaseClient();
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Client.Info(method, channel.Domain, result);
                    }
                    else Logger.Client.Error(method, channel.Domain, result);
                    if (needLogInfo) Logger.Client.Info(method, channel.Domain, null, string.Format("{0}.{1} EndInvoke.", typeof(TChannel).FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Client.Error(method, channel.Domain, null, ex.ToString());
                    if (client == null) return;
                    client.Dispose(channel.Domain);
                    TryCallAsync((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>), channel.Client, func.Method), t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, callSuccess, channel, callBack, needLogInfo);
                }
            }, null);
        }
    }
}
