using System;
using System.Threading;
using X.Business.Model;
using X.Business.Util;
using X.DB.Core;
using X.DB.Entitis;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enum;
using X.Util.Extend.Cache;
using X.Util.Extend.Core;
using X.Util.Extend.Cryption;
using X.Util.Provider;

namespace X.Business.Core
{
    public class UserService
    {
        public const EnumCacheType CacheType = EnumCacheType.RedisBoth;

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //[CacheContext(EnumCacheExpireType.Absolute, EnumCacheType.Runtime, EnumCacheTimeLevel.Minute, 30, false, true)]//设置缓存
        public static ResultInfo<User> Login(string accountNo, string password)
        {
            var result = new ResultInfo<User>();
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            var iresult = CoreAccess<IUserInfoManager>.TryCall(provider.Client.Login, accountNo, password, provider, 
                new LogOptions<ResultInfo<UserInfo>>(CoreBase.CallSuccess));
            if (CoreBase.CallSuccess(iresult))
            {
                var token = ExecutionContext<BusinessRequestContext>.Current.Token;
                var customerNo = Guid.NewGuid().ToString("N");
                var data = iresult.Result;
                var user = new User()
                {
                    UserInfo = data,
                    CustomerNo = data.CustomerNo,
                    Token = token
                };
                user.UToken = BaseCryption.SignData(token, Guid.NewGuid().ToString("N"), HmacType.Md5);
                result.Result = user;
                result.Succeed = true;
                var key = ConstHelper.LoginKeyPrefix + user.UToken;
                CacheData.Default.SetCacheDbData(key, user, TimeSpan.FromMinutes(ConstHelper.LoginExpireMinutes), CacheType);
            }
            else
            {
                result.Message = iresult.Message;
            }
            return result;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="utoken"></param>
        /// <param name="uri"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void VerifyToken(string utoken, string uri)
        {
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, utoken, HmacType.Md5)) throw new InvalidOperationException("utoken错误或过期");
            var key = ConstHelper.LoginKeyPrefix + utoken;
            var obj = CacheData.Default.GetCacheDbData<User>(key, CacheType);
            if (obj == null) throw new InvalidOperationException("utoken错误或过期");
            var requestKey = ConstHelper.LoginKeyPrefix + utoken + uri;
            var requestStatus = CacheData.Default.GetCacheDbData<RequestStatus>(requestKey, CacheType);
            if (requestStatus == null)
            {
                requestStatus = new RequestStatus
                {
                    Uri = uri,
                    TokenId = utoken,
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
