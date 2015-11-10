using System.Collections.Concurrent;

namespace X.Util.Entities
{
    public class CoreChannelFactoryPool<TChannel>
    {
        public int Size { get; set; }
        public string ServiceUri { get; set; }
        public ConcurrentQueue<ContextChannel<TChannel>> ContextQueue { get; set; }
    }
}
