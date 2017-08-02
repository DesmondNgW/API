using System;
using System.Collections.Generic;
using System.Reflection;

namespace X.Util.Entities
{
    /// <summary>
    /// 请求调用的方法
    /// </summary>
    public class RequestMethodInfo
    {
        /// <summary>
        /// 请求id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public Type ClassName { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 调用地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, object> ParamList { get; set; }

        /// <summary>
        /// 内部使用的真实方法
        /// </summary>
        public MethodBase Method { get; set; }

        /// <summary>
        /// 客户端唯一标识(没有则随机生成)
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端Ip
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 服务器Ip
        /// </summary>
        public string ServerIp { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public Dictionary<string, object> ExtendInfo { get; set; }
    }
}
