using Em.FundTrade.EncryptHelper;
using System.Collections.Generic;
using System;

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
    }
}
