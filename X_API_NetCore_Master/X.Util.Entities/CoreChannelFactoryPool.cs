using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace X.Util.Entities
{
    public class CoreChannelFactoryPool<TChannel>
    {
        public int Size { get; set; }
        public string ServiceUri { get; set; }
        public ManualResetEvent EventWait { get; set; }
        public ConcurrentQueue<ContextChannel<TChannel>> ContextQueue { get; set; }
    }
}
