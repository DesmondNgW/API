using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace X.Util.Core
{
    /// <summary>
    /// 队列延迟处理
    /// </summary>
    /// <typeparam name="T">处理方法的参数</typeparam>
    public class CoreQueue<T>
    {
        private static readonly IDictionary<Action<T>, ConcurrentQueue<T>> ContextDictionary = new Dictionary<Action<T>, ConcurrentQueue<T>>();
        private const string LockerPrefix = "X.Util.Core.CoreQueue.Prefix";
        /// <summary>
        /// 延迟队列处理方法
        /// </summary>
        private Action<T> Method { get; }

        /// <summary>
        /// 延迟队列
        /// </summary>
        /// <param name="method">延迟队列处理方法</param>
        /// <param name="sleep">睡眠时间</param>
        public CoreQueue(Action<T> method, int sleep)
        {
            if (Equals(method, null)) return;
            if (ContextDictionary.ContainsKey(method)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof (T).FullName))
            {
                if (ContextDictionary.ContainsKey(method)) return;
                Method = method;
                ContextDictionary[method] = new ConcurrentQueue<T>();
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        T context;
                        if (ContextDictionary[method].TryDequeue(out context))
                        {
                            method.BeginInvoke(context, null, null);
                        }
                        Thread.Sleep(sleep);
                    }
                }) {IsBackground = true};
                thread.Start();
            }
        }

        public void Enqueue(T context)
        {
            if (Equals(Method, null)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof(T).FullName))
            {
                ContextDictionary[Method].Enqueue(context);
            }
        }
    }
}
