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

        #region GetUserInfo
        /// <summary>
        /// GetUserInfo
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public static ResultInfo<UserInfo> GetUserInfo(string customerNo)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.GetUserInfo, customerNo, provider,
                new LogOptions<ResultInfo<UserInfo>>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// GetUserInfoByAccountNo
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <returns></returns>
        public static ResultInfo<UserInfo> GetUserInfoByAccountNo(string AccountNo)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.GetUserInfoByAccountNo, AccountNo, provider,
                new LogOptions<ResultInfo<UserInfo>>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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
        #endregion

        #region Register
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ResultInfo<bool> Register(UserInfo user)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.Register, user, provider,
                new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <param name="Password"></param>
        /// <param name="Telephone"></param>
        /// <param name="Email"></param>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public static ResultInfo<bool> Register(string AccountNo, string Password, string Telephone, string Email, string CustomerName)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.Register, AccountNo, Password, Telephone, Email,
                CustomerName, provider, new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }
        #endregion

        #region Bind
        /// <summary>
        /// BindEmail
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <returns></returns>
        public static ResultInfo<bool> BindEmail(string CustomerNo)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.BindEmail, CustomerNo, provider,
                new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// BindTelephone
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <returns></returns>
        public static ResultInfo<bool> BindTelephone(string CustomerNo)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.BindTelephone, CustomerNo, provider,
                new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }
        #endregion

        #region Password
        /// <summary>
        /// ResetPassword
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static ResultInfo<bool> ResetPassword(string CustomerNo, string oldPassword, string newPassword)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.ResetPassword, CustomerNo, oldPassword, newPassword,
                provider, new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// ForgetPassword
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <param name="Telephone"></param>
        /// <param name="Email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static ResultInfo<bool> ForgetPassword(string AccountNo, string Telephone, string Email, string newPassword)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.ForgetPassword, AccountNo, Telephone, Email, newPassword,
                provider, new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }
        #endregion

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ResultInfo<bool> Edit(string CustomerNo, UserInfo user)
        {
            var provider = new InstanceProvider<UserInfoManager>(LogDomain.Db);
            return CoreAccess<IUserInfoManager>.TryCall(provider.Client.Edit, CustomerNo, user, provider,
                new LogOptions<ResultInfo<bool>>(CoreBase.CallSuccess));
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="utoken"></param>
        /// <param name="uri"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static User VerifyToken(string utoken, string uri)
        {
            if (string.IsNullOrEmpty(utoken)) throw new InvalidOperationException("utoken不能为空");
            if (!BaseCryption.VerifyData(ConstHelper.GenerateHmacKey, utoken, HmacType.Md5)) throw new InvalidOperationException("utoken错误或过期");
            var key = ConstHelper.LoginKeyPrefix + utoken;
            var obj = CacheData.Default.GetCacheDbData<User>(key, CacheType);
            if (obj == null) throw new InvalidOperationException("utoken错误或过期");
            RequestStatusHelper.VerifyRequestStatus(utoken, uri, CacheType, (request) => { Thread.Sleep(ConstHelper.RequestInterval); });
            return obj;
        }
    }
}
