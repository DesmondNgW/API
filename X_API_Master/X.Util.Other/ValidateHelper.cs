﻿using System.Text.RegularExpressions;

namespace X.Util.Other
{
    /// <summary>
    /// 正则验证格式
    /// </summary>
    public class ValidateHelper
    {
        private const string RegNumber = @"^\\d+$";
        private const string RegDecimal = @"^\\d+[.]?\\d+$";
        private const string RegEmail = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
        private const string RegChzn = @"[\u4e00-\u9fa5]";
        private const string RegPhone = @"^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}$";

        /// <summary>
        /// 是否数字字符串
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns>返回值</returns>
        public static bool IsNumber(string inputData)
        {
            return Regex.IsMatch(inputData, RegNumber);
        }

        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns>返回值</returns>
        public static bool IsDecimal(string inputData)
        {
            return Regex.IsMatch(inputData, RegDecimal);
        }

        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns>返回值</returns>
        public static bool IsHasChzn(string inputData)
        {
            return Regex.IsMatch(inputData, RegChzn);
        }

        /// <summary>
        /// 是否是邮件地址
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns>返回值</returns>
        public static bool IsEmail(string inputData)
        {
            return Regex.IsMatch(inputData, RegEmail);
        }

        /// <summary>
        /// 邮编有效性
        /// </summary>
        /// <param name="postCode"></param>
        /// <returns></returns>
        public static bool IsPostCode(string postCode)
        {
            return Regex.IsMatch(postCode, @"^\d{6}$");
        }

        /// <summary>
        /// 固定电话有效性
        /// </summary>
        /// <param name="phone">输入字符串</param>
        /// <returns>返回值</returns>
        public static bool IsPhone(string phone)
        {
            return Regex.IsMatch(phone, RegPhone, RegexOptions.None);
        }
    }
}
