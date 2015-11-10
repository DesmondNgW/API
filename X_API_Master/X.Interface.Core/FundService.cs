using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using X.Business.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Interface.Core
{
    public class FundService : IFund
    {
        private const LogDomain EDomain = LogDomain.Business;

        public ApiResult<FundDto> GetFund(string fundCode)
        {
            if (!Regex.IsMatch(fundCode, "^[0-9A-Za-z]{6}$")) throw new ArgumentException(ErrorMessage.FundCodeFormat);
            var provider = new InstanceProvider<FundBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetFund, fundCode, CoreBase.CallSuccess, provider.Close);
            return ServiceHelper.Convert(iresult, () => iresult.Result.AutoMapper<FundDto>());
        }

        public ApiResult<FundNavDto> GetFundNav(string fundCode)
        {
            if (!Regex.IsMatch(fundCode, "^[0-9A-Za-z]{6}$")) throw new ArgumentException(ErrorMessage.FundCodeFormat);
            var provider = new InstanceProvider<FundBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetFundNav, fundCode, CoreBase.CallSuccess, provider.Close);
            return ServiceHelper.Convert(iresult, () => iresult.Result.AutoMapper<FundNavDto>());
        }

        public ApiResult<List<FundDto>> GetFunds()
        {
            var provider = new InstanceProvider<FundBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetFunds, CoreBase.CallSuccess, provider.Close);
            return ServiceHelper.Convert(iresult, () => iresult.Result.AutoMapper<List<FundDto>>());
        }

        public ApiResult<IList<FundCompanyDto>> GetFundCompanies()
        {
            var provider = new InstanceProvider<FundBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetFundCompanies, CoreBase.CallSuccess, provider.Close);
            return ServiceHelper.Convert(iresult, () => iresult.Result.AutoMapper<IList<FundCompanyDto>>());
        }

        public ApiResult<FundDaysDto> GetFundDay(string fundCode)
        {
            if (!Regex.IsMatch(fundCode, "^[0-9A-Za-z]{6}$")) throw new ArgumentException(ErrorMessage.FundCodeFormat);
            var provider = new InstanceProvider<WorkdayBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetFundDay, fundCode, CoreBase.CallSuccess, provider.Close);
            return ServiceHelper.Convert(iresult, () => iresult.Result.AutoMapper<FundDaysDto>());
        }
    }
}
