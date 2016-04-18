using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace X.Util.Core
{
    /// <summary>
    /// 方法的委托调用（记录日志以及异常处理）
    /// </summary>
    public sealed class CoreAccess<TS> where TS : Exception
    {
        public static TResult Call<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync(LogDomain eDomain, Action func, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                ((Action)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync(LogDomain eDomain, Action func, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                try
                {
                    ((Action)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult>(LogDomain eDomain, Func<TResult> func, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(result =>
            {
                try
                {
                    var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1>(LogDomain eDomain, Action<T1> func, T1 t1, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                ((Action<T1>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1>(LogDomain eDomain, Action<T1> func, T1 t1, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                try
                {
                    ((Action<T1>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1>(LogDomain eDomain, Func<T1, TResult> func, T1 t1, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, result =>
            {
                try
                {
                    var iresult = ((Func<TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2>(LogDomain eDomain, Action<T1, T2> func, T1 t1, T2 t2, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2 }, address);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                func.BeginInvoke(t1, t2, result =>
                {
                    ((Action<T1, T2>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }, null);
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
        }

        public static void TryCallAsync<T1, T2>(LogDomain eDomain, Action<T1, T2> func, T1 t1, T2 t2, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, result =>
            {
                try
                {
                    ((Action<T1, T2>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, result =>
            {
                var iresult = ((Func<T1, T2, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2>(LogDomain eDomain, Func<T1, T2, TResult> func, T1 t1, T2 t2, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3>(LogDomain eDomain, Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                ((Action<T1, T2, T3>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3>(LogDomain eDomain, Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                try
                {
                    ((Action<T1, T2, T3>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                var iresult = ((Func<T1, T2, T3, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3>(LogDomain eDomain, Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4>(LogDomain eDomain, Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                ((Action<T1, T2, T3, T4>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4>(LogDomain eDomain, Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4>(LogDomain eDomain, Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5>(LogDomain eDomain, Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                ((Action<T1, T2, T3, T4, T5>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5>(LogDomain eDomain, Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static TResult Call<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, Math.Max(1, againTimes));
            releaseClient(func.Method, eDomain);
            if (callSuccess(iresult))
            {
                if (needLogInfo) Logger.Info(method, eDomain, iresult);
            }
            else Logger.Error(method, eDomain, iresult);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            return iresult;
        }

        public static TResult TryCall<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, int againTimes = 1, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, address);
            var result = default(TResult);
            try
            {
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                var iresult = CoreSecurity<TS>.TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, Math.Max(1, againTimes));
                result = iresult;
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            }
            catch (Exception ex)
            {
                Logger.Error(method, eDomain, null, ex.ToString());
            }
            return result;
        }

        public static void CallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke();
            }, null);
        }

        public static void TryCallAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Action<MethodBase, LogDomain> releaseClient, Action callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                try
                {
                    ((Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }

        public static void CallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                releaseClient(func.Method, eDomain);
                if (callSuccess(iresult))
                {
                    if (needLogInfo) Logger.Info(method, eDomain, result);
                }
                else Logger.Error(method, eDomain, result);
                if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                if (callBack != null) callBack.Invoke(iresult);
            }, null);
        }

        public static void TryCallAsync<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(LogDomain eDomain, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, Func<TResult, bool> callSuccess, Action<MethodBase, LogDomain> releaseClient, Action<TResult> callBack, bool needLogInfo = true, string address = null)
        {
            var method = Logger.GetMethodInfo(func.Method, new object[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16 }, address);
            if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} BeginInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
            func.BeginInvoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, result =>
            {
                try
                {
                    var iresult = ((Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)(((AsyncResult)result).AsyncDelegate)).EndInvoke(result);
                    releaseClient(func.Method, eDomain);
                    if (callSuccess(iresult))
                    {
                        if (needLogInfo) Logger.Info(method, eDomain, result);
                    }
                    else Logger.Error(method, eDomain, result);
                    if (needLogInfo) Logger.Info(method, eDomain, null, string.Format("{0}.{1} EndInvoke.", func.Method.DeclaringType.FullName, func.Method.Name));
                    if (callBack != null) callBack.Invoke(iresult);
                }
                catch (Exception ex)
                {
                    Logger.Error(method, eDomain, null, ex.ToString());
                }
            }, null);
        }
    }
}
