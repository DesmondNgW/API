using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using X.Util.Core;

namespace X.Util.Other
{
    public class IpBase
    {
		public static string GetIp(HttpContextWrapper context)
        {
            return CoreUtil.GetIp(context);
        }

        public static string GetLocalIp()
        {
            var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ip = CoreUtil.LocalIp;
            foreach (var ipa in ipEntry.AddressList)
            {
                ip = ipa.ToString();
                if (!ipa.IsIPv6LinkLocal && !ipa.IsIPv6Multicast && !ipa.IsIPv6SiteLocal && !ipa.IsIPv6Teredo && !ip.StartsWith("172.") && !ip.StartsWith("192.") && !ip.StartsWith("168.") && IpValid(ip)) break;
            }
            return ip;
        }

        public static bool IpValid(string ip)
        {
            return Regex.IsMatch(ip, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        }
    }
}
