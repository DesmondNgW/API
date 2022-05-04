using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace X.Util.Core.Kernel
{
    public class IpBase
    {
        public const string LocalIp = "127.0.0.1";
        /// <summary>
        /// GetIP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            return Equals(HttpContextHelper.Current, null) ? LocalIp : GetIp(HttpContextHelper.Current);
        }

        public static string GetIp(HttpContext context)
        {
            if (Equals(context, null)) return LocalIp;
            var ips = new[] { LocalIp, context.Request.Headers["REMOTE_ADDR"].ToString(), context.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString() };
            var length = ips.Length;
            var ret = ips[0];
            while (length-- > 0)
            {
                if (string.IsNullOrWhiteSpace(ips[length]) || Equals(ips[length], "::1")) continue;
                ret = ips[length];
                break;
            }
            return ret;
        }

        public static string GetLocalIp()
        {
            var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ip = LocalIp;
            foreach (var ipa in ipEntry.AddressList)
            {
                ip = ipa.ToString();
                if (!ipa.IsIPv6LinkLocal && !ipa.IsIPv6Multicast && !ipa.IsIPv6SiteLocal && !ipa.IsIPv6Teredo && !ip.StartsWith("172.") && !ip.StartsWith("192.") && !ip.StartsWith("168.") && IpValid(ip)) break;
            }
            return ip;
        }

        public static bool IpValid(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}
