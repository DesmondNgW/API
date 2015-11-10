using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace X.UI.API.Util
{
    /// <summary>
    /// 需要OutputCache的Action加此特性
    /// </summary>
    public class OutputCacheAttribute : ActionFilterAttribute
    {
        public double Duration { get; set; }

        /// <summary>
        /// 在调用操作方法之后发生
        /// </summary>
        /// <param name="actionExecutedContext">操作执行的上下文</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromMinutes(Duration),
                MustRevalidate = true,
                Public = true
            };
        }
    }
}