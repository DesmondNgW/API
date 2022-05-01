using System;
using X.Business.Helper;

namespace X.Interface.Core.Helper
{
    /// <summary>
    /// 登录状态
    /// </summary>
    public struct LoginStatus
    {
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
    }


    public class LoginServiceHelper
    {
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clientId"></param>
        /// <param name="userAgent"></param>
        /// <param name="uri"></param>
        /// <param name="clientIp"></param>
        public static void VerifyToken(string token, string clientId, string clientIp, string userAgent, Uri uri)
        {
            TokenHelper.VerifyToken(token, clientId, clientIp, userAgent, uri);
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
            return LoginStatusHelper.SetLoginStatus(customerNo, customerName, zone, clientId, ip, userAgent);
        }

        /// <summary>
        /// 清除登录状态
        /// </summary>
        /// <param name="uid"></param>
        public static void Clear(string uid)
        {
            LoginStatusHelper.Clear(uid);
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
            var status = LoginStatusHelper.UserIdentity(token, uid, clientId, uri);
            return new LoginStatus
            {
                CustomerName = status.CustomerName,
                CustomerNo = status.CustomerNo,
                Zone = status.Zone
            };
        }
    }
}
