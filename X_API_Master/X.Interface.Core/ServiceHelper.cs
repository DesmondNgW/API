using System;
using System.Collections.Generic;
using X.Interface.Dto;
using X.Util.Core;
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
        /// StatusZone
        /// </summary>
        public int StatusZone { get; set; }
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
                for (var i = 1; i < Enum.GetValues(typeof (Em.FundTrade.Route.Entities.Zone)).Length; i++)
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
            return CoreParse.GetInt32(CoreUtil.GetConsistentHash(Zones, Prefix + token + uid), 1);
        }

        /// <summary>
        ///  token Store Zone
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int GetTokenZone(string token)
        {
            return CoreParse.GetInt32(CoreUtil.GetConsistentHash(Zones, Prefix + token), 1);
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
            ExecutionContext<RequestContext>.Update("Zone", statusZone);
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
        /// <returns></returns>
        public static string SetLoginStatus(string customerNo, string customerName, int zone)
        {
            if (string.IsNullOrEmpty(customerNo) || zone.Equals(0)) return string.Empty;
            var uid = Guid.NewGuid().ToString("N");
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
            ExecutionContext<RequestContext>.Update("Zone", statusZone);
            CouchCache.Default.Set(key, result, DateTime.Now.AddMinutes(Exp));
            RuntimeCache.Set(key, result, DateTime.Now.AddMinutes(SubExp));
            ExecutionContext<RequestContext>.Update("Zone", zone);
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
        public static string GenerateToken()
        {
            var token = BaseCryption.SignData(GenerateHmacKey, Guid.NewGuid().ToString("N"), HmacType.Md5);
            var key = Prefix + token;
            ExecutionContext<RequestContext>.Update("Zone", GetTokenZone(token));
            CouchCache.Default.Set(key, key, DateTime.Now.AddMinutes(Exp));
            return token;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool VerifyToken(string token)
        {
            if (!BaseCryption.VerifyData(GenerateHmacKey, token, HmacType.Md5)) return false;
            var key = Prefix + token;
            ExecutionContext<RequestContext>.Update("Zone", GetTokenZone(token));
            var obj = CouchCache.Default.Get<string>(key);
            return key.Equals(obj);
        }
        #endregion

        /// <summary>
        /// 转换实体
        /// </summary>
        /// <typeparam name="T">UI实体</typeparam>
        /// <typeparam name="TS">数据库实体</typeparam>
        /// <param name="iresult">接口数据</param>
        /// <param name="init">初始化UI实体</param>
        /// <returns></returns>
        public static ApiResult<T> Convert<T, TS>(Em.Entities.ResultInfo<TS> iresult, Func<T> init)
        {
            if (Equals(iresult, null)) iresult = new Em.Entities.ResultInfo<TS> { Succeed = false, Message = CoreBase.CoreDefaultMesssage };
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
            else if (string.IsNullOrEmpty(iresult.Message)) result.DebugError = result.Error = CoreBase.CoreDefaultMesssage;
            return result;
        }
    }
}
