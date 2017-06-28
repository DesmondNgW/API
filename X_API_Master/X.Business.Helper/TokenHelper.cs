using System;
using System.Threading;
using X.Business.Entities;
using X.Util.Core;
using X.Util.Core.Kernel;
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
        public static string GenerateToken(string clientId)
        {
            var token = new Token
            {
                ClientId = clientId,
                ClientIp = IpBase.GetIp(),
            };
            token.TokenId = BaseCryption.SignData(token.ClientId + token.ClientIp, Guid.NewGuid().ToString("N"), HmacType.Md5);
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
        /// <param name="uri"></param>
        /// <returns></returns>
        public static void VerifyToken(string token, string clientId, Uri uri)
        {
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, token, HmacType.Md5)) throw new InvalidOperationException("token错误或过期");
            var key = ConstHelper.LoginKeyPrefix + token;
            ExecutionContext<RequestContext>.Current.Update("Zone", EnumZoneHelper.GetTokenZone(token));
            var obj = CacheData.Default.GetCacheDbData<Token>(key, CacheType);
            if (obj == null) throw new InvalidOperationException("token错误或过期");
            if (obj.ClientId != clientId)
            {
                CacheData.Default.Remove(key, CacheType);
                throw new InvalidOperationException("token错误或过期");
            }
            if (obj.ClientIp != IpBase.GetIp())
            {
                CacheData.Default.Remove(key, CacheType);
                throw new Exception("请求IP非同一IP");
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
