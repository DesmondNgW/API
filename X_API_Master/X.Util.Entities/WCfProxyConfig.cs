using System.Collections.Generic;

namespace X.Util.Entities
{
    public enum MetadataExchangeClientMode
    {
        // 使用 WS-Transfer Get 请求。
        MetadataExchange = 0,
        // 使用 HTTP GET 请求。
        HttpGet = 1,
    }
    /// <summary>
    /// 
    /// </summary>
    public class WCfProxy
    {
        /// <summary>
        /// 元数据地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 元数据交换方式
        /// </summary>
        public MetadataExchangeClientMode Mode { get; set; }
        /// <summary>
        /// 客户端代理文件路径
        /// </summary>
        public string ProxyFilePath { get; set; }
    }

    public class WCfConfig
    {
        /// <summary>
        /// 客户端配置文件路径
        /// </summary>
        public string ConfigPathPath { get; set; }

        public List<WCfProxy> ProxyList { get; set; }
    }
}
