using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using X.Interface.Core;
using X.Interface.Dto;
using X.UI.API.Model;
using X.Util.Core;
using X.Util.Entities;
using X.Util.Extend.Cache;
using X.Util.Extend.Mongo;
using X.Util.Other;

namespace X.UI.API.Util
{
    public class ControllerHelper
    {
        public const string ArgumentName = "Uid";
        public const LogDomain EDomain = LogDomain.Interface;

        #region 解析ApiController上下文
        /// <summary>
        /// 获取HttpContextWrapper对象
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static HttpContextWrapper GetHttpContextWrapper(HttpActionContext actionContext)
        {
            return (HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"] ?? new HttpContextWrapper(HttpContext.Current);
        }

        /// <summary>
        /// 获取Uid参数
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static string GetUid(HttpActionContext actionContext)
        {
            var actionArguments = new Dictionary<string, object>(actionContext.ActionArguments, StringComparer.OrdinalIgnoreCase);
            var uid = actionArguments.ContainsKey(ArgumentName) ? (string)actionArguments[ArgumentName] : string.Empty;
            if (!string.IsNullOrEmpty(uid)) return uid;
            foreach (var item in actionContext.ActionArguments.Where(item => item.Value is UserRequestDtoBase))
            {
                uid = ((UserRequestDtoBase)item.Value).Uid;
                break;
            }
            return uid;
        }

        /// <summary>
        /// 检查参数、获取Api请求上下文
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static ApiRequestContext GetApiRequestContext(HttpActionContext actionContext)
        {
            if (actionContext.ActionArguments != null && actionContext.ActionArguments.Count > 0)
            {
                foreach (var item in actionContext.ActionArguments)
                {
                    if (Equals(item.Value, null)) throw new ArgumentNullException(item.Key);
                    if (!(item.Value is UserRequestDtoBase)) continue;
                    var dtoProps = item.Value.GetType().GetProperties();
                    foreach (var p in dtoProps.Where(p => Equals(p.GetValue(item.Value, null), null)))
                    {
                        throw new ArgumentNullException(p.Name);
                    }
                }
            }
            var apiContext = new ApiRequestContext();
            var apiProps = typeof(ApiRequestContext).GetProperties();
            var contextWrapper = GetHttpContextWrapper(actionContext);
            var collection = contextWrapper.Request.Headers;
            foreach (var p in apiProps) p.SetValue(apiContext, collection[p.Name], null);
            if (string.IsNullOrEmpty(apiContext.Version)) apiContext.Version = "0.0.0";
            apiContext.UserAgent = actionContext.Request.Headers.UserAgent.ToString();
            apiContext.ClientIp = IpBase.GetIp(contextWrapper);
            apiContext.ServerIp = IpBase.GetLocalIp();
            apiContext.Interface = actionContext.Request.RequestUri.AbsolutePath;
            apiContext.ActionArgument = actionContext.ActionArguments.ToJson();
            apiContext.Now = DateTime.Now;
            apiContext.Cid = Thread.CurrentThread.ManagedThreadId.ToString();
            if (string.IsNullOrEmpty((apiContext.Token))) throw new ArgumentNullException(nameof(apiContext.Token));
            if (!ServiceHelper.VerifyToken(apiContext.Token)) throw new InvalidOperationException("token错误或过期");
            return apiContext;
        } 
        #endregion

        #region Api内部缓存机制
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="guid"></param>
        private static void ClearPageCache(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid)) return;
            var list = RuntimeCache.Get(guid) as IList<string>;
            if (Equals(list, null)) return;
            foreach (var key in list)
            {
                RuntimeCache.Remove(key);
                CouchCache.Default.Remove(key);
            }
            RuntimeCache.Remove(guid);
        }

        /// <summary>
        /// 初始化缓存
        /// </summary>
        /// <param name="noCache"></param>
        public static void CacheInit(bool noCache)
        {
            var token = ExecutionContext<RequestContext>.Current.Ptoken;
            if (string.IsNullOrWhiteSpace(token)) return;
            if (noCache) ClearPageCache(token);
            else RuntimeCache.Remove(token);
        }
        #endregion

        #region 对外Api
        /// <summary>
        /// 监控Api调用记录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static void ApiCallMonitor(ApiRequestContext context)
        {
            MongoDbBase.Default.AddMongo(context, "APIMonitor", DateTime.Now.ToString("Call.yyyy.MM.dd.HH"));
            ExecutionContext<ApiRequestContext>.Init(context);
        }

        /// <summary>
        /// Action执行完毕 附加响应头部
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public static void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.Add("Tid", Thread.CurrentThread.ManagedThreadId.ToString());
            actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        /// <summary>
        /// CallSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="iresult"></param>
        /// <returns></returns>
        public static bool CallSuccess<TResult>(ApiResult<TResult> iresult)
        {
            return iresult != null && iresult.Data != null && iresult.Success;
        }

        #endregion
    }
}