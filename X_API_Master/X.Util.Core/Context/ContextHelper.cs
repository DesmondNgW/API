using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X.Util.Entities;
using X.Util.Entities.Interface;

namespace X.Util.Core.Context
{
    public class ContextHelper
    {
        public static List<IContext<TResult, TChannel>> GetContext<TResult, TChannel>(IProvider<TChannel> channel, Func<TResult, bool> callSuccess, MethodBase method, bool needElapsed, bool needLogInfo)
        {
            var list = new List<IContext<TResult, TChannel>>
            {
                new ChannelContext<TResult, TChannel>(channel),
                new LoggerContext<TResult, TChannel>(channel, callSuccess, needElapsed, needLogInfo)
            };
            list.AddRange(method.GetCustomAttributes(true).OfType<IContext<TResult, TChannel>>());
            if (method.DeclaringType != null) list.AddRange(method.DeclaringType.GetCustomAttributes(true).OfType<IContext<TResult, TChannel>>());
            return list;
        }

        public static List<IContext<TChannel>> GetContext<TChannel>(IProvider<TChannel> channel, MethodBase method, bool needElapsed, bool needLogInfo)
        {
            var list = new List<IContext<TChannel>>
            {
                new ChannelContext<TChannel>(channel),
                new LoggerContext<TChannel>(channel, needElapsed, needLogInfo)
            };
            list.AddRange(method.GetCustomAttributes(true).OfType<IContext<TChannel>>());
            if (method.DeclaringType != null) list.AddRange(method.DeclaringType.GetCustomAttributes(true).OfType<IContext<TChannel>>());
            return list;
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
                    ActionHeader = header
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
                    ActionHeader = header
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
