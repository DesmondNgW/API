using System;

namespace X.Util.Entities
{
    public class ApiRequestContext : HttpRequestContext
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
        public DateTime Now { get; set; }

        /// <summary>
        /// ActionArgument 参数
        /// </summary>
        public string ActionArgument { get; set; }

        /// <summary>
        /// 处理当前请求的线程id
        /// </summary>
        public string Cid { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string UserInfo { get; set; }
    }
}
