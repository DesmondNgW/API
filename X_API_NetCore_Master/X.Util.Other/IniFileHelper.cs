using System.Runtime.InteropServices;
using System.Text;

namespace X.Util.Other
{
    /// <summary>
    /// INI文件读写类。
    /// Copyright (C) Maticsoft
    /// </summary>
    public class IniFileHelper
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        public static void IniWriteValue(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetIniValue(string section, string key, string filePath)
        {
            var ret = new StringBuilder(255);
            GetPrivateProfileString(section, key, string.Empty, ret, 255, filePath);
            return ret.ToString();
        }
        public static byte[] GetIniValues(string section, string key, string filePath)
        {
            var temp = new byte[255];
            GetPrivateProfileString(section, key, string.Empty, temp, 255, filePath);
            return temp;
        }

        /// <summary>
        /// 删除ini文件下所有段落
        /// </summary>
        /// <param name="filePath"></param>
        public static void ClearAllSection(string filePath)
        {
            IniWriteValue(null, null, null, filePath);
        }

        /// <summary>
        /// 删除ini文件下personal段落下的所有键
        /// </summary>
        /// <param name="section"></param>
        /// <param name="filePath"></param>
        public static void ClearSection(string section, string filePath)
        {
            IniWriteValue(section, null, null, filePath);
        }

        /// <summary>
        /// 删除ini文件下personal段落下的指定key
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="filePath"></param>
        public static void ClearSectionByKey(string section, string key, string filePath)
        {
            IniWriteValue(section, key, null, filePath);
        }
    }
}
