﻿using System;
using X.Business.Entities;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Extend.Cache;
using X.Util.Extend.Cryption;

namespace X.Business.Helper
{
    public class LoginStatusHelper
    {
        public const EnumCacheType CacheType = EnumCacheType.MemBoth;
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
            loginState = CacheData.Default.GetCacheDbData<LoginStatus>(key, CacheType, DateTime.Now.AddMinutes(ConstHelper.SubLoginExpireMinutes));
            return loginState;
        }

        /// <summary>
        /// 设置登录状态
        /// </summary>
        /// <param name="customerNo"></param>
        /// <param name="customerName"></param>
        /// <param name="zone"></param>
        /// <param name="clientId"></param>
        /// <param name="ip"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string SetLoginStatus(string customerNo, string customerName, int zone, string clientId, string ip, string userAgent)
        {
            if (string.IsNullOrEmpty(customerNo) || zone.Equals(0)) return string.Empty;
            var uid = TokenHelper.GenerateToken(clientId, ip, userAgent);
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
            CacheData.Default.SetCacheDbData(key, result, DateTime.Now.AddMinutes(ConstHelper.LoginExpireMinutes), CacheType);
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
            CacheData.Default.Remove(key, CacheType);
        }

        /// <summary>
        /// 用户验证
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <param name="clientId"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static LoginStatus UserIdentity(string token, string uid, string clientId, Uri uri)
        {
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, uid, HmacType.Md5)) throw new InvalidOperationException("uid错误或过期");
            var statusZone = EnumZoneHelper.GetStatusZone(token, uid);
            var loginState = GetLoginStatus(token, uid, statusZone);
            if (Equals(loginState, null) || loginState.StatusZone != statusZone || loginState.Token != token || loginState.Uid != uid) throw new InvalidOperationException("token过期或与uid不匹配");
            return loginState;
        }
    }
}
