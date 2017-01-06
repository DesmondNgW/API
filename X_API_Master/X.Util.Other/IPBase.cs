using System.Web;
using X.Util.Core.Kernel;

namespace X.Util.Other
{
    public class IpBase
    {
        public static string GetIp()
        {
            return CoreUtil.GetIp();
        }
        
        public static string GetIp(HttpContextWrapper context)
        {
            return CoreUtil.GetIp(context);
        }

        public static string GetLocalIp()
        {
            return CoreUtil.GetLocalIp();
        }

        public static bool IpValid(string ip)
        {
            return CoreUtil.IpValid(ip);
        }
    }
}
