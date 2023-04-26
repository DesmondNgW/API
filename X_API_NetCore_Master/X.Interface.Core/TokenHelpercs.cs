using System;
using System.Threading;
using X.Business.Core;
using X.Business.Model;
using X.Business.Util;
using X.Interface.Dto;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enum;
using X.Util.Extend.Cache;
using X.Util.Extend.Cryption;

namespace X.Interface.Core
{
    public class TokenHelper
    {
        public const EnumCacheType CacheType = EnumCacheType.RedisBoth;

        /// <summary>
        /// 生成token
        /// </summary>
        /// <returns></returns>
        public static string GenerateToken(string clientId, string ip, string userAgent)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(userAgent)) throw new ArgumentException("clientId|ip|userAgent");
            var token = new Token
            {
                ClientId = clientId,
                ClientIp = ip,
                UserAgent = userAgent
            };
            token.TokenId = BaseCryption.SignData(token.ClientId + token.ClientIp + token.UserAgent, Guid.NewGuid().ToString("N"), HmacType.Md5);
            var key = ConstHelper.LoginKeyPrefix + token.TokenId;
            CacheData.Default.SetCacheDbData(key, token, TimeSpan.FromMinutes(ConstHelper.LoginExpireMinutes), CacheType);
            return token.TokenId;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// /// <param name="clientId"></param>
        /// <param name="clientIp"></param>
        /// <param name="userAgent"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static void VerifyToken(string token, string clientId, string clientIp, string userAgent, string uri)
        {
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("token不能为空");
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, token, HmacType.Md5)) throw new InvalidOperationException("token错误或过期");
            var key = ConstHelper.LoginKeyPrefix + token;
            var obj = CacheData.Default.GetCacheDbData<Token>(key, CacheType) ?? throw new InvalidOperationException("token错误或过期");
            if (obj.ClientId != clientId)
            {
                CacheData.Default.Remove(key, CacheType);
                Logger.Client.Warn(string.Format("token: {0}, clientId: {1}|{2}不一致", token, obj.ClientId, clientId), LogDomain.Ui);
                throw new InvalidOperationException("token错误或过期");
            }
            if (obj.UserAgent != userAgent)
            {
                CacheData.Default.Remove(key, CacheType);
                Logger.Client.Warn(string.Format("token: {0}, userAgent: {1}|{2}不一致", token, obj.UserAgent, userAgent), LogDomain.Ui);
                throw new InvalidOperationException("token错误或过期");
            }
            if (obj.ClientIp != clientIp)
            {
                CacheData.Default.Remove(key, CacheType);
                Logger.Client.Warn(string.Format("token: {0}, clientIp: {1}|{2}不一致", token, obj.ClientIp, clientIp), LogDomain.Ui);
                throw new InvalidOperationException("token错误或过期");
            }
            RequestStatusHelper.VerifyRequestStatus(token, uri, CacheType, (request) => { Thread.Sleep(ConstHelper.RequestInterval); });
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <param name="utoken"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static User VerifyUToken(string utoken, string uri)
        {
            return UserService.VerifyToken(utoken, uri);
        }
    }
}
