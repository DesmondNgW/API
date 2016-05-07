using System;

namespace X.Util.Entities
{
    public class ContextChannel<T>
    {
        public DateTime ChannelClosedTime { get; set; }

        public T Channel { get; set; }
    }
}
