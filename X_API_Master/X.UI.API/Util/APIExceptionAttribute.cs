using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using X.Interface.Dto;
using X.Util.Core;

namespace X.UI.API.Util
{
    public class ApiExceptionAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 引发异常事件
        /// </summary>
        /// <param name="actionExecutedContext">操作的上下文</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            var message = actionExecutedContext.Exception.Message;
            var statusCode = HttpStatusCode.OK;
            if ("404".Equals(message))
            {
                message = string.Format("未找到与请求 URI“{0}”匹配的 HTTP 资源。", actionExecutedContext.Request.RequestUri);
                statusCode = HttpStatusCode.NotFound;
            }
            else
            {
                string errorInfo = string.Format("请求URI: {0} 发生异常, 异常信息: {1}.", actionExecutedContext.Request.RequestUri, actionExecutedContext.Exception);
                Logger.Error(System.Reflection.MethodBase.GetCurrentMethod(), LogDomain.Ui, null, errorInfo);
            }
            var newContent = new ApiResult<string>
            {
                Success = false,
                Error = message,
                DebugError = message
            };
            var contentType = actionExecutedContext.Request.Headers.Accept.ToString();
            ObjectContent content = contentType.ToLower().Contains("application/json") ? new ObjectContent<ApiResult<string>>(newContent, new JsonMediaTypeFormatter()) : new ObjectContent<ApiResult<string>>(newContent, new XmlMediaTypeFormatter());
            actionExecutedContext.Response = new HttpResponseMessage { Content = content, StatusCode = statusCode };
        }
    }
}