using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using X.Util.Core;

namespace X.Util.Other
{
    public class QueryInfo
    {
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

        #region GetQuery
        public static string GetQueryString(HttpContextWrapper httpContext, string strName, string defValue)
        {
            try
            {
                var decode = FilterXss(HttpUtility.UrlDecode(httpContext.Request.QueryString[strName] ?? defValue));
                return decode != null ? decode.Trim() : defValue;
            }
            catch
            {
                return defValue;
            }
        }

        public static bool GetQueryBoolean(HttpContextWrapper httpContext, string strName, bool defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetBoolean(defValue);
        }

        public static byte GetQueryByte(HttpContextWrapper httpContext, string strName, byte defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetByte(defValue);
        }

        public static short GetQueryShort(HttpContextWrapper httpContext, string strName, short defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetInt16(defValue);
        }

        public static int GetQueryInt(HttpContextWrapper httpContext, string strName, int defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetInt32(defValue);
        }

        public static long GetQueryLong(HttpContextWrapper httpContext, string strName, long defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetInt64(defValue);
        }

        public static decimal GetQueryDecimal(HttpContextWrapper httpContext, string strName, decimal defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetDecimal(defValue);
        }

        public static double GetQueryDouble(HttpContextWrapper httpContext, string strName, double defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetDouble(defValue);
        }

        public static float GetQuerySingle(HttpContextWrapper httpContext, string strName, float defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetSingle(defValue);
        }

        public static DateTime GetQueryDateTime(HttpContextWrapper httpContext, string strName, string format, DateTime defValue)
        {
            return GetQueryString(httpContext, strName, string.Empty).GetDateTime(format, defValue);
        }
        #endregion

        #region PostQuery
        public static string PostQueryString(HttpContextWrapper httpContext, string strName, string defValue)
        {
            if (Equals(httpContext, null)) return defValue;
            var dictionary = (IDictionary<string, object>)httpContext.Items["paramsDictionary"];
            if (dictionary != null) return dictionary.ContainsKey(strName) ? dictionary[strName].ToString() : defValue;
            var collection = (NameValueCollection)httpContext.Items["paramsCollection"];
            if (collection != null) return collection[strName] ?? defValue;
            var reader = new StreamReader(httpContext.Request.InputStream);
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

        public static bool PostQueryBoolean(HttpContextWrapper httpContext, string strName, bool defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetBoolean(defValue);
        }

        public static byte PostQueryByte(HttpContextWrapper httpContext, string strName, byte defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetByte(defValue);
        }

        public static short PostQueryShort(HttpContextWrapper httpContext, string strName, short defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetInt16(defValue);
        }

        public static int PostQueryInt(HttpContextWrapper httpContext, string strName, int defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetInt32(defValue);
        }

        public static long PostQueryLong(HttpContextWrapper httpContext, string strName, long defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetInt64(defValue);
        }

        public static decimal PostQueryDecimal(HttpContextWrapper httpContext, string strName, decimal defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetDecimal(defValue);
        }

        public static double PostQueryDouble(HttpContextWrapper httpContext, string strName, double defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetDouble(defValue);
        }

        public static float PostQuerySingle(HttpContextWrapper httpContext, string strName, float defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetSingle(defValue);
        }

        public static DateTime PostQueryDateTime(HttpContextWrapper httpContext, string strName, string format, DateTime defValue)
        {
            return PostQueryString(httpContext, strName, string.Empty).GetDateTime(format, defValue);
        }
        #endregion
    }
}
