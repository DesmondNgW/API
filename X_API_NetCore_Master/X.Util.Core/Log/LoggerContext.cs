using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        public Func<TResult> Calling(ActionContext<TResult> context, Func<TResult> caller)
        {
            if (context.ContextArguments == null) context.ContextArguments = new Dictionary<string, object>();
            var crm = context.Request.Method;
            var method = Logger.Client.GetMethodInfo(crm, context.Request.ActionArguments, Channel.EndpointAddress);
            context.ContextArguments["CoreMethodInfo"] = method;
            if (Options.NeedLogInfo) Logger.Client.Request(method, Channel.Domain);
            var sw = new Stopwatch();
            sw.Start();
            Func<long> stopElapsed = () =>
            {
                sw.Stop();
                return sw.ElapsedMilliseconds;
            };
            context.ContextArguments["StopElapsed"] = stopElapsed;
            return caller;
        }

        public void Called(ActionContext<TResult> context)
        {
            var stopElapsed = Options.NeedLogInfo && context.ContextArguments != null ? context.ContextArguments["StopElapsed"] as Func<long> : null;
            var method = context.ContextArguments != null ? context.ContextArguments["CoreMethodInfo"] as RequestMethodInfo : null;
            var iresult = context.Response.Result;
            if (method == null) return;
            var response = new ResponseResult
            {
                Elapsed = stopElapsed != null ? stopElapsed() : 0,
                Return = iresult
            };
            if (Options.CallSuccess == null) return;
            if (Options.CallSuccess(iresult))
            {
                if (Options.NeedLogInfo) Logger.Client.Response(method, response, Channel.Domain, LogType.Info);
            }
            else Logger.Client.Response(method, response, Channel.Domain, LogType.Error);
        }

        public void OnException(ActionContext<TResult> context)
        {
            var method = Logger.Client.GetMethodInfo(context.Request.Method, context.Request.ActionArguments, Channel.EndpointAddress);
            Logger.Client.Error(method, context.Response.Exception, Channel.Domain);
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

        public Action Calling(ActionContext context, Action caller)
        {
            if (context.ContextArguments == null) context.ContextArguments = new Dictionary<string, object>();
            var crm = context.Request.Method;
            var method = Logger.Client.GetMethodInfo(crm, context.Request.ActionArguments, Channel.EndpointAddress);
            context.ContextArguments["CoreMethodInfo"] = method;
            if (Options.NeedLogInfo) Logger.Client.Request(method, Channel.Domain);
            var sw = new Stopwatch();
            sw.Start();
            Func<long> stopElapsed = () =>
            {
                sw.Stop();
                return sw.ElapsedMilliseconds;
            };
            context.ContextArguments["StopElapsed"] = stopElapsed;
            return caller;
        }

        public void Called(ActionContext context)
        {
            var stopElapsed = Options.NeedLogInfo && context.ContextArguments != null ? context.ContextArguments["StopElapsed"] as Func<long> : null;
            var method = context.ContextArguments != null ? context.ContextArguments["CoreMethodInfo"] as RequestMethodInfo : null;
            if (method == null) return;
            if (Options.NeedLogInfo)
                Logger.Client.Response(method, new ResponseResult
                {
                    Elapsed = stopElapsed != null ? stopElapsed() : 0,
                }, Channel.Domain, LogType.Info);
        }

        public void OnException(ActionContext context)
        {
            var method = Logger.Client.GetMethodInfo(context.Request.Method, context.Request.ActionArguments, Channel.EndpointAddress);
            Logger.Client.Error(method, context.Response.Exception, Channel.Domain);
        }

        public int Priority { get { return int.MaxValue; } }
    }
}
