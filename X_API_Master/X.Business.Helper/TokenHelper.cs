using System;
using System.Threading;
using X.Business.Entities;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Extend.Cache;
using X.Util.Extend.Cryption;

namespace X.Business.Helper
{
    public class TokenHelper
    {
        public const EnumCacheType CacheType = EnumCacheType.MemBoth;

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
            ExecutionContext<RequestContext>.Current.Update("Zone", EnumZoneHelper.GetTokenZone(token.TokenId));
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
        public static void VerifyToken(string token, string clientId, string clientIp, string userAgent, Uri uri)
        {
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, token, HmacType.Md5)) throw new InvalidOperationException("token错误或过期");
            var key = ConstHelper.LoginKeyPrefix + token;
            ExecutionContext<RequestContext>.Current.Update("Zone", EnumZoneHelper.GetTokenZone(token));
            var obj = CacheData.Default.GetCacheDbData<Token>(key, CacheType);
            if (obj == null) throw new InvalidOperationException("token错误或过期");
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
            var requestKey = ConstHelper.LoginKeyPrefix + token + uri;
            var requestStatus = CacheData.Default.GetCacheDbData<RequestStatus>(requestKey, CacheType);
            if (requestStatus == null)
            {
                requestStatus = new RequestStatus
                {
                    Uri = uri.ToString(),
                    TokenId = token,
                    RequesTime = DateTime.Now
                };
                CacheData.Default.SetCacheDbData(requestKey, requestStatus, DateTime.Now.AddMinutes(ConstHelper.RequestExpireMinutes), CacheType);
            }
            else
            {
                var ts = (DateTime.Now - requestStatus.RequesTime).TotalMilliseconds;
                if (ts < ConstHelper.RequestInterval)
                {
                    Thread.Sleep(ConstHelper.RequestInterval);
                }
                requestStatus.RequesTime = DateTime.Now;
                CacheData.Default.SetCacheDbData(requestKey, requestStatus, DateTime.Now.AddMinutes(ConstHelper.RequestExpireMinutes), CacheType);
            }
        }
    }
}
