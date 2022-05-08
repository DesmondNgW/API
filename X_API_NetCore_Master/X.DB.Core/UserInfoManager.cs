using MongoDB.Driver;
using X.DB.Entitis;
using X.DB.Util;
using X.Util.Entities;
using X.Util.Extend.Core;
using X.Util.Extend.Cryption;
using X.Util.Extend.Mongo;

namespace X.DB.Core
{
    public class UserInfoManager : IUserInfoManager
    {
        public const string DataBase = "UserInfo";
        public const string Collection = "UserInfo";

        #region GetUserInfo
        /// <summary>
        /// GetUserInfo
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <returns></returns>
        public ResultInfo<UserInfo> GetUserInfo(string CustomerNo)
        {
            var result = new ResultInfo<UserInfo>() { };
            var Filter = Builders<UserInfo>.Filter;
            var filter = Filter.Eq("_id", CustomerNo) & Filter.Eq("IsEnable", true);
            var list = MongoDbBase<UserInfo>.Default.Find(DataBase, Collection, filter);
            if (list != null)
            {
                result.Succeed = true;
                result.Result = list.FirstOrDefault();
            }
            else
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            return result;
        }

        /// <summary>
        /// GetUserInfoByAccountNo
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <returns></returns>
        public ResultInfo<UserInfo> GetUserInfoByAccountNo(string AccountNo)
        {
            var result = new ResultInfo<UserInfo>() { };
            var Filter = Builders<UserInfo>.Filter;
            var filter = Filter.Eq("AccountNo", AccountNo) & Filter.Eq("IsEnable", true);
            var list = MongoDbBase<UserInfo>.Default.Find(DataBase, Collection, filter);
            if (list != null)
            {
                result.Succeed = true;
                result.Result = list.FirstOrDefault();
            }
            else
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            return result;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResultInfo<UserInfo> Login(string accountNo, string password)
        {
            var result = new ResultInfo<UserInfo>() { };
            var pwd = BaseCryption.Sha1(password);
            var Filter = Builders<UserInfo>.Filter;
            var accountFilter = Filter.Eq("AccountNo", accountNo);
            var telephoneFilter = Filter.Eq("Telephone", accountNo) & Filter.Eq("BindTelephone", true);
            var emailFilter = Filter.Eq("Email", accountNo) & Filter.Eq("BindEmail", true);
            var filter = Filter.Eq("Password", pwd) & Filter.Eq("IsEnable", true) & (accountFilter | telephoneFilter | emailFilter);
            var list = MongoDbBase<UserInfo>.Default.Find(DataBase, Collection, filter);
            if (list != null)
            {
                result.Succeed = true;
                result.Result = list.FirstOrDefault();
            }
            else
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            return result;
        }
        #endregion

        #region Insert
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultInfo<bool> Register(UserInfo user)
        {
            var result = new ResultInfo<bool>() { };
            var iresult = GetUserInfoByAccountNo(user.AccountNo);
            if (CoreBase.CallSuccess(iresult))
            {
                result.Message = ConstHelper.UserManagerCode1;
            }
            else
            {
                var Filter = Builders<UserInfo>.Filter;
                var filter = Filter.Eq("_id", user.Id);
                MongoDbBase<UserInfo>.Default.ReplaceMongo(DataBase, Collection, filter, user);
                result.Succeed = true;
                result.Result = true;
            }
            return result;
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
        public ResultInfo<bool> Register(string AccountNo, string Password, string Telephone, string Email, string CustomerName)
        {
            var result = new ResultInfo<bool>() { };
            var iresult = UserInfoHelper.GetUserInfo(AccountNo, Password, Telephone, Email, CustomerName);
            if (!CoreBase.CallSuccess(iresult))
            {
                return Register(iresult.Result);
            }
            else
            {
                result.Message = iresult.Message;
            }
            return result;
        }
        #endregion

        #region Bind
        /// <summary>
        /// BindEmail
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <returns></returns>
        public ResultInfo<bool> BindEmail(string CustomerNo)
        {
            var result = new ResultInfo<bool>() { };
            var Filter = Builders<UserInfo>.Filter;
            var filter = Filter.Eq("_id", CustomerNo) & Filter.Eq("IsEnable", true) & Filter.Eq("BindEmail", false);
            var iresult = GetUserInfo(CustomerNo);
            if (!CoreBase.CallSuccess(iresult))
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            else
            {
                if (!iresult.Result.BindEmail)
                {
                    var update = Builders<UserInfo>.Update.Set(p => p.BindEmail, false);
                    MongoDbBase<UserInfo>.Default.UpdateMongo(DataBase, Collection, filter, update);
                    result.Succeed = true;
                    result.Result = true;
                }
                else
                {
                    result.Message = ConstHelper.UserManagerCode5;
                }
            }
            return result;
        }

        /// <summary>
        /// BindTelephone
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <returns></returns>
        public ResultInfo<bool> BindTelephone(string CustomerNo)
        {
            var result = new ResultInfo<bool>() { };
            var Filter = Builders<UserInfo>.Filter;
            var filter = Filter.Eq("_id", CustomerNo) & Filter.Eq("IsEnable", true) & Filter.Eq("BindTelephone", false);
            var iresult = GetUserInfo(CustomerNo);
            if (!CoreBase.CallSuccess(iresult))
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            else
            {
                if (!iresult.Result.BindTelephone)
                {
                    var update = Builders<UserInfo>.Update.Set(p => p.BindTelephone, false);
                    MongoDbBase<UserInfo>.Default.UpdateMongo(DataBase, Collection, filter, update);
                    result.Succeed = true;
                    result.Result = true;
                }
                else
                {
                    result.Message = ConstHelper.UserManagerCode6;
                }
            }
            return result;
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
        public ResultInfo<bool> ResetPassword(string CustomerNo, string oldPassword, string newPassword)
        {
            var result = new ResultInfo<bool>() { };
            var Filter = Builders<UserInfo>.Filter;
            var pwd = BaseCryption.Sha1(oldPassword);
            var filter = Filter.Eq("_id", CustomerNo) & Filter.Eq("IsEnable", true) & Filter.Eq("Password", pwd);
            var iresult = GetUserInfo(CustomerNo);
            if (!CoreBase.CallSuccess(iresult))
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            else
            {
                var update = Builders<UserInfo>.Update.Set(p => p.Password, BaseCryption.Sha1(newPassword));
                MongoDbBase<UserInfo>.Default.UpdateMongo(DataBase, Collection, filter, update);
                result.Succeed = true;
                result.Result = true;
            }
            return result;
        }

        /// <summary>
        /// ForgetPassword
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <param name="Telephone"></param>
        /// <param name="Email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public ResultInfo<bool> ForgetPassword(string AccountNo, string Telephone, string Email, string newPassword)
        {
            var result = new ResultInfo<bool>() { };
            var Filter = Builders<UserInfo>.Filter;
            var pwd = BaseCryption.Sha1(newPassword);
            var filter = Filter.Eq("AccountNo", AccountNo) & Filter.Eq("Telephone", Telephone) & Filter.Eq("Email", Email) & Filter.Eq("IsEnable", true);
            var iresult = GetUserInfoByAccountNo(AccountNo);
            if (!CoreBase.CallSuccess(iresult))
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            else
            {
                var update = Builders<UserInfo>.Update.Set(p => p.Password, BaseCryption.Sha1(newPassword));
                MongoDbBase<UserInfo>.Default.UpdateMongo(DataBase, Collection, filter, update);
                result.Succeed = true;
                result.Result = true;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="CustomerNo"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public ResultInfo<bool> Edit(string CustomerNo, UserInfo user)
        {
            var result = new ResultInfo<bool>() { };
            var Filter = Builders<UserInfo>.Filter;
            var filter = Filter.Eq("_id", CustomerNo) & Filter.Eq("IsEnable", true);
            var iresult = GetUserInfo(CustomerNo);
            if (!CoreBase.CallSuccess(iresult))
            {
                result.Message = ConstHelper.UserManagerCode0;
            }
            else
            {
                var data = iresult.Result;
                data.Telephone = user.Telephone;
                data.CustomerName = user.CustomerName;
                data.Email = data.Email;
                MongoDbBase<UserInfo>.Default.ReplaceMongo(DataBase, Collection, filter, data);
                result.Succeed = true;
                result.Result = true;
            }
            return result;
        }
    }
}
