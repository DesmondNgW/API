using System;

namespace X.Util.Core.Kernel
{
    public class ExecutionContext<T>
    {
        public static T Current => (T)(CallContext.GetData(CallContext.Prefix + typeof(T).FullName) ?? Activator.CreateInstance<T>());
    }
}
