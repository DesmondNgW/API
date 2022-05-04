using System;
using System.Collections.Generic;

namespace X.Util.Entities
{
    public class ActionResponse<TResult>
    {
        public TResult Result { get; set; }

        public Exception Exception { get; set; }

        public Dictionary<string, object> ActionHeader { get; set; }
    }

    public class ActionResponse
    {
        public Exception Exception { get; set; }

        public Dictionary<string, object> ActionHeader { get; set; }
    }
}
