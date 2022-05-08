using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using X.Util.Entities;
using X.Util.Entities.Enum;

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
            lock (CoreUtil.Getlocker(LockerPrefix + typeof(T).FullName))
            {
                if (Queue.ContainsKey(queueId)) return;
                Queue[queueId] = new QueueContext<T>
                {
                    QueueId = queueId,
                    EventWait = new ManualResetEvent(false),
                    RequestQueue = new ConcurrentQueue<RequestItem<T>>(),
                    Timer = new ConcurrentDictionary<string, Timer>()
                };

                //独立线程处理
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        if (Queue[queueId].RequestQueue.TryDequeue(out RequestItem<T> item) && item != null)
                        {
                            if (item.Mode == ProcessingMode.Common)
                            {
                                item.Method.BeginInvoke(item.Context, null, null);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.RequestId) && !Queue[QueueId].Timer.ContainsKey(item.RequestId))
                                {
                                    Queue[QueueId].Timer[item.RequestId] = new Timer(Convert(item.Method), item.Context, 0, item.Period);
                                }
                            }
                        }
                        else
                        {
                            Queue[queueId].EventWait.Reset();
                            Queue[queueId].EventWait.WaitOne();
                        }
                    }
                })
                { IsBackground = true };
                thread.Start();
            }
        }

        public TimerCallback Convert(Action<T> action)
        {
            return o => { action((T)o); };
        }

        public void Enqueue(RequestItem<T> item)
        {
            if (Equals(item, null) || string.IsNullOrEmpty(item.RequestId)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof(T).FullName))
            {
                if (!string.IsNullOrEmpty(item.RequestId))
                {
                    if ((item.Mode != ProcessingMode.Timer || Queue[QueueId].Timer.ContainsKey(item.RequestId)) && item.Mode != ProcessingMode.Common) return;
                    Queue[QueueId].RequestQueue.Enqueue(item);
                    Queue[QueueId].EventWait.Set();
                }
                else
                {
                    Queue[QueueId].EventWait.Set();
                }
            }
        }
    }
}
