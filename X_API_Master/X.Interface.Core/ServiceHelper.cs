using System;
using System.Collections.Generic;
using X.Interface.Dto;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Extend.Cache;
using X.Util.Extend.Core;
using X.Util.Extend.Cryption;

namespace X.Interface.Core
{
    /// <summary>
    /// 登录状态
    /// </summary>
    public class LoginStatus
    {
        /// <summary>
        /// 用户标示
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// 用户所属分区
        /// </summary>
        public int Zone { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 用户状态存储的分区
        /// </summary>
        public int StatusZone { get; set; }
    }

    /// <summary>
    /// 分区号
    /// </summary>
    public enum Zone
    {
        //Trade0 = 0,
        Trade1 = 1,
        //Trade2 = 2,
        //Trade3 = 3,
        //Trade4 = 4,
        //Trade5 = 5,
        //Trade6 = 6,
        //Trade7 = 7,
        //Trade8 = 8,
        //Trade9 = 9,
        //Trade10 = 10,
        //Trade11 = 11,
        //Trade12 = 12,
        //Trade13 = 13,
        //Trade14 = 14,
        //Trade15 = 15,
        //Trade16 = 16,
        //Trade17 = 17,
        //Trade18 = 18,
        //Trade19 = 19,
        //Trade110 = 20,
        //Trade21 = 21,
        //Trade22 = 22,
        //Trade23 = 23,
        //Trade24 = 24,
        //Trade25 = 25,
        //Trade26 = 26,
        //Trade27 = 27,
        //Trade28 = 28,
        //Trade29 = 29,
        //Trade30 = 30,
        //Trade31 = 31,
        //Trade32 = 32,
        //Trade33 = 33,
        //Trade34 = 34,
        //Trade35 = 35,
        //Trade36 = 36,
        //Trade37 = 37,
        //Trade38 = 38,
        //Trade39 = 39,
        //Trade40 = 40,
        //Trade41 = 41,
        //Trade42 = 42,
        //Trade43 = 43,
        //Trade44 = 44,
        //Trade45 = 45,
        //Trade46 = 46,
        //Trade47 = 47,
        //Trade48 = 48,
        //Trade49 = 49,
        //Trade50 = 50
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 转换实体
        /// </summary>
        /// <typeparam name="T">UI实体</typeparam>
        /// <typeparam name="TS">数据库实体</typeparam>
        /// <param name="iresult">接口数据</param>
        /// <param name="init">初始化UI实体</param>
        /// <returns></returns>
        public static ApiResult<T> Convert<T, TS>(this CacheResult<TS> iresult, Func<T> init)
        {
            if (Equals(iresult, null)) iresult = new CacheResult<TS> { Succeed = false, Message = CoreBase.CoreCacheMesssage };
            var result = new ApiResult<T>
            {
                Success = iresult.Succeed,
                DebugError = iresult.Message,
                Error = iresult.Message,
                Code = iresult.ErrorCode
            };
            if (CoreBase.CallSuccess(iresult))
            {
                result.Success = true;
                if (init != null) result.Data = init();
            }
            else if (string.IsNullOrEmpty(iresult.Message)) result.DebugError = result.Error = CoreBase.CoreCacheMesssage;
            return result;
        }
    }

    public class ServiceHelper
    {
        #region LoginStatus Api && TokenApi
        private const string Prefix = "X.Interface.Core.ServiceHelper.Prefix";
        private const int Exp = 30;
        private const int SubExp = 10;
        /// <summary>
        /// HMAC key
        /// </summary>
        private const string GenerateHmacKey = "X.Interface.Core.ServiceHelper.GenerateHmacKey";

        private static List<string> Zones
        {
            get
            {
                var zones = new List<string>();
                for (var i = 1; i < Enum.GetValues(typeof(Zone)).Length; i++)
                {
                    zones.Add(i.ToString());
                }
                return zones;
            }
        }

        /// <summary>
        /// LoginStatus Store Zone
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static int GetStatusZone(string token, string uid)
        {
            return CoreUtil.GetConsistentHash(Zones, Prefix + token + uid).Convert2Int32(1);
        }

        /// <summary>
        ///  token Store Zone
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int GetTokenZone(string token)
        {
            return CoreUtil.GetConsistentHash(Zones, Prefix + token).Convert2Int32(1);
        }

        /// <summary>
        /// 获取登录状态
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <param name="statusZone"></param>
        /// <returns></returns>
        public static LoginStatus GetLoginStatus(string token, string uid, int statusZone)
        {
            var key = Prefix + token + uid;
            var loginState = RuntimeCache.Get<LoginStatus>(key);
            if (loginState != null) return loginState;
            ExecutionContext<RequestContext>.Current.Update("Zone", statusZone);
            loginState = CouchCache.Default.Get<LoginStatus>(key, DateTime.Now.AddMinutes(Exp));
            if (loginState != null) RuntimeCache.Set(key, loginState, DateTime.Now.AddMinutes(SubExp));
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
            var uid = GenerateToken(clientId);
            var token = ExecutionContext<RequestContext>.Current.Token;
            var key = Prefix + token + uid;
            var statusZone = GetStatusZone(token, uid);
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
            CouchCache.Default.Set(key, result, DateTime.Now.AddMinutes(Exp));
            RuntimeCache.Set(key, result, DateTime.Now.AddMinutes(SubExp));
            ExecutionContext<RequestContext>.Current.Update("Zone", zone);
            return result.Uid;
        }

        /// <summary>
        /// 清除登录状态
        /// </summary>
        /// <param name="uid"></param>
        public static void Clear(string uid)
        {
            var key = Prefix + uid;
            RuntimeCache.Remove(key);
            CouchCache.Default.Remove(key);
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <returns></returns>
        public static string GenerateToken(string clientId)
        {
            var token = BaseCryption.SignData(clientId, Guid.NewGuid().ToString("N"), HmacType.Md5);
            var key = Prefix + token + clientId;
            ExecutionContext<RequestContext>.Current.Update("Zone", GetTokenZone(token));
            CouchCache.Default.Set(key, key, DateTime.Now.AddMinutes(Exp));
            return token;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool VerifyToken(string clientId, string token)
        {
            if (!BaseCryption.VerifyData(GenerateHmacKey, token, HmacType.Md5)) return false;
            var key = Prefix + token + clientId;
            ExecutionContext<RequestContext>.Current.Update("Zone", GetTokenZone(token));
            var obj = CouchCache.Default.Get<string>(key);
            return key.Equals(obj);
        }
        #endregion
    }
}
