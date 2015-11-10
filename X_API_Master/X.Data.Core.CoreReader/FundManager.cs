using Em.Entities;
using System.Collections.Generic;
using System.Reflection;
using X.Data.Core.CoreReader.Services;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;
using System.Linq;

namespace X.Data.Core.CoreReader
{
    public class FundManager
    {
        private const LogDomain EDomain = LogDomain.Core;

        public ResultInfo<List<FundDays>> GetFundDays()
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IFundManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFundDays, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Hour, 6);
        }

        public ResultInfo<FundDays> GetFundDay(string fundCode)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var result = new ResultInfo<FundDays> { Succeed = false, Message = "fundcoe not exists" };
                var list = GetFundDays();
                if (list?.Result != null && list.Result.Count > 0)
                {
                    var funddays = list.Result.FirstOrDefault(p => Equals(p.FundCode, fundCode));
                    if (funddays != null) result = new ResultInfo<FundDays> { Succeed = true, Result = funddays, Message = string.Empty };
                }
                return result;
            }, MethodBase.GetCurrentMethod(), new object[] { fundCode }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }

        public ResultInfo<List<Fund>> GetFunds()
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IFundManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFunds, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Hour, 6);
        }

        public ResultInfo<Fund> GetFund(string fundCode)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IFundManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFund, fundCode, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { fundCode }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }

        public ResultInfo<FundNav> GetFundNav(string fundCode)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IFundManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFundNav, fundCode, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { fundCode }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }

        public ResultInfo<IList<FundCompany>> GetFundCompanies()
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IFundManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFundCompanies, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Hour, 6);
        }

        public ResultInfo<List<Fund>> GetFixedBagGroups(int financialRound)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IFundManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFixedBagGroups, financialRound, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { financialRound }, AppConfig.FundCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }
    }
}
