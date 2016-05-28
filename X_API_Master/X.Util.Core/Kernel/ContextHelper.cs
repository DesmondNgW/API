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
            attr.AddRange(method.GetCustomAttributes(typeof(IContext<TResult, TChannel>), true).Cast<IContext<TResult, TChannel>>());
            if (method.DeclaringType != null) attr.AddRange(method.DeclaringType.GetCustomAttributes(typeof(IContext<TResult, TChannel>), true).Cast<IContext<TResult, TChannel>>());
            return new List<IContext<TResult, TChannel>>
            {
                new ChannelContext<TResult, TChannel>(channel),
                new LoggerContext<TResult, TChannel>(channel, options)
            }.Concat(attr.OrderByDescending(p=>p.Priority)).ToList();
        }

        public static List<IContext<TChannel>> GetContext<TChannel>(IProvider<TChannel> channel, MethodBase method, LogOptions options)
        {
            var attr = new List<IContext<TChannel>>();
            attr.AddRange(method.GetCustomAttributes(typeof(IContext<TChannel>), true).Cast<IContext<TChannel>>());
            if (method.DeclaringType != null) attr.AddRange(method.DeclaringType.GetCustomAttributes(typeof(IContext<TChannel>), true).Cast<IContext<TChannel>>());
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
