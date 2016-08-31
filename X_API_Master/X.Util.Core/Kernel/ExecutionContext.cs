using System;
using System.Runtime.Remoting.Messaging;

namespace X.Util.Core.Kernel
{
    public class ExecutionContext<T>
    {
        public static T Current
        {
            get { return (T)(CallContext.GetData("CallContext.Data." + typeof(T).FullName) ?? Activator.CreateInstance<T>()); }
        }

        public static T LogicalCurrent
        {
            get { return (T)(CallContext.LogicalGetData("CallContext.LogicalData." + typeof(T).FullName) ?? Activator.CreateInstance<T>()); }
        }
    }
}
