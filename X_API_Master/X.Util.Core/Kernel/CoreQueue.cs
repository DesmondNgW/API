using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using X.Util.Entities;
using System.Linq;

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
        private Timer _timer = default(Timer);
        public string QueueId { get; set; }

        public CoreQueue(string queueId)
        {
            if (string.IsNullOrEmpty(queueId)) return;
            QueueId = queueId;
            if (Queue.ContainsKey(queueId)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof (T).FullName))
            {
                if (Queue.ContainsKey(queueId)) return;
                Queue[queueId] = new QueueContext<T>
                {
                    QueueId = queueId,
                    EventWait = new ManualResetEvent(false),
                    Queue = new ConcurrentQueue<QueueItem<T>>(),
                    Timer = new ConcurrentQueue<QueueItem<T>>(),
                    Index = 0
                };

                //独立线程处理普通队列
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        QueueItem<T> item;
                        if (Queue[queueId].Queue.TryDequeue(out item) && item != null)
                        {
                            item.Method.BeginInvoke(item.Context, null, null);
                        }
                        else
                        {
                            Queue[queueId].EventWait.Reset();
                            Queue[queueId].EventWait.WaitOne();
                        }
                    }
                }) {IsBackground = true};
                thread.Start();

                //独立线程处理定时队列
                var thread2 = new Thread(() =>
                {
                    while (true)
                    {
                        if (Queue[queueId].Timer != null && Queue[queueId].Index < Queue[queueId].Timer.Count)
                        {
                            foreach (var item in Queue[queueId].Timer.Skip(Queue[queueId].Index))
                            {
                                _timer = new Timer(Convert(item.Method), item.Context, 0, item.Timer);
                                Queue[queueId].Index++;
                            }
                        }
                        else
                        {
                            Queue[queueId].EventWait.Reset();
                            Queue[queueId].EventWait.WaitOne();
                        }
                    }
                }) { IsBackground = true };
                thread2.Start();
            }
        }

        public TimerCallback Convert(Action<T> action)
        {
            return o => { action((T)o); };
        }

        public void Enqueue(QueueItem<T> item)
        {
            if (Equals(item, null)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof(T).FullName))
            {
                if (!string.IsNullOrEmpty(item.Id) && item.Timer > 0 && Queue[QueueId].Timer.Count(p => p.Id == item.Id) == 0)
                {
                    Queue[QueueId].Timer.Enqueue(item);
                    Queue[QueueId].EventWait.Set();
                }
                else
                {
                    Queue[QueueId].Queue.Enqueue(item);
                    Queue[QueueId].EventWait.Set();
                }
            }
        }
    }
}
