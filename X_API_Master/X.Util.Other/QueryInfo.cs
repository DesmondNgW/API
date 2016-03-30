﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using X.Util.Core;

namespace X.Util.Other
{
    public class QueryInfo
    {
        #region GetQuery
        public static string GetQueryString(HttpContextWrapper httpContext, string strName, string defValue)
        {
            try
            {
                var decode = HttpUtility.UrlDecode(httpContext.Request.QueryString[strName] ?? defValue);
                return decode != null ? decode.Trim() : defValue;
            }
            catch
            {
                return defValue;
            }
        }

        public static bool GetQueryBoolean(HttpContextWrapper httpContext, string strName, bool defValue)
        {
            return CoreParse.GetBoolean(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static byte GetQueryByte(HttpContextWrapper httpContext, string strName, byte defValue)
        {
            return CoreParse.GetByte(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static short GetQueryShort(HttpContextWrapper httpContext, string strName, short defValue)
        {
            return CoreParse.GetInt16(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static int GetQueryInt(HttpContextWrapper httpContext, string strName, int defValue)
        {
            return CoreParse.GetInt32(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static long GetQueryLong(HttpContextWrapper httpContext, string strName, long defValue)
        {
            return CoreParse.GetInt64(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static decimal GetQueryDecimal(HttpContextWrapper httpContext, string strName, decimal defValue)
        {
            return CoreParse.GetDecimal(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static double GetQueryDouble(HttpContextWrapper httpContext, string strName, double defValue)
        {
            return CoreParse.GetDouble(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static float GetQuerySingle(HttpContextWrapper httpContext, string strName, float defValue)
        {
            return CoreParse.GetSingle(GetQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static DateTime GetQueryDateTime(HttpContextWrapper httpContext, string strName, string format, DateTime defValue)
        {
            return CoreParse.GetDateTime(GetQueryString(httpContext, strName, string.Empty), format, defValue);
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
            var urlDecode = HttpUtility.UrlDecode(reader.ReadToEnd());
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
            return CoreParse.GetBoolean(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static byte PostQueryByte(HttpContextWrapper httpContext, string strName, byte defValue)
        {
            return CoreParse.GetByte(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static short PostQueryShort(HttpContextWrapper httpContext, string strName, short defValue)
        {
            return CoreParse.GetInt16(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static int PostQueryInt(HttpContextWrapper httpContext, string strName, int defValue)
        {
            return CoreParse.GetInt32(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static long PostQueryLong(HttpContextWrapper httpContext, string strName, long defValue)
        {
            return CoreParse.GetInt64(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static decimal PostQueryDecimal(HttpContextWrapper httpContext, string strName, decimal defValue)
        {
            return CoreParse.GetDecimal(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static double PostQueryDouble(HttpContextWrapper httpContext, string strName, double defValue)
        {
            return CoreParse.GetDouble(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static float PostQuerySingle(HttpContextWrapper httpContext, string strName, float defValue)
        {
            return CoreParse.GetSingle(PostQueryString(httpContext, strName, string.Empty), defValue);
        }

        public static DateTime PostQueryDateTime(HttpContextWrapper httpContext, string strName, string format, DateTime defValue)
        {
            return CoreParse.GetDateTime(PostQueryString(httpContext, strName, string.Empty), format, defValue);
        }
        #endregion
    }
}