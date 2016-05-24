using System;
using System.Collections.Generic;
using System.Reflection;

namespace X.Util.Entities
{
    public class ActionRequest
    {
        public MethodBase Method { get; set; }

        public object[] ActionArguments { get; set; }

        public Type DeclaringType { get; set; }

        public Dictionary<string, object> ActionHeader { get; set; }
    }
}
