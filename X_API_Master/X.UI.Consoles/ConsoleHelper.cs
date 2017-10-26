using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Em.FundTrade.EncryptHelper;
using Microsoft.SqlServer.Server;
using X.Util.Core;
using X.Util.Entities.Enums;
using X.Util.Other;

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
