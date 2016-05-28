using System;
using System.Collections.Generic;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core.Log
{
    public class LoggerContext<TResult, TChannel> : IContext<TResult, TChannel>
    {
        private readonly IProvider<TChannel> _channel;
        public readonly LogOptions<TResult> Options;

        public LoggerContext(IProvider<TChannel> channel, LogOptions<TResult> options)
        {
            _channel = channel;
            Options = options ?? new LogOptions<TResult>(null);
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
            if (Options.NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", method.ClassName.FullName, method.MethodName));
            if (!Options.NeedElapsed) return;
            var stopElapsed = Logger.Client.GetStopElapsed(crm, Channel.Domain, Channel.EndpointAddress);
            context.ContextArguments["StopElapsed"] = stopElapsed;
        }

        public void Called(ActionContext<TResult> context)
        {
            var stopElapsed = Options.NeedElapsed && context.ContextArguments != null ? context.ContextArguments["StopElapsed"] as Action : null;
            var method = context.ContextArguments != null ? context.ContextArguments["CoreMethodInfo"] as CoreMethodInfo : null;
            var iresult = context.Response.Result;
            if (stopElapsed != null) stopElapsed();
            if (method == null) return;
            if (Options.CallSuccess != null)
            {
                if (Options.CallSuccess(iresult))
                {
                    if (Options.NeedLogInfo) Logger.Client.Info(method, Channel.Domain, iresult);
                }
                else Logger.Client.Error(method, Channel.Domain, iresult);
            }
            if (Options.NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} EndInvoke.", method.ClassName.FullName, method.MethodName));
        }

        public void OnException(ActionContext<TResult> context)
        {
            var method = Logger.Client.GetMethodInfo(context.Request.Method, context.Request.ActionArguments, Channel.EndpointAddress);
            Logger.Client.Error(method, Channel.Domain, null, context.Response.Exception.ToString());
        }

        public int Priority { get { return int.MaxValue; } }
    }

    public class LoggerContext<TChannel> : IContext<TChannel>
    {
        private readonly IProvider<TChannel> _channel;
        public readonly LogOptions Options;

        public LoggerContext(IProvider<TChannel> channel)
        {
            _channel = channel;
        }

        public LoggerContext(IProvider<TChannel> channel, LogOptions options)
        {
            _channel = channel;
            Options = options ?? new LogOptions();
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
            if (Options.NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} BeginInvoke.", method.ClassName.FullName, method.MethodName));
            if (!Options.NeedElapsed) return;
            var stopElapsed = Logger.Client.GetStopElapsed(crm, Channel.Domain, Channel.EndpointAddress);
            context.ContextArguments["StopElapsed"] = stopElapsed;
        }

        public void Called(ActionContext context)
        {
            var stopElapsed = Options.NeedElapsed && context.ContextArguments != null ? context.ContextArguments["StopElapsed"] as Action : null;
            var method = context.ContextArguments != null ? context.ContextArguments["CoreMethodInfo"] as CoreMethodInfo : null;
            if (stopElapsed != null) stopElapsed();
            if (method == null) return;
            if (Options.NeedLogInfo) Logger.Client.Info(method, Channel.Domain, null, string.Format("{0}.{1} EndInvoke.", method.ClassName.FullName, method.MethodName));
        }

        public void OnException(ActionContext context)
        {
            var method = Logger.Client.GetMethodInfo(context.Request.Method, context.Request.ActionArguments, Channel.EndpointAddress);
            Logger.Client.Error(method, Channel.Domain, null, context.Response.Exception.ToString());
        }

        public int Priority { get { return int.MaxValue; } }
    }
}
