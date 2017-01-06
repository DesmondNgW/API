using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http.Filters;
using X.Interface.Dto;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

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
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { }, actionExecutedContext.Request.RequestUri.ToString()), actionExecutedContext.Exception, LogDomain.Ui);
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