using System;

namespace X.Cache.Service
{
    public class SendModel
    {
        public string Key { get; set; }

        public object Value { get; set; }

        public DateTime ExpireAt { get; set; }

        public TimeSpan Expire { get; set; }
    }
}
