using Em.FundTrade.EncryptHelper;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace X.UI.Consoles
{
    public class ConsoleHelper
    {
        public static EncryptResult MobileEncrypt(string incoming)
        {
            return EncryptHelper.Instance.MobileEncrypt(incoming);
        }

        public static List<string> Draw(string chat, int num)
        {
            var ret = new List<string>();
            var i = 0;
            var add = true;
            while (i >= 0)
            {
                ret.Add(string.Join(chat, new string[2 * i + 2]).PadLeft(num + i));
                if (add && i == num - 1) add = false;
                i = add ? i + 1 : i - 1;
            }
            return ret;
        }

        public static uint ConvertIpv4(string ipv4)
        {
            Match match = Regex.Match(ipv4, "^\\s*(\\d{1,3})\\s*\\.\\s*(\\d{1,3})\\s*\\.\\s*(\\d{1,3})\\s*\\.\\s*(\\d{1,3})\\s*$");
            if (match.Groups.Count != 5)
            {
                throw new ArgumentException("参数格式不正确");
            }
            return uint.Parse(match.Groups[1].ToString()) * 256 * 256 * 256 +
                uint.Parse(match.Groups[2].ToString()) * 256 * 256 +
                uint.Parse(match.Groups[3].ToString()) * 256 +
                uint.Parse(match.Groups[4].ToString());
        }
    }
}
