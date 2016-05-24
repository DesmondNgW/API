using System;
using System.Collections.Generic;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core.Context
{
    public class LoggerContext<TResult, TChannel> : IContext<TResult, TChannel>
    {
        public bool NeedElapsed = true;
        public bool NeedLogInfo = true;
        private readonly IProvider<TChannel> _channel;
        public readonly Func<TResult, bool> CallSuccess;

        public LoggerContext(IProvider<TChannel> channel, Func<TResult, bool> callSuccess)
        {
            _channel = channel;
            CallSuccess = callSuccess;
        }

        public LoggerContext(IProvider<TChannel> channel, Func<TResult, bool> callSuccess, bool needElapsed, bool needLogInfo)
        {
            _channel = channel;
            CallSuccess = callSuccess;
            NeedElapsed = needElapsed;
            NeedLogInfo = needLogInfo;
        }

        public IProvider<TChannel> Channel
        {
            get { return _channel; } 
        }

        public void Calling(ActionContext<TResult> context)
        {
            if (context.ContextArguments == null) context.ContextArguments = new Dictionary<string, object>();
            var crm = context.Request.Method;
            var method = Logger.Client.GetMethodInfo(crm, context.Request.ActionArguments, Channel.EndpointAddress);
            context.ContextArguments["CoreMethodInfo"] = method;
            if (NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", method.ClassName.FullName, method.MethodName));
            if (!NeedElapsed) return;
            var stopElapsed = Logger.Client.GetStopElapsed(crm, Channel.Domain, Channel.EndpointAddress);
            context.ContextArguments["StopElapsed"] = stopElapsed;
        }

        public void Called(ActionContext<TResult> context)
        {
            var stopElapsed = NeedElapsed && context.ContextArguments != null ? context.ContextArguments["StopElapsed"] as Action : null;
            var method = context.ContextArguments != null ? context.ContextArguments["CoreMethodInfo"] as CoreMethodInfo : null;
            var iresult = context.Response.Result;
            if (stopElapsed != null) stopElapsed();
            if (method == null) return;
            if (CallSuccess != null)
            {
                if (CallSuccess(iresult))
                {
                    if (NeedLogInfo) Logger.Client.Info(method, Channel.Domain, iresult);
                }
                else Logger.Client.Error(method, Channel.Domain, iresult);
            }
            if (NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} EndInvoke.", method.ClassName.FullName, method.MethodName));
        }

        public void OnException(ActionContext<TResult> context)
        {
            var method = Logger.Client.GetMethodInfo(context.Request.Method, context.Request.ActionArguments, Channel.EndpointAddress);
            Logger.Client.Error(method, Channel.Domain, null, context.Response.Exception.ToString());
        }
    }

    public class LoggerContext<TChannel> : IContext<TChannel>
    {
        public bool NeedElapsed = true;
        public bool NeedLogInfo = true;
        private readonly IProvider<TChannel> _channel;

        public LoggerContext(IProvider<TChannel> channel)
        {
            _channel = channel;
        }

        public LoggerContext(IProvider<TChannel> channel, bool needElapsed, bool needLogInfo)
        {
            _channel = channel;
            NeedElapsed = needElapsed;
            NeedLogInfo = needLogInfo;
        }

        public IProvider<TChannel> Channel
        {
            get { return _channel; }
        }

        public void Calling(ActionContext context)
        {
            if (context.ContextArguments == null) context.ContextArguments = new Dictionary<string, object>();
            var crm = context.Request.Method;
            var method = Logger.Client.GetMethodInfo(crm, context.Request.ActionArguments, Channel.EndpointAddress);
            context.ContextArguments["CoreMethodInfo"] = method;
            if (NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", method.ClassName.FullName, method.MethodName));
            if (!NeedElapsed) return;
            var stopElapsed = Logger.Client.GetStopElapsed(crm, Channel.Domain, Channel.EndpointAddress);
            context.ContextArguments["StopElapsed"] = stopElapsed;
        }

        public void Called(ActionContext context)
        {
            var stopElapsed = NeedElapsed && context.ContextArguments != null ? context.ContextArguments["StopElapsed"] as Action : null;
            var method = context.ContextArguments != null ? context.ContextArguments["CoreMethodInfo"] as CoreMethodInfo : null;
            if (stopElapsed != null) stopElapsed();
            if (method == null) return;
            if (NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} EndInvoke.", method.ClassName.FullName, method.MethodName));
        }

        public void OnException(ActionContext context)
        {
            var method = Logger.Client.GetMethodInfo(context.Request.Method, context.Request.ActionArguments, Channel.EndpointAddress);
            Logger.Client.Error(method, Channel.Domain, null, context.Response.Exception.ToString());
        }
    }
}
