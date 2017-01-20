using System;
using X.Business.Entities;
using X.Util.Core;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Extend.Cache;
using X.Util.Extend.Cryption;

namespace X.Business.Helper
{
    public class TokenHelper
    {
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
                RequesTime = DateTime.Now,
                AllowAccess = true
            };
            token.TokenId = BaseCryption.SignData(token.ClientId + token.ClientIp, Guid.NewGuid().ToString("N"), HmacType.Md5);
            var key = ConstHelper.LoginKeyPrefix + token.TokenId;
            ExecutionContext<RequestContext>.Current.Update("Zone", EnumZoneHelper.GetTokenZone(token.TokenId));
            CouchCache.Default.Set(key, token, DateTime.Now.AddMinutes(ConstHelper.LoginExpireMinutes));
            return token.TokenId;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// /// <param name="clientId"></param>
        /// <returns></returns>
        public static Exception VerifyToken(string token, string clientId)
        {
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, token, HmacType.Md5)) return new InvalidOperationException("token错误或过期");
            var key = ConstHelper.LoginKeyPrefix + token;
            ExecutionContext<RequestContext>.Current.Update("Zone", EnumZoneHelper.GetTokenZone(token));
            var obj = CouchCache.Default.Get<Token>(key);
            if (obj == null || obj.ClientId != clientId) return new InvalidOperationException("token错误或过期");
            if (obj.ClientIp != IpBase.GetIp()) return new Exception("token错误或过期");
            obj.LastRequesTime = obj.RequesTime;
            obj.RequesTime = DateTime.Now;
            obj.AllowAccess = true;
            CouchCache.Default.Set(key, obj, DateTime.Now.AddMinutes(ConstHelper.LoginExpireMinutes));
            return null;
        }
    }
}
