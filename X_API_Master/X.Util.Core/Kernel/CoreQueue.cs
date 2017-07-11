using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using X.Util.Entities;

namespace X.Util.Core.Kernel
{
    /// <summary>
    /// 队列延迟处理
    /// </summary>
    /// <typeparam name="T">处理方法的参数</typeparam>
    public class CoreQueue<T>
    {
        private static readonly IDictionary<string, QueueContext<T>> Queue = new Dictionary<string, QueueContext<T>>();
        private const string LockerPrefix = "X.Util.Core.Kernel.CoreQueue.Prefix";
        public string QueueId { get; set; }

        public CoreQueue(string queueId)
        {
            if (string.IsNullOrEmpty(queueId)) return;
            QueueId = queueId;
            if (Queue.ContainsKey(queueId)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof (T).FullName))
            {
                if (Queue.ContainsKey(queueId)) return;
                Queue[queueId] = new QueueContext<T>()
                {
                    QueueId = queueId,
                    EventWait = new ManualResetEvent(false),
                    Queue = new ConcurrentQueue<QueueItem<T>>(),
                    Timer = new ConcurrentDictionary<string, string>()
                };
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        QueueItem<T> item;
                        if (Queue[queueId].Queue.TryDequeue(out item) && item != null)
                        {
                            if (string.IsNullOrEmpty(item.Id) && item.Timer >= 0) continue;
                            if (item.Timer == 0)
                            {
                                item.Method.BeginInvoke(item.Context, null, null);
                            }
                            else if (item.Timer > 0)
                            {
                                if (Queue[queueId].Timer.ContainsKey(item.Id)) continue;
                                Queue[queueId].Timer[item.Id] = item.Id;
                                Action<T> target = T =>
                                {
                                    while (true)
                                    {
                                        item.Method.Invoke(T);
                                        Thread.Sleep(1000*item.Timer);
                                    }
                                };
                                target.BeginInvoke(item.Context, null, null);
                            }
                        }
                        else
                        {
                            Queue[queueId].EventWait.Reset();
                            Queue[queueId].EventWait.WaitOne();
                        }
                    }
                }) {IsBackground = true};
                thread.Start();
            }
        }

        public void Enqueue(QueueItem<T> item)
        {
            if (Equals(item, null)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof(T).FullName))
            {
                if (Queue[QueueId].Timer.ContainsKey(item.Id)) return;
                Queue[QueueId].Queue.Enqueue(item);
                Queue[QueueId].EventWait.Set();
            }
        }
    }
}
