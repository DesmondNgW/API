using System;
using System.Reflection;

namespace X.Util.Entities.Interface
{
    public interface ILogger
    {
        /// <summary>
        /// MethodInfo
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="values"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        CoreMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address);

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="domain"></param>
        /// <param name="returnValue"></param>
        /// <param name="messages"></param>
        void Info(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages);

        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="domain"></param>
        /// <param name="returnValue"></param>
        /// <param name="messages"></param>
        void Debug(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages);

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="domain"></param>
        /// <param name="returnValue"></param>
        /// <param name="messages"></param>
        void Error(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages);

        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="domain"></param>
        /// <param name="returnValue"></param>
        /// <param name="address"></param>
        /// <param name="messages"></param>
        void Debug(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages);

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="domain"></param>
        /// <param name="returnValue"></param>
        /// <param name="address"></param>
        /// <param name="messages"></param>
        void Error(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages);

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="domain"></param>
        /// <param name="returnValue"></param>
        /// <param name="address"></param>
        /// <param name="messages"></param>
        void Info(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages);

        /// <summary>
        /// 记录方法调用耗时
        /// </summary>
        /// <param name="method"></param>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="edomain"></param>
        /// <param name="address"></param>
        void Elapsed(MethodBase method, long elapsedMilliseconds, LogDomain edomain, string address = null);

        /// <summary>
        /// 获取记录耗时的action
        /// </summary>
        /// <param name="method"></param>
        /// <param name="edomain"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Action GetStopElapsed(MethodBase method, LogDomain edomain, string address = null);
    }
}
