using System.Collections.Generic;

namespace X.Util.Entities
{
    public class ActionContext<TResult>
    {
        public ActionRequest Request { get; set; }

        public ActionResponse<TResult> Response { get; set; }

        public Dictionary<string, object> ContextArguments { get; set; }
    }

    public class ActionContext
    {
        public ActionRequest Request { get; set; }

        public ActionResponse Response { get; set; }

        public Dictionary<string, object> ContextArguments { get; set; }
    }
}
