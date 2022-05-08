using System;
using System.Text.RegularExpressions;
using X.DB.Entitis;
using X.DB.Util;
using X.Util.Entities;
using X.Util.Extend.Cryption;
using X.Util.Other;

namespace X.DB.Core
{
    public class UserInfoHelper
    {
        private const string RegNumber = "(?=.*[0-9]).{8,30}";
        private const string RegUpper = "(?=.*[A-Z]).{8,30}";
        private const string RegLower = "(?=.*[a-z]).{8,30}";
        private const string RegSpecial = "(?=.*[^a-zA-Z0-9]).{8,30}";

        /// <summary>
        /// 密码强度
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool PasswordStrong(string password)
        {
            var strong = 0;
            if (Regex.IsMatch(password, RegNumber))
            {
                strong++;
            }
            if (Regex.IsMatch(password, RegUpper))
            {
                strong++;
            }
            if (Regex.IsMatch(password, RegLower))
            {
                strong++;
            }
            if (Regex.IsMatch(password, RegSpecial))
            {
                strong++;
            }
            return strong > 2;
        }

        /// <summary>
        /// GetUserInfo
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <param name="Password"></param>
        /// <param name="Telephone"></param>
        /// <param name="Email"></param>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public static ResultInfo<UserInfo> GetUserInfo(string AccountNo, string Password, string Telephone, string Email, string CustomerName)
        {
            var result = new ResultInfo<UserInfo>() { };
            if (!PasswordStrong(Password))
            {
                result.Message = ConstHelper.UserManagerCode2;
                return result;
            }
            if (!ValidateHelper.IsPhone(Telephone))
            {
                result.Message = ConstHelper.UserManagerCode3;
                return result;
            }
            if (!ValidateHelper.IsEmail(Email))
            {
                result.Message = ConstHelper.UserManagerCode4;
                return result;
            }
            var customerNo = Guid.NewGuid().ToString("N");
            result.Result = new UserInfo()
            {
                Id = customerNo,
                CustomerNo = customerNo,
                CustomerName = CustomerName,
                AccountNo = AccountNo,
                Password = BaseCryption.Sha1(Password),
                Telephone = Telephone,
                Email = Email,
                IsEnable = true,
                BindEmail = false,
                BindTelephone = false
            };
            result.Succeed = true;
            return result;

        }
    }
}
