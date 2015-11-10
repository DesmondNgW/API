using System;
using X.Interface.Dto;

namespace X.UI.API.Model
{
    public class ApiRequestContext : HttpRequestContext
    {
        /// <summary>
        /// UserAgent
        /// </summary>     
        public string UserAgent { get; set; }

        /// <summary>
        /// ClientIP
        /// </summary>
        public string ClientIp { get; set; }

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
