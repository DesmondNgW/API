using System;
using System.Collections.Generic;
using System.Reflection;

namespace X.Util.Entities
{
    public class CoreMethodInfo
    {
        public Type ClassName { get; set; }
        public string MethodName { get; set; }
        public string Address { get; set; }
        public Dictionary<string, object> ParamList { get; set; }
        public MethodBase DeclaringType { get; set; }
    }
}
