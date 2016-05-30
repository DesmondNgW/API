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
                    Queue = new ConcurrentQueue<QueueItem<T>>()
                };
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
            }
        }

        public void Enqueue(QueueItem<T> item)
        {
            if (Equals(item, null)) return;
            lock (CoreUtil.Getlocker(LockerPrefix + typeof(T).FullName))
            {
                Queue[QueueId].Queue.Enqueue(item);
                Queue[QueueId].EventWait.Set();
            }
        }
    }
}
