using System.Collections.Generic;

namespace X.Interface.Dto.HttpRequest
{
    public class ApiRequestDto
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// post参数
        /// </summary>
        public object Postdata { get; set; }

        /// <summary>
        /// Get参数
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }

        /// <summary>
        /// 额外头部
        /// </summary>
        public Dictionary<string, string> ExtendHeaders { get; set; }
    }
}
