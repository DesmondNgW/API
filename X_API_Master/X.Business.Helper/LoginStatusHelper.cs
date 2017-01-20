﻿using System;
using X.Business.Entities;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Extend.Cache;

namespace X.Business.Helper
{
    public class LoginStatusHelper
    {
        /// <summary>
        /// 获取登录状态
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <param name="statusZone"></param>
        /// <returns></returns>
        public static LoginStatus GetLoginStatus(string token, string uid, int statusZone)
        {
            var key = ConstHelper.LoginKeyPrefix + token + uid;
            var loginState = RuntimeCache.Get<LoginStatus>(key);
            if (loginState != null) return loginState;
            ExecutionContext<RequestContext>.Current.Update("Zone", statusZone);
            loginState = CouchCache.Default.Get<LoginStatus>(key, DateTime.Now.AddMinutes(ConstHelper.LoginExpireMinutes));
            if (loginState != null) RuntimeCache.Set(key, loginState, DateTime.Now.AddMinutes(ConstHelper.SubLoginExpireMinutes));
            return loginState;
        }

        /// <summary>
        /// 设置登录状态
        /// </summary>
        /// <param name="customerNo"></param>
        /// <param name="customerName"></param>
        /// <param name="zone"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static string SetLoginStatus(string customerNo, string customerName, int zone, string clientId)
        {
            if (string.IsNullOrEmpty(customerNo) || zone.Equals(0)) return string.Empty;
            var uid = TokenHelper.GenerateToken(clientId);
            var token = ExecutionContext<RequestContext>.Current.Token;
            var key = ConstHelper.LoginKeyPrefix + token + uid;
            var statusZone = EnumZoneHelper.GetStatusZone(token, uid);
            var result = new LoginStatus
            {
                Uid = uid,
                CustomerNo = customerNo,
                CustomerName = customerName,
                Zone = zone,
                StatusZone = statusZone,
                Token = token
            };
            ExecutionContext<RequestContext>.Current.Update("Zone", statusZone);
            CouchCache.Default.Set(key, result, DateTime.Now.AddMinutes(ConstHelper.LoginExpireMinutes));
            RuntimeCache.Set(key, result, DateTime.Now.AddMinutes(ConstHelper.SubLoginExpireMinutes));
            ExecutionContext<RequestContext>.Current.Update("Zone", zone);
            return result.Uid;
        }

        /// <summary>
        /// 清除登录状态
        /// </summary>
        /// <param name="uid"></param>
        public static void Clear(string uid)
        {
            var key = ConstHelper.LoginKeyPrefix + uid;
            RuntimeCache.Remove(key);
            CouchCache.Default.Remove(key);
        }
    }
}
