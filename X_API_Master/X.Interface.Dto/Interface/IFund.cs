using System.Collections.Generic;
using X.Interface.Dto.HttpResponse;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 基金对象接口
    /// </summary>
    public interface IFund
    {
        /// <summary>
        /// 获取基金信息
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns>基金对象序列化</returns>
        ApiResult<FundDto> GetFund(string fundCode);

        /// <summary>
        /// 获取净值
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns>净值对象序列化</returns>
        ApiResult<FundNavDto> GetFundNav(string fundCode);

        /// <summary>
        /// 获取基金列表
        /// </summary>
        /// <returns>基金对象序列化</returns>
        ApiResult<List<FundDto>> GetFunds();

        /// <summary>
        /// 获取基金公司列表
        /// </summary>
        /// <returns>基金公司对象序列化</returns>
        ApiResult<IList<FundCompanyDto>> GetFundCompanies();

        /// <summary>
        /// 获取基金确认日和到账日
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns>基金到期日和对账日序列化</returns>
        ApiResult<FundDaysDto> GetFundDay(string fundCode);
    }
}
