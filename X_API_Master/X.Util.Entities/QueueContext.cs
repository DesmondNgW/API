using System;
using System.Collections.Concurrent;
using System.Threading;

namespace X.Util.Entities
{
    public class QueueItem<T>
    {
        /// <summary>
        /// 队列处理方法
        /// </summary>
        public Action<T> Method { get; set; }

        /// <summary>
        /// 队列处理的参数
        /// </summary>
        public T Context { get; set; }
    }


    public class QueueContext<T>
    {
        /// <summary>
        /// 队列ID
        /// </summary>
        public string QueueId { get; set; }

        /// <summary>
        /// 队列
        /// </summary>
        public ConcurrentQueue<QueueItem<T>> Queue { get; set; }

        /// <summary>
        /// 同步事件
        /// </summary>
        public ManualResetEvent EventWait { get; set; }
    }
}
