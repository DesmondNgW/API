using System;
using System.Web;
using X.Util.Core;
using X.Util.Core.Configuration;

namespace X.Util.Other
{
    public class CookieHelper
    {
        #region Cookie
        public static string GetCookie(HttpContextWrapper context, string strName)
        {
            if (Equals(context, null)) return string.Empty;
            var cookie = context.Request.Cookies[strName];
            return cookie != null && !string.IsNullOrEmpty(cookie.Value) ? cookie.Value : string.Empty;
        }

        public static HttpCookie SetCookie(HttpContextWrapper context, string strName, bool ifExistUndo, string value, DateTime? expires = null, string path = "", string domain = "", bool? secure = null, bool? httpOnly = null)
        {
            if (Equals(context, null)) return null;
            var cookie = context.Request.Cookies[strName];
            if (cookie != null && ifExistUndo) return cookie;
            if (Equals(cookie, null)) cookie = new HttpCookie(strName);
            if (expires != null) cookie.Expires = (DateTime)expires;
            if (!string.IsNullOrWhiteSpace(path)) cookie.Path = path;
            if (!string.IsNullOrWhiteSpace(domain)) cookie.Domain = domain;
            if (secure != null) cookie.Secure = (bool)secure;
            if (httpOnly != null) cookie.HttpOnly = (bool)httpOnly;
            cookie.Value = value;
            context.Response.Cookies.Add(cookie);
            return cookie;
        }

        public static HttpCookie SetCookie(HttpContextWrapper context, string strName, string value)
        {
            return SetCookie(context, strName, false, value, null, string.Empty, AppConfig.CookieDomain);
        }

        public static HttpCookie SetCookie(HttpContextWrapper context, string strName, string value, string cookieDomain)
        {
            return SetCookie(context, strName, false, value, null, string.Empty, cookieDomain);
        }

        public static HttpCookie SetCookie(HttpContextWrapper context, string strName, string value, DateTime expires)
        {
            return SetCookie(context, strName, false, value, expires, string.Empty, AppConfig.CookieDomain);
        }

        public static HttpCookie SetCookie(HttpContextWrapper context, string strName, string value, DateTime expires, string cookieDomain)
        {
            return SetCookie(context, strName, false, value, expires, string.Empty, cookieDomain);
        }

        public static void RemoveCookie(HttpContextWrapper context, string strName)
        {
            var cookie = context != null ? context.Request.Cookies[strName] : null;
            if (Equals(cookie, null)) return;
            cookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Remove(strName);
            context.Response.Cookies.Add(cookie);
        }
        #endregion
    }
}
