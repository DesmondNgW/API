using System.Net;

namespace X.Util.Entities
{
    /// <summary>
    /// http请求结果
    /// </summary>
    public class HttpRequestResult
    {
        public string Content { get; set; }

        public bool Success { get; set; }

        public CookieCollection CookieCollection { get; set; }

        public string Cookies { get; set; }
    }
}
