﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Extend.Cache;

namespace X.Util.Other
{
    public class HttpRequestBase
    {
        private static HttpRequestResult GetHttpRequest(string uri, string charset, string data, string method, string contentType, Dictionary<string, string> extendHeaders, string cookie)
        {
            var result = new HttpRequestResult() { Success = false };
            var request = (HttpWebRequest)WebRequest.Create(uri);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encode = charset.Contains("utf8") || charset.Contains("utf-8") ? Encoding.UTF8 : charset.Contains("unicode") ? Encoding.Unicode : Encoding.GetEncoding(charset);
            if (uri.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => SslPolicyErrors.None.Equals(errors);
            request.KeepAlive = true;
            if (!string.IsNullOrWhiteSpace(cookie)) request.Headers.Add("Cookie:" + cookie);
            request.CookieContainer = new CookieContainer();
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
            request.Timeout = 60000;
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            if (extendHeaders != null)
            {
                foreach (var item in extendHeaders) request.Headers[item.Key] = item.Value;
            }
            if (!string.IsNullOrEmpty(contentType)) request.ContentType = contentType;
            if (!string.IsNullOrWhiteSpace(data))
            {
                var dataBytes = encode.GetBytes(data);
                var stream = request.GetRequestStream();
                stream.Write(dataBytes, 0, dataBytes.Length);
                stream.Close();
            }
            else
            {
                request.ContentLength = 0;
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                result.Cookies = request.CookieContainer.GetCookieHeader(request.RequestUri);
                result.CookieCollection = request.CookieContainer.GetCookies(request.RequestUri);
                var content = string.Empty;
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var sr = new StreamReader(responseStream, encode);
                    content = sr.ReadToEnd();
                    sr.Close();
                }
                result.Content = content;
                result.Success = true;
                request.Abort();
                response.Close();
                return result;
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { uri, charset, data, method, contentType, extendHeaders, cookie }), e, LogDomain.Util);
            }
            return result;
        }

        /// <summary>
        /// GET HTTP
        /// </summary>
        public static HttpRequestResult GetHttpInfo(string uri, string charset, string contentType, Dictionary<string, string> extendHeaders, string cookie)
        {
            return GetHttpRequest(uri, charset, string.Empty, "GET", contentType, extendHeaders, cookie);
        }

        /// <summary>
        /// POST HTTP
        /// </summary>
        public static HttpRequestResult PostHttpInfo(string uri, string charset, string postdata, string contentType, Dictionary<string, string> extendHeaders, string cookie)
        {
            return GetHttpRequest(uri, charset, postdata, "POST", contentType, extendHeaders, cookie);
        }

        /// <summary>
        /// 含304的静态文件HTTP请求
        /// </summary>
        public static HttpCacheResult<T> GetStaticInfo<T>(string uri, Func<HttpWebResponse, T> loader)
        {
            var cache = RuntimeCache.Get<HttpCacheResult<T>>(uri);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 10000;
                request.IfModifiedSince = cache != null ? cache.LastModified : DateTime.MinValue;
                var response = (HttpWebResponse)request.GetResponse();
                cache = new HttpCacheResult<T> { CacheKey = uri, Result = loader(response), LastModified = response.LastModified };
                RuntimeCache.Set(uri, cache, new TimeSpan(1, 0, 0), string.Empty);
                request.Abort();
                response.Close();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (Equals(response, null) || (HttpStatusCode.NotModified.Equals(response.StatusCode) || response.StatusCode.GetHashCode() < 300)) return cache;
                cache = new HttpCacheResult<T> { Result = default };
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { uri }), ex, LogDomain.Util);
            }
            catch (Exception ex)
            {
                cache = new HttpCacheResult<T> { Result = default };
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { uri }), ex, LogDomain.Util);
            }
            return cache;
        }

        /// <summary>
        /// 表单自动提交
        /// </summary>
        /// <param name="formAction"></param>
        /// <param name="formName"></param>
        /// <param name="formMethod"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string FormSubmit(string formAction, string formName, string formMethod, IDictionary<string, string> param)
        {
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><title>" + formName + "</title></head><body><form id='form1' name='" + formName + "' action='" + formAction + "' method='" + formMethod + "'>");
            if (param != null && param.Count > 0)
            {
                foreach (var item in param) sb.Append("<input type='hidden' name='" + item.Key + "' value ='" + item.Value + "' id='" + item.Key + "' />");
            }
            sb.Append("</form><script type='text/javascript'>document.getElementById('form1').submit()</script></body></html>");
            return sb.ToString();
        }
    }
}
