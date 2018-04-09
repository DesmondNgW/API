using Em.FundTrade.EncryptHelper;

namespace X.UI.Consoles
{
    public class ConsoleHelper
    {
        public static EncryptResult MobileEncrypt(string incoming)
        {
            return EncryptHelper.Instance.MobileEncrypt(incoming);
        }
    }
}
