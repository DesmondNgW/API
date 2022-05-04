using System;
using System.Collections.Generic;
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
        /// <returns></returns>
        RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values);

        /// <summary>
        /// MethodInfo
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="values"></param>
        /// <param name="extendInfo"></param>
        /// <returns></returns>
        RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, Dictionary<string, object> extendInfo);

        /// <summary>
        /// MethodInfo
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="values"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address);

        /// <summary>
        /// MethodInfo
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="values"></param>
        /// <param name="address"></param>
        /// <param name="extendInfo"></param>
        /// <returns></returns>
        RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address, Dictionary<string, object> extendInfo);

        /// <summary>
        /// LogRequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="domain"></param>
        void Request(RequestMethodInfo request, LogDomain domain);

        /// <summary>
        /// LogResponse
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="domain"></param>
        /// <param name="logtype"></param>
        void Response(RequestMethodInfo request, ResponseResult response, LogDomain domain, LogType logtype);

        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="domain"></param>
        void Error(RequestMethodInfo request, Exception exception, LogDomain domain);

        /// <summary>
        /// LogFatal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="domain"></param>
        void Fatal(RequestMethodInfo request, Exception exception, LogDomain domain);

        /// <summary>
        /// LogWarn
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="domain"></param>
        void Warn(RequestMethodInfo request, Exception exception, LogDomain domain);

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="domain"></param>
        void Debug(string message, LogDomain domain);

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="domain"></param>
        void Warn(string message, LogDomain domain);
    }
}
