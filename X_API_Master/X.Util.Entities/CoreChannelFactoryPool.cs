using System.Collections.Generic;
using System.Threading;

namespace X.Util.Entities
{
    public class CoreChannelFactoryPool<TChannel>
    {
        public int Size { get; set; }
        public string ServiceUri { get; set; }
        public ManualResetEvent EventWait { get; set; }
        public Queue<ContextChannel<TChannel>> ContextQueue { get; set; }
    }
}
