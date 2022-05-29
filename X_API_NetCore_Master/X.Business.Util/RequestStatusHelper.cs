using System;
using System.Reflection;
using System.Threading;
using X.Business.Model;
using X.Util.Core.Log;
using X.Util.Entities;
using X.Util.Entities.Enum;
using X.Util.Extend.Cache;

namespace X.Business.Util
{
    public class RequestStatusHelper
    {
        /// <summary>
        /// 验证请求状态，成功后执行成功回调，失败后执行失败回调
        /// </summary>
        /// <param name="key"></param>
        /// <param name="uri"></param>
        /// <param name="cacheType"></param>
        /// <param name="OnFail"></param>
        /// <param name="OnSuccess"></param>
        public static void VerifyRequestStatus(string key, string uri, EnumCacheType cacheType, Action<RequestStatus> OnFail, Action<RequestStatus> OnSuccess = default)
        {
            if (OnFail == default) OnFail = (request) => { Thread.Sleep(ConstHelper.RequestInterval); };
            if (OnSuccess == default) OnSuccess = (request) => { };
            var requestKey = ConstHelper.RequestStatusPrefix + key + uri;
            var requestStatus = CacheData.Default.GetCacheDbData<RequestStatus>(requestKey, cacheType);
            if (requestStatus == null)
            {
                requestStatus = new RequestStatus
                {
                    Uri = uri,
                    TokenId = key,
                    RequesTime = DateTime.Now
                };
                CacheData.Default.SetCacheDbData(requestKey, requestStatus, DateTime.Now.AddMinutes(ConstHelper.RequestExpireMinutes), cacheType);
                OnSuccess(requestStatus);
            }
            else
            {
                var ts = (DateTime.Now - requestStatus.RequesTime).TotalMilliseconds;
                if (ts < ConstHelper.RequestInterval)
                {
                    OnFail(requestStatus);
                    Logger.Client.Warn(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { key, uri, cacheType }), new Exception("请求调用过于频繁"), LogDomain.Business);
                }
                else
                {
                    OnSuccess(requestStatus);
                }
                requestStatus.RequesTime = DateTime.Now;
                CacheData.Default.SetCacheDbData(requestKey, requestStatus, DateTime.Now.AddMinutes(ConstHelper.RequestExpireMinutes), cacheType);
            }
        }
    }
}
