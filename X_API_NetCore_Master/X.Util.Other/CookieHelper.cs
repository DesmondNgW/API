using Microsoft.AspNetCore.Http;
using System;
using X.Util.Core.Configuration;

namespace X.Util.Other
{
    public class CookieHelper
    {
        #region Cookie
        public static string GetCookie(HttpContext context, string strName)
        {
            if (Equals(context, null)) return string.Empty;
            var cookie = context.Request.Cookies[strName];
            return cookie != null && !string.IsNullOrEmpty(cookie) ? cookie : string.Empty;
        }

        public static void SetCookie(HttpContext context, string strName, bool ifExistUndo, string value, DateTime? expires = null, string path = "", string domain = "", bool? secure = null, bool? httpOnly = null)
        {
            if (Equals(context, null)) return;
            var cookie = context.Request.Cookies[strName];
            if (cookie != null && ifExistUndo) return;
            var option = new CookieOptions();
            if (expires != null) option.Expires = (DateTime)expires;
            if (!string.IsNullOrWhiteSpace(path)) option.Path = path;
            if (!string.IsNullOrWhiteSpace(domain)) option.Domain = domain;
            if (secure != null) option.Secure = (bool)secure;
            if (httpOnly != null) option.HttpOnly = (bool)httpOnly;
            context.Response.Cookies.Append(strName, value, option);
        }

        public static void SetCookie(HttpContext context, string strName, string value)
        {
            SetCookie(context, strName, false, value, null, string.Empty, AppConfig.CookieDomain);
        }

        public static void SetCookie(HttpContext context, string strName, string value, string cookieDomain)
        {
            SetCookie(context, strName, false, value, null, string.Empty, cookieDomain);
        }

        public static void SetCookie(HttpContext context, string strName, string value, DateTime expires)
        {
            SetCookie(context, strName, false, value, expires, string.Empty, AppConfig.CookieDomain);
        }

        public static void SetCookie(HttpContext context, string strName, string value, DateTime expires, string cookieDomain)
        {
            SetCookie(context, strName, false, value, expires, string.Empty, cookieDomain);
        }

        public static void RemoveCookie(HttpContext context, string strName)
        {
            var cookie = context != null ? context.Request.Cookies[strName] : string.Empty;
            if (string.IsNullOrEmpty(cookie)) return;
            SetCookie(context, strName, string.Empty, DateTime.MinValue);
            context.Response.Cookies.Delete(strName);
        }
        #endregion
    }
}
