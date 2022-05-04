using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using X.Util.Core;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Util.Other
{
    public class QueryInfo
    {
        private const string SqlStr = @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";

        /// <summary>
        /// Xss过滤
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FilterXss(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            var regex = new Regex("javascript|vbscript|expression|applet|meta|xml|blink|link|style|script|embed|object|iframe|frame|frameset|ilayer|layer|bgsound|title|base|alert|onabort|onactivate|onafterprint|onafterupdate|onbeforeactivate|onbeforecopy|onbeforecut|onbeforedeactivate|onbeforeeditfocus|onbeforepaste|onbeforeprint|onbeforeunload|onbeforeupdate|onblur|onbounce|oncellchange|onchange|onclick|oncontextmenu|oncontrolselect|oncopy|oncut|ondataavailable|ondatasetchanged|ondatasetcomplete|ondblclick|ondeactivate|ondrag|ondragend|ondragenter|ondragleave|ondragover|ondragstart|ondrop|onerror|onerrorupdate|onfilterchange|onfinish|onfocus|onfocusin|onfocusout|onhelp|onkeydown|onkeypress|onkeyup|onlayoutcomplete|onload|onlosecapture|onmousedown|onmouseenter|onmouseleave|onmousemove|onmouseout|onmouseover|onmouseup|onmousewheel|onmove|onmoveend|onmovestart|onpaste|onpropertychange|onreadystatechange|onreset|onresize|onresizeend|onresizestart|onrowenter|onrowexit|onrowsdelete|onrowsinserted|onscroll|onselect|onselectionchange|onselectstart|onstart|onstop|onsubmit|onunload");
            while (regex.IsMatch(html))
            {
                html = regex.Replace(html, p => string.Empty);
            }
            return html;
        }

        /// <summary>
        /// 验证sql注入
        /// </summary>
        /// <param name="inputObj"></param>
        /// <returns></returns>
        public static bool ProcessSqlStr(object inputObj)
        {
            try
            {
                if (inputObj != null)
                {
                    var inputString = inputObj.ToString();
                    if (!inputString.IsNullOrEmpty())
                    {
                        if (Regex.IsMatch(inputString, @"\b(" + SqlStr + @")\b", RegexOptions.IgnoreCase)) return false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new[] { inputObj }), e, LogDomain.Util);
                return false;
            }
            return true;
        }

        #region GetQuery
        public static string GetQueryString(HttpContext httpContext, string strName, string defValue)
        {
            try
            {
                var decode = FilterXss(HttpUtility.UrlDecode(httpContext.Request.Query[strName].ToString() ?? defValue));
                return decode != null ? decode.Trim() : defValue;
            }
            catch
            {
                return defValue;
            }
        }

        public static bool GetQueryBoolean(HttpContext httpContext, string strName, bool defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Boolean(defValue);
        }

        public static byte GetQueryByte(HttpContext httpContext, string strName, byte defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Byte(defValue);
        }

        public static short GetQueryShort(HttpContext httpContext, string strName, short defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Int16(defValue);
        }

        public static int GetQueryInt(HttpContext httpContext, string strName, int defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Int32(defValue);
        }

        public static long GetQueryLong(HttpContext httpContext, string strName, long defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Int64(defValue);
        }

        public static decimal GetQueryDecimal(HttpContext httpContext, string strName, decimal defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Decimal(defValue);
        }

        public static double GetQueryDouble(HttpContext httpContext, string strName, double defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Double(defValue);
        }

        public static float GetQuerySingle(HttpContext httpContext, string strName, float defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2Single(defValue);
        }

        public static DateTime GetQueryDateTime(HttpContext httpContext, string strName, string format, DateTime defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).Convert2DateTime(format, defValue);
        }
        #endregion

        #region PostQuery
        public static string PostQueryString(HttpContext httpContext, string strName, string defValue)
        {
            if (Equals(httpContext, null)) return defValue;
            var dictionary = (IDictionary<string, object>)httpContext.Items["paramsDictionary"];
            if (dictionary != null) return dictionary.ContainsKey(strName) ? dictionary[strName].ToString() : defValue;
            var collection = (NameValueCollection)httpContext.Items["paramsCollection"];
            if (collection != null) return collection[strName] ?? defValue;
            var reader = new StreamReader(httpContext.Request.Body);
            var urlDecode = FilterXss(HttpUtility.UrlDecode(reader.ReadToEnd()));
            if (Equals(urlDecode, null)) return string.Empty;
            var paramsInput = urlDecode.Trim();
            if (string.IsNullOrWhiteSpace(paramsInput)) return defValue;
            if (paramsInput.IsJson())
            {
                dictionary = paramsInput.FromJson<IDictionary<string, object>>();
                dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
                httpContext.Items["paramsDictionary"] = dictionary;
                return dictionary.ContainsKey(strName) ? dictionary[strName].ToString() : defValue;
            }
            collection = HttpUtility.ParseQueryString(paramsInput);
            httpContext.Items["paramsCollection"] = collection;
            return collection[strName] ?? defValue;
        }

        public static bool PostQueryBoolean(HttpContext httpContext, string strName, bool defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Boolean(defValue);
        }

        public static byte PostQueryByte(HttpContext httpContext, string strName, byte defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Byte(defValue);
        }

        public static short PostQueryShort(HttpContext httpContext, string strName, short defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Int16(defValue);
        }

        public static int PostQueryInt(HttpContext httpContext, string strName, int defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Int32(defValue);
        }

        public static long PostQueryLong(HttpContext httpContext, string strName, long defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Int64(defValue);
        }

        public static decimal PostQueryDecimal(HttpContext httpContext, string strName, decimal defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Decimal(defValue);
        }

        public static double PostQueryDouble(HttpContext httpContext, string strName, double defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Double(defValue);
        }

        public static float PostQuerySingle(HttpContext httpContext, string strName, float defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2Single(defValue);
        }

        public static DateTime PostQueryDateTime(HttpContext httpContext, string strName, string format, DateTime defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).Convert2DateTime(format, defValue);
        }
        #endregion
    }
}
