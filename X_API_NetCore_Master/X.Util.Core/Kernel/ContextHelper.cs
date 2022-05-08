using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core.Kernel
{
    public class ContextHelper
    {
        public static List<IContext<TResult, TChannel>> GetContext<TResult, TChannel>(IProvider<TChannel> channel, MethodBase method, LogOptions<TResult> options)
        {
            var attr = new List<IContext<TResult, TChannel>>();
            if (method.GetCustomAttributes(typeof(ContextResultAttribute), true) is ContextResultAttribute[] cas && cas.Any()) attr.AddRange(cas.Select(ca => ca.GetContext(channel, options.CallSuccess)));
            if (method.DeclaringType == null)
                return new List<IContext<TResult, TChannel>>
                {
                    new ChannelContext<TResult, TChannel>(channel),
                    new LoggerContext<TResult, TChannel>(channel, options)
                }.Concat(attr.OrderByDescending(p => p.Priority)).ToList();
            cas = method.DeclaringType.GetCustomAttributes(typeof(ContextResultAttribute), true) as ContextResultAttribute[];
            if (cas != null && cas.Any()) attr.AddRange(cas.Select(ca => ca.GetContext(channel, options.CallSuccess)));
            return new List<IContext<TResult, TChannel>>
            {
                new ChannelContext<TResult, TChannel>(channel),
                new LoggerContext<TResult, TChannel>(channel, options)
            }.Concat(attr.OrderByDescending(p => p.Priority)).ToList();
        }

        public static Func<TResult> GetCaller<TResult, TChannel>(List<IContext<TResult, TChannel>> list, ActionContext<TResult> context, Func<TResult> caller)
        {
            return list.Aggregate(caller, (current, item) => item.Calling(context, current));
        }

        public static Action GetCaller<TChannel>(List<IContext<TChannel>> list, ActionContext context, Action caller)
        {
            return list.Aggregate(caller, (current, item) => item.Calling(context, current));
        }

        public static List<IContext<TChannel>> GetContext<TChannel>(IProvider<TChannel> channel, MethodBase method, LogOptions options)
        {
            var attr = new List<IContext<TChannel>>();
            if (method.GetCustomAttributes(typeof(ContextAttribute), true) is ContextAttribute[] cas && cas.Any()) attr.AddRange(cas.Select(ca => ca.GetContext(channel)));
            if (method.DeclaringType == null)
                return new List<IContext<TChannel>>
                {
                    new ChannelContext<TChannel>(channel),
                    new LoggerContext<TChannel>(channel, options)
                }.Concat(attr.OrderByDescending(p => p.Priority)).ToList();
            cas = method.DeclaringType.GetCustomAttributes(typeof(ContextAttribute), true) as ContextAttribute[];
            if (cas != null && cas.Any()) attr.AddRange(cas.Select(ca => ca.GetContext(channel)));
            return new List<IContext<TChannel>>
            {
                new ChannelContext<TChannel>(channel),
                new LoggerContext<TChannel>(channel, options)
            }.Concat(attr.OrderByDescending(p => p.Priority)).ToList();
        }

        public static ActionContext<TResult> GetActionContext<TResult>(MethodBase method, object[] values, Dictionary<string, object> header)
        {
            return new ActionContext<TResult>
            {
                Request = new ActionRequest
                {
                    Method = method,
                    ActionArguments = values,
                    DeclaringType = method.DeclaringType,
                    ActionHeader = header ?? new Dictionary<string, object>()
                },
                Response = new ActionResponse<TResult>()
            };
        }

        public static ActionContext GetActionContext(MethodBase method, object[] values, Dictionary<string, object> header)
        {
            return new ActionContext
            {
                Request = new ActionRequest
                {
                    Method = method,
                    ActionArguments = values,
                    DeclaringType = method.DeclaringType,
                    ActionHeader = header ?? new Dictionary<string, object>()
                },
                Response = new ActionResponse()
            };
        }

        public static void AddResponse<TResult>(ActionContext<TResult> context, TResult result, Exception e, Dictionary<string, object> header)
        {
            if (context.Response == null) context.Response = new ActionResponse<TResult>();
            context.Response.Result = result;
            context.Response.Exception = e;
            context.Response.ActionHeader = header;
        }

        public static void AddResponse(ActionContext context, Exception e, Dictionary<string, object> header)
        {
            if (context.Response == null) context.Response = new ActionResponse();
            context.Response.Exception = e;
            context.Response.ActionHeader = header;
        }
    }
}
