using System;
using X.Interface.Dto;

namespace X.UI.Util.Model
{
    public class ApiRequestContext
    {
        /// <summary>
        /// RequestId
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// API-IP
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// Current接口
        /// </summary>
        public string Interface { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime ServerTime { get; set; }

        /// <summary>
        /// ActionArgument 参数
        /// </summary>
        public string ActionArgument { get; set; }

        /// <summary>
        /// 处理当前请求的线程id
        /// </summary>
        public string Cid { get; set; }

        /// <summary>
        /// HttpRequestContext4Heads 客户端传递
        /// </summary>
        public HttpRequestContext4Heads Heads { get; set; }
    }
}
