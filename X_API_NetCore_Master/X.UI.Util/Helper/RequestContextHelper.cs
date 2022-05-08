using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading;
using X.Business.Core;
using X.Interface.Core;
using X.Interface.Dto;
using X.UI.Util.Model;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Entities;

namespace X.UI.Util.Helper
{
    public class RequestContextHelper
    {
        public static ApiRequestContext GetApiRequestContext(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var headers = request.Headers;
            var ApiRequestContext = new ApiRequestContext()
            {
                RequestId = Guid.NewGuid().ToString("N"),
                UserAgent = headers["User-Agent"],
                ServerIp = IpBase.GetLocalIp(),
                Interface = request.GetDisplayUrl(),
                ServerTime = DateTime.Now,
                Cid = Thread.CurrentThread.ManagedThreadId.ToString(),
                ActionArgument = context.ActionArguments.ToJson()
            };

            var headersContext = new HttpRequestContext4Heads();
            var props = typeof(HttpRequestContext4Heads).GetProperties();
            foreach (var p in props.Where(p => headers.ContainsKey(p.Name)))
            {
                p.SetValue(headersContext, headers[p.Name], null);
            }
            ApiRequestContext.Heads = headersContext;
            return ApiRequestContext;
        }

        public static BusinessRequestContext GetBusinessRequestContext(ApiRequestContext context, bool isLogin)
        {
            //验证Token
            TokenHelper.VerifyToken(context.Heads.Token, context.Heads.ClientId, context.Heads.ClientIp, context.UserAgent, context.Interface);
            //验证客户端时间
            var ts = context.ServerTime - new DateTime(context.Heads.Timestamp);
            if (ts.Minutes >= 30 || ts.Minutes <= -30)
            {
                throw new InvalidOperationException("Timestamp和服务器校准不一致");
            }
            //验证登录
            if (isLogin)
            {
                TokenHelper.VerifyUToken(context.Heads.UToken, context.Interface);
            }
            return null;

        }
    }
}
