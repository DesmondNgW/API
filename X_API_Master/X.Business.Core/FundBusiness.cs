using Em.Entities;
using System.Collections.Generic;
using X.Data.Core.CoreReader;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Business.Core
{
    public class FundBusiness
    {
        private const LogDomain EDomain = LogDomain.Cache;
        /// <summary>
        /// 获取指定基金的基金对象
        /// </summary>
        /// <param name="fundCode"></param>
        /// <returns></returns>
        public ResultInfo<Fund> GetFund(string fundCode)
        {
            var provider = new InstanceProvider<FundManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetFund, fundCode, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取指定基金的基金净值
        /// </summary>
        /// <param name="fundCode"></param>
        /// <returns></returns>
        public ResultInfo<FundNav> GetFundNav(string fundCode)
        {
            var provider = new InstanceProvider<FundManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetFundNav, fundCode, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取所有基金列表
        /// </summary>
        /// <returns></returns>
        public ResultInfo<List<Fund>> GetFunds()
        {
            var provider = new InstanceProvider<FundManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetFunds, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取所有基金公司列表
        /// </summary>
        /// <returns></returns>
        public ResultInfo<IList<FundCompany>> GetFundCompanies()
        {
            var provider = new InstanceProvider<FundManager>();
            return CoreAccess.TryCall(EDomain, provider.Instance.GetFundCompanies, CoreBase.CallSuccess, provider.Close);
        }
    }
}
