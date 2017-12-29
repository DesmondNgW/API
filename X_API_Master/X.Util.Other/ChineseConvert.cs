using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.International.Converters.PinYinConverter;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.Util.Other
{
    /// <summary>
    /// 常用转换类
    /// </summary>
    public class ChineseConvert
    {
        /// <summary>
        /// 取拼音第一个字段
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static char GetFirst(char ch, int index)
        {
            return Get(ch, index)[0];
        }

        /// <summary>
        /// 取拼音第一个字段
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> GetFirst(string str)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(str)) return ret;
            ret.AddRange(str.ToCharArray().Select(t => GetFirst(t, 0).ToString()));
            return ret;
        }

        /// <summary>
        /// 获取单字拼音
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static List<string> Get(char ch)
        {
            try
            {
                var chineseChar = new ChineseChar(ch);
                return (from item in chineseChar.Pinyins where item != null select item.Substring(0, item.Length - 1)).ToList();
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { ch }), e, LogDomain.Util);
            }
            return null;
        }

        /// <summary>
        /// 获取单字拼音,出现多音字的处理方式
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string Get(char ch, int index)
        {
            var list = Get(ch);
            if (list == null) return string.Empty;
            if (index >= list.Count || index < 0) index = 0;
            return list[index];
        }

        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static List<string> Get(string str)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(str)) return ret;
            ret.AddRange(str.ToCharArray().Select(t => Get(t, 0)));
            return ret;
        }
    }
}
