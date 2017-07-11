using System;
using System.Collections.Concurrent;
using System.Threading;

namespace X.Util.Entities
{
    public class QueueItem<T>
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 队列处理方法
        /// </summary>
        public Action<T> Method { get; set; }

        /// <summary>
        /// 队列处理的参数
        /// </summary>
        public T Context { get; set; }

        /// <summary>
        /// 队列间隔周期（单位：秒）,0表示执行一次
        /// </summary>
        public int Timer { get; set; }
    }


    public class QueueContext<T>
    {
        /// <summary>
        /// 队列ID
        /// </summary>
        public string QueueId { get; set; }

        /// <summary>
        /// 队列索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 普通队列
        /// </summary>
        public ConcurrentQueue<QueueItem<T>> Queue { get; set; }

        /// <summary>
        /// 定时队列
        /// </summary>
        public ConcurrentQueue<QueueItem<T>> Timer { get; set; }

        /// <summary>
        /// 同步事件
        /// </summary>
        public ManualResetEvent EventWait { get; set; }
    }
}
