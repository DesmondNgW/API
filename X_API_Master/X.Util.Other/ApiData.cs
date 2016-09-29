using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using X.Util.Core;

namespace X.Util.Other
{
    public class ApiData
    {
        /// <summary>
        /// Url路径相关正则
        /// </summary>
        private static readonly Regex DotRe = new Regex(@"\/\.\/");
        private static readonly Regex DoubleDotRe = new Regex(@"\/[^/]+\/\.\.\/");
        private static readonly Regex MultiSlashRe = new Regex(@"([^:/])\/+\/");

        /// <summary>
        /// GetUri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static string GetUri(string uri, Dictionary<string, string> arguments)
        {
            if (Equals(arguments, null) || arguments.Count <= 0) return uri;
            var sb = new StringBuilder();
            sb.Append(uri.Contains("?") ? uri : uri + "?");
            foreach (var arg in arguments) sb.Append(string.Format("&{0}={1}", arg.Key, arg.Value));
            return sb.ToString().Replace("?&", "?");
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="arguments"></param>
        /// <param name="extendHeaders"></param>
        /// <returns></returns>
        public static T Get<T>(string uri, Dictionary<string, string> arguments, Dictionary<string, string> extendHeaders)
        {
            var iresult = HttpRequestBase.GetHttpInfo(GetUri(uri, arguments), "utf-8", "application/json", extendHeaders, string.Empty);
            return iresult.Success && !string.IsNullOrEmpty(iresult.Content) ? iresult.Content.FromJson<T>() : default(T);
        }

        public static string GetContent(string uri, Dictionary<string, string> arguments, string contentType, Dictionary<string, string> extendHeaders)
        {
            var iresult = HttpRequestBase.GetHttpInfo(GetUri(uri, arguments), "utf-8", contentType, extendHeaders, string.Empty);
            return iresult.Success && !string.IsNullOrEmpty(iresult.Content) ? iresult.Content : string.Empty;
        }

        /// <summary>
        /// Post data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="postdata"></param>
        /// <param name="extendHeaders"></param>
        /// <returns></returns>
        public static T Post<T>(string uri, object postdata, Dictionary<string, string> extendHeaders)
        {
            var iresult = HttpRequestBase.PostHttpInfo(uri, "utf-8", postdata.ToJson(), "application/json", extendHeaders, string.Empty);
            return iresult.Success && !string.IsNullOrEmpty(iresult.Content) ? iresult.Content.FromJson<T>() : default(T);
        }

        public static string PostContent(string uri, object postdata, Dictionary<string, string> arguments, string contentType, Dictionary<string, string> extendHeaders)
        {
            var iresult = HttpRequestBase.PostHttpInfo(uri, "utf-8", postdata.ToJson(), contentType, extendHeaders, string.Empty);
            return iresult.Success && !string.IsNullOrEmpty(iresult.Content) ? iresult.Content : string.Empty;
        }

        /// <summary>
        /// 解析成最终路径，去除多余的字符和相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRealPath(string path)
        {
            MatchEvaluator matchEvaluator = m => "/";
            path = DotRe.Replace(path, matchEvaluator);
            path = MultiSlashRe.Replace(path, m => m.Groups[1].ToString() + "/");
            while (DoubleDotRe.IsMatch(path)) path = DoubleDotRe.Replace(path, matchEvaluator);
            return path;
        }
    }
}
