using System.Collections.Generic;
using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;
using X.UI.API.Util;
using X.Util.Core;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    /// <summary>
    /// 基金对象
    /// </summary>
    public class FundController : VisitorBaseController
    {
        /// <summary>
        /// 获取基金公司列表
        /// </summary>
        /// <returns>基金公司对象序列化</returns>
        [HttpGet]
        public ApiResult<IList<FundCompanyDto>> GetFundCompanies()
        {
            var provider = new InstanceProvider<IFund>(typeof(FundService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetFundCompanies, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取基金列表
        /// </summary>
        /// <returns>基金对象序列化</returns>
        [HttpGet]
        public ApiResult<List<FundDto>> GetFunds()
        {
            var provider = new InstanceProvider<IFund>(typeof(FundService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetFunds, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取基金信息
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns>基金对象序列化</returns>
        [HttpGet]
        public ApiResult<FundDto> GetFund(string fundCode)
        {
            var provider = new InstanceProvider<IFund>(typeof(FundService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetFund, fundCode, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取净值
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns>基金净值对象序列化</returns>
        [HttpGet]
        public ApiResult<FundNavDto> GetFundNav(string fundCode)
        {
            var provider = new InstanceProvider<IFund>(typeof(FundService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetFundNav, fundCode, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取基金确认日和到账日
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns>基金到期日和对账日序列化</returns>
        [HttpGet]
        public ApiResult<FundDaysDto> GetFundDay(string fundCode)
        {
            var provider = new InstanceProvider<IFund>(typeof(FundService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetFundDay, fundCode, ControllerHelper.CallSuccess, provider.Close);
        }
    }
}