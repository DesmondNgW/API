using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using StackExchange.Redis;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Entities.Interface;

namespace X.Util.Core.Log
{
    public sealed class Logger : ILogger
    {
        private Logger() { }
        public static ILogger Client = new Logger();
        
        /// <summary>
        /// GetMethodParamList
        /// </summary>
        private static Dictionary<string, object> GetParamList(MethodBase declaringType, IList<object> values)
        {
            var arguments = declaringType.GetParameters().OrderBy(p => p.Position).ToList();
            if (arguments.Count.Equals(0) || arguments.Count != values.Count) return null;
            var result = new Dictionary<string, object>();
            for (var i = 0; i < arguments.Count; i++)
            {
                //fix RedisKey ToJson is nullObj
                if (values[i] is RedisKey)
                {
                    result[arguments[i].Name] = ((RedisKey) values[i]).ToString();
                }
                else
                {
                    result[arguments[i].Name] = values[i];
                }
            }
            return result;
        }

        private static void Log(string message, LogDomain domain, LogType logType)
        {
            CoreUtil.CoderLocker("X.Util.Core.Log." + domain + logType, () =>
            {
                var log = LoggerConfig.Instance.GetLogger(domain);
                switch (logType)
                {
                    case LogType.Warn:
                        log.Warn(message);
                        return;
                    case LogType.Debug:
                        log.Debug(message);
                        return;
                }
            });
        }

        /// <summary>
        /// GetMethodInfo
        /// </summary>
        public RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values)
        {
            var id = ExecutionContext<RequestContext>.Current.ApiRequestContext != null ? ExecutionContext<RequestContext>.Current.ApiRequestContext.RequestId : Guid.NewGuid().ToString("N");
            var token = ExecutionContext<RequestContext>.Current.Token ?? id;
            return new RequestMethodInfo { Id = id, ClientId = token, ClassName = declaringType.DeclaringType, MethodName = declaringType.Name, Method = declaringType, ParamList = GetParamList(declaringType, values), ClientIp = IpBase.GetIp(), ServerIp = IpBase.GetLocalIp() };
        }

        /// <summary>
        /// GetMethodInfo
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="values"></param>
        /// <param name="extendInfo"></param>
        /// <returns></returns>
        public RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, Dictionary<string, object> extendInfo)
        {
            var id = ExecutionContext<RequestContext>.Current.ApiRequestContext != null
                ? ExecutionContext<RequestContext>.Current.ApiRequestContext.RequestId
                : Guid.NewGuid().ToString();
            var token = ExecutionContext<RequestContext>.Current.Token ?? id;
            return new RequestMethodInfo { Id = id, ClientId = token, ClassName = declaringType.DeclaringType, MethodName = declaringType.Name, Method = declaringType, ParamList = GetParamList(declaringType, values), ExtendInfo = extendInfo, ClientIp = IpBase.GetIp(), ServerIp = IpBase.GetLocalIp() };
        }

        /// <summary>
        /// GetMethodInfo
        /// </summary>
        public RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address)
        {
            var id = ExecutionContext<RequestContext>.Current.ApiRequestContext != null
                ? ExecutionContext<RequestContext>.Current.ApiRequestContext.RequestId
                : Guid.NewGuid().ToString();
            var token = ExecutionContext<RequestContext>.Current.Token ?? id;
            return new RequestMethodInfo { Id = id, ClientId = token, ClassName = declaringType.DeclaringType, MethodName = declaringType.Name, Method = declaringType, ParamList = GetParamList(declaringType, values), Address = address, ClientIp = IpBase.GetIp(), ServerIp = IpBase.GetLocalIp() };
        }

        /// <summary>
        /// GetMethodInfo
        /// </summary>
        public RequestMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address, Dictionary<string, object> extendInfo)
        {
            var id = ExecutionContext<RequestContext>.Current.ApiRequestContext != null
                ? ExecutionContext<RequestContext>.Current.ApiRequestContext.RequestId
                : Guid.NewGuid().ToString();
            var token = ExecutionContext<RequestContext>.Current.Token ?? id;
            return new RequestMethodInfo { Id = id, ClientId = token, ClassName = declaringType.DeclaringType, MethodName = declaringType.Name, Method = declaringType, ParamList = GetParamList(declaringType, values), Address = address, ExtendInfo = extendInfo, ClientIp = IpBase.GetIp(), ServerIp = IpBase.GetLocalIp() };
        }

        /// <summary>
        /// LogRequest
        /// </summary>
        /// <param name="request"></param>
        /// <param name="domain"></param>
        private static void LogRequest(RequestMethodInfo request, LogDomain domain)
        {
            CoreUtil.CoderLocker("X.Util.Core.Log." + domain + LogType.Info, () =>
            {
                var log = LoggerConfig.Instance.GetLogger(domain);
                var message = new StringBuilder();
                message.AppendLine(string.Format("{0} ClientId: {1} Request Id: {2}, From: {3}, To: {4}, Path:{5}.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), request.ClientId, request.Id, request.ClientIp, request.ServerIp, string.Format("{0}.{1}", request.ClassName.FullName, request.MethodName)));
                if (request.ParamList != null && request.ParamList.Count > 0)
                {
                    message.AppendLine(string.Format("param: {0}", request.ParamList.ToJson()));
                }
                if (!string.IsNullOrEmpty(request.Address))
                {
                    message.AppendLine(string.Format("address: {0}", request.Address));
                }
                if (request.ExtendInfo != null && request.ExtendInfo.Count > 0)
                {
                    message.AppendLine(string.Format("extend: {0}", request.ExtendInfo.ToJson()));
                }
                log.Info(message);
            });
        }

        /// <summary>
        /// LogResponse
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="domain"></param>
        /// <param name="logtype"></param>
        private static void LogResponse(RequestMethodInfo request, ResponseResult response, LogDomain domain, LogType logtype)
        {
            if (response.Exception != null && (logtype == LogType.Debug || logtype == LogType.Info)) logtype = LogType.Warn;
            CoreUtil.CoderLocker("X.Util.Core.Log." + domain + logtype, () =>
            {
                var log = LoggerConfig.Instance.GetLogger(domain);
                var message = new StringBuilder();
                message.AppendLine(string.Format("{0} ClientId: {1} Response Id: {2}, From: {3}, To: {4}, Path:{5}.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), request.ClientId, request.Id, request.ClientIp, request.ServerIp, string.Format("{0}.{1}", request.ClassName.FullName, request.MethodName)));
                if (request.ParamList != null && request.ParamList.Count > 0)
                {
                    message.AppendLine(string.Format("param: {0}", request.ParamList.ToJson()));
                }
                if (!string.IsNullOrEmpty(request.Address))
                {
                    message.AppendLine(string.Format("address: {0}", request.Address));
                }
                if (request.ExtendInfo != null && request.ExtendInfo.Count > 0)
                {
                    message.AppendLine(string.Format("request Extend: {0}", request.ExtendInfo.ToJson()));
                }
                if (response.Exception != null)
                {
                    message.AppendLine(string.Format("Exception: {0}", response.Exception));
                }
                else
                {
                    if (response.Return != null)
                    {
                        message.AppendLine(string.Format("Return: {0}", response.Return.ToJson()));
                    }
                    if (response.ExtendMessages != null)
                    {
                        message.AppendLine(string.Format("Response Extend: {0}", response.ExtendMessages.ToJson()));
                    }
                    message.AppendLine(string.Format("Response Elapsed: {0} ms.", response.Elapsed));
                }
                switch (logtype)
                {
                    case LogType.Fatal:
                        log.Fatal(message);
                        break;
                    case LogType.Error:
                        log.Error(message);
                        break;
                    case LogType.Warn:
                        log.Warn(message);
                        break;
                    case LogType.Info:
                        log.Info(message);
                        break;
                }
            });
        }

        /// <summary>
        /// 记录请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="domain"></param>
        public void Request(RequestMethodInfo request, LogDomain domain)
        {
            ((Action<RequestMethodInfo, LogDomain>) LogRequest).BeginInvoke(request, domain, null, null);
        }

        /// <summary>
        /// 记录响应
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="domain"></param>
        /// <param name="logtype"></param>
        public void Response(RequestMethodInfo request, ResponseResult response, LogDomain domain, LogType logtype)
        {
            ((Action<RequestMethodInfo, ResponseResult, LogDomain, LogType>) LogResponse).BeginInvoke(request, response, domain, logtype, null, null);
        }

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="domain"></param>
        public void Error(RequestMethodInfo request, Exception exception, LogDomain domain)
        {
            ((Action<RequestMethodInfo, ResponseResult, LogDomain, LogType>) LogResponse).BeginInvoke(request, new ResponseResult {Exception = exception}, domain, LogType.Error, null, null);
        }

        /// <summary>
        /// 记录Fatal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="domain"></param>
        public void Fatal(RequestMethodInfo request, Exception exception, LogDomain domain)
        {
            ((Action<RequestMethodInfo, ResponseResult, LogDomain, LogType>)LogResponse).BeginInvoke(request, new ResponseResult { Exception = exception }, domain, LogType.Fatal, null, null);
        }

        /// <summary>
        /// 记录Warn
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="domain"></param>
        public void Warn(RequestMethodInfo request, Exception exception, LogDomain domain)
        {
            ((Action<RequestMethodInfo, ResponseResult, LogDomain, LogType>)LogResponse).BeginInvoke(request, new ResponseResult { Exception = exception }, domain, LogType.Warn, null, null);
        }

        /// <summary>
        /// LogDebug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="domain"></param>
        public void Debug(string message, LogDomain domain)
        {
            Log(message, domain, LogType.Debug);
        }

        /// <summary>
        /// LogWarn
        /// </summary>
        /// <param name="message"></param>
        /// <param name="domain"></param>
        public void Warn(string message, LogDomain domain)
        {
            Log(message, domain, LogType.Warn);
        }
    }
}
