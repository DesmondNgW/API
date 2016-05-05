using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core
{
    public sealed class Logger : ILogger
    {
        private static readonly Action<string, CoreMethodInfo, LogDomain, LogType, object, string[]> NAction = Log;
        private static readonly Action<string, MethodBase, LogDomain, LogType, object, string, string[]> MAction = Log;

        private Logger() { }
        public static ILogger Client = new Logger();

        /// <summary>
        /// GetMethodInfo
        /// </summary>
        public CoreMethodInfo GetMethodInfo(MethodBase declaringType, object[] values, string address)
        {
            return new CoreMethodInfo { ClassName = declaringType.DeclaringType, MethodName = declaringType.Name, DeclaringType = declaringType, ParamList = GetParamList(declaringType, values), Address = address };
        }
        /// <summary>
        /// GetMethodParamList
        /// </summary>
        private Dictionary<string, object> GetParamList(MethodBase declaringType, IList<object> values)
        {
            var arguments = declaringType.GetParameters().OrderBy(p => p.Position).ToList();
            if (arguments.Count.Equals(0) || arguments.Count != values.Count) return null;
            var result = new Dictionary<string, object>();
            for (var i = 0; i < arguments.Count; i++) result[arguments[i].Name] = values[i];
            return result;
        }

        #region WriteFiles
        private static void Log(string ip, CoreMethodInfo methodInfo, LogDomain domain, LogType logtype, object returnValue, params string[] messages)
        {
            var log = LoggerConfig.Instance.GetLogger(domain);
            try
            {
                var message = new StringBuilder();
                message.Append("[" + ip + "][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]");
                message.Append("\t\n");
                if (methodInfo.ParamList != null && methodInfo.ParamList.Count > 0)
                {
                    message.Append("param：");
                    message.Append(methodInfo.ParamList.ToJson());
                    message.Append("\t\n");
                }
                if (!string.IsNullOrEmpty(methodInfo.Address))
                {
                    message.Append("address：");
                    message.Append(methodInfo.Address);
                    message.Append("\t\n");
                }
                if (returnValue != null)
                {
                    message.Append("Result：");
                    message.Append(returnValue.ToJson());
                    message.Append("\t\n");
                }
                if (messages.Length > 0) message.Append(string.Join("\t\n", messages) + "\t\n");
                switch (logtype)
                {
                    case LogType.Error:
                        log.Error(message);
                        break;
                    case LogType.Debug:
                        log.Debug(message);
                        break;
                    case LogType.Info:
                        log.Info(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Client.Error(MethodBase.GetCurrentMethod(), domain, logtype, null, string.Empty, ex.ToString());
            }
        }

        private static void Log(string ip, MethodBase declaringType, LogDomain domain, LogType logtype, object returnValue, string address, params string[] messages)
        {
            var log = LoggerConfig.Instance.GetLogger(domain);
            var message = new StringBuilder();
            message.Append("[" + ip + "][" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]");
            message.Append("\t\n");
            if (!string.IsNullOrEmpty(address))
            {
                message.Append("address：");
                message.Append(address);
                message.Append("\t\n");
            }
            if (returnValue != null)
            {
                message.Append("Result：");
                message.Append(returnValue.ToJson());
                message.Append("\t\n");
            }
            if (messages.Length > 0) message.Append(string.Join("\t\n", messages) + "\t\n");
            switch (logtype)
            {
                case LogType.Error:
                    log.Error(message);
                    break;
                case LogType.Debug:
                    log.Debug(message);
                    break;
                case LogType.Info:
                    log.Info(message);
                    break;
            }
        }

        public void Info(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages)
        {
            NAction.BeginInvoke(CoreUtil.GetIp(), methodInfo, domain, LogType.Info, returnValue, messages, null, null);
        }

        public void Debug(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages)
        {
            NAction.BeginInvoke(CoreUtil.GetIp(), methodInfo, domain, LogType.Debug, returnValue, messages, null, null);
        }

        public void Error(CoreMethodInfo methodInfo, LogDomain domain, object returnValue, params string[] messages)
        {
            NAction.BeginInvoke(CoreUtil.GetIp(), methodInfo, domain, LogType.Error, returnValue, messages, null, null);
        }

        public void Debug(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages)
        {
            MAction.BeginInvoke(CoreUtil.GetIp(), declaringType, domain, LogType.Debug, returnValue, address, messages, null, null);
        }

        public void Error(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages)
        {
            MAction.BeginInvoke(CoreUtil.GetIp(), declaringType, domain, LogType.Error, returnValue, address, messages, null, null);
        }

        public void Info(MethodBase declaringType, LogDomain domain, object returnValue, string address, params string[] messages)
        {
            MAction.BeginInvoke(CoreUtil.GetIp(), declaringType, domain, LogType.Info, returnValue, address, messages, null, null);
        }
        #endregion
    }
}
