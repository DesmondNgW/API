using System;
using System.Collections.Concurrent;
using System.Threading;
using X.Util.Entities.Enums;

namespace X.Util.Entities
{
    public class RequestItem<T>
    {
        /// <summary>
        /// 请求Id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 处理方法
        /// </summary>
        public Action<T> Method { get; set; }

        /// <summary>
        /// 处理参数
        /// </summary>
        public T Context { get; set; }

        /// <summary>
        /// 处理模式
        /// </summary>
        public ProcessingMode Mode { get; set; }

        /// <summary>
        /// 定时处理的时间间隔
        /// </summary>
        public int Period { get; set; }
    }

    public class QueueContext<T>
    {
        /// <summary>
        /// 队列ID
        /// </summary>
        public string QueueId { get; set; }

        /// <summary>
        /// 请求队列
        /// </summary>
        public ConcurrentQueue<RequestItem<T>> RequestQueue { get; set; }

        /// <summary>
        /// 定时器集合
        /// </summary>
        public ConcurrentDictionary<string, Timer> Timer { get; set; }

        /// <summary>
        /// 同步事件
        /// </summary>
        public ManualResetEvent EventWait { get; set; }
    }
}
