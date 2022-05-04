using System;
using System.Collections.Generic;
using System.Text;

namespace X.Util.Entities
{
    public class ContextChannel<T>
    {
        public DateTime ChannelClosedTime { get; set; }

        public T Channel { get; set; }
    }
}
