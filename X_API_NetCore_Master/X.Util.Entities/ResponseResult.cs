using System;

namespace X.Util.Entities
{
    /// <summary>
    /// 响应结果
    /// </summary>
    public class ResponseResult
    {
        /// <summary>
        /// 请求结果
        /// </summary>
        public object Return { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string[] ExtendMessages { get; set; }

        /// <summary>
        /// 处理耗时
        /// </summary>
        public long Elapsed { get; set; }
    }
}
