using System.Collections.Generic;
using System.Reflection;

namespace X.Util.Entities.Interface
{
    public interface ILogger
    {
        CoreMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address);

        void Info(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages);

        void Debug(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages);

        void Error(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages);

        void Debug(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages);

        void Error(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages);

        void Info(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages);
    }
}
