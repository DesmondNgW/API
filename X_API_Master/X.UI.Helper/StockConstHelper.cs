using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.UI.Helper
{
    public class StockConstHelper
    {
        #region 普通业务常量
        public static readonly string ST = "ST";

        public static readonly string KZZ = "转债";

        public static readonly string CYB = "创业板";

        public static readonly string ZB = "主板";

        public static readonly string AQS = "AQS";

        public static readonly string WAVE = "Wave";

        public static readonly string AUTO = "auto";
        #endregion

        #region 文件路径常量
        public static readonly string JXPATH = "./src/dp/精选.txt";

        public static readonly string AQSPATH = "./src/fp/AQS.txt";

        public static readonly string JXPATH2 = "./src/dp/精选2.txt";

        public static readonly string KERNELJXPATH = "./src/dp/Kernel精选.txt";

        public static readonly string WAVEPATH = "./src/fp/Wave.txt";

        public static readonly string DIRPATH = "./dest";

        public static readonly string DIRKPATH = "./dest/K";

        public static readonly string DIRAPATH = "./dest/A";

        public static readonly string DIRBKPATH = "./dest/Bk";

        public static readonly string FIRSTPATH = "./src/dp/首板.txt";

        public static readonly string ZTPATH = "./src/dp/涨停.txt";

        public static readonly string CORETPATH = "./src/dp/CORET.txt";

        public static readonly string CORETPATH2 = "./src/dp/CORET2.txt";

        public static readonly string CORETPATH3 = "./src/dp/CORET3.txt";

        public static readonly string KERNELPATH = "./src/dp/Kernel.txt";

        public static readonly string KERNELHPATH = "./src/dp/KernelH.txt";

        public static readonly string KERNELLPATH = "./src/dp/KernelL.txt";

        public static readonly string JLPATH = "./src/dp/接力.txt";

        public static readonly string TOOLPATH = "./src/dp/tool.txt";

        public static readonly string DPBKPATH = "./src/dp/bk.txt";

        public static readonly string BAKPATH = "./src/data/bak.txt";

        public static readonly string DATABASEPATH = "./src/data/database.txt";
        #endregion

        #region 字符常量
        public static readonly string GB2312 = "gb2312";

        public static readonly string UTF8 = "utf-8";

        public static readonly string RN = "\r\n";

        public static readonly string TN = "\t\n";

        public static readonly char T = '\t';

        public static readonly string REGEXSPACE = "[\\S]+";

        public static readonly string REGEXBK = "(.+)：【(.+)】";
        #endregion

    }
}
