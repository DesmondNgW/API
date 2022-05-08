using X.Util.Entities;

namespace X.Interface.Dto
{
    /// <summary>
    /// 请求上下文（把相应参数通过请求头部传递）
    /// </summary>
    public class HttpRequestContext4Heads : MongoBaseModel
    {
        #region 客户端信息
        /// <summary>
        /// 分辨率(Web情况是用户设备分辨率，其余是设备分辨率)
        /// </summary>
        public string Screen { get; set; }

        /// <summary>
        /// 操作系统平台以及版本号(Web情况是用户操作系统以及版本号，其余为设备系统以及版本号)
        /// </summary>
        public string PlatForm { get; set; }

        /// <summary>
        /// 客户端id(Web情况是用户电脑唯一id，其余为设备唯一id)
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// ClientIP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 客户端版本(Web情况是web服务器站点版本，其余为App版本)
        /// </summary>
        public string ClientVersion { get; set; }

        /// <summary>
        /// 客户端型号(Web情况是用户设备名称-可用IP代替，其余为设备名称)
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户端来源类型(参考EnumClientType)
        /// Web = 0, Tablet = 1, Mobile = 2, ThirdParty = 3
        /// </summary>
        public int ClientType { get; set; }
        #endregion

        #region 业务信息
        /// <summary>
        /// token(必须传递)-每次请求必须传递
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// UToken（登录状态下必须传递）
        /// </summary>
        public string UToken { get; set; }

        /// <summary>
        /// API版本号（类似0.0.0）
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Timestamp(必须传递)
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Prev Ctoken
        /// </summary>
        public string PCToken { get; set; }

        #endregion

        #region 服务器信息
        /// <summary>
        /// Last Request Thread id
        /// </summary>
        public string Cid { get; set; }
        #endregion
    }
}
