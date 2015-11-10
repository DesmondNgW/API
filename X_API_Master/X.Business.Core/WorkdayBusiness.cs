using Em.Entities;
using System;
using X.Data.Core.CoreReader;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Business.Core
{
    public class WorkdayBusiness
    {
        private const LogDomain EDomain = LogDomain.Cache;
        /// <summary>
        /// 当前日期的所属工作日
        /// </summary>
        /// <returns></returns>
        public DateTime GetCurrentWorkDay()
        {
            var provider = new InstanceProvider<WorkdayManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetCurrentWorkDay, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 指定日期的所属工作日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DateTime GetWorkday(DateTime dt)
        {
            var provider = new InstanceProvider<WorkdayManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetWorkday, dt, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 指定日期的前（后）n个工作日
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public DateTime GetPointWorkday(DateTime dt, int day)
        {
            var provider = new InstanceProvider<WorkdayManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetPointWorkday, dt, day, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取指定基金的资金到账日（FundDays对象）
        /// </summary>
        /// <param name="fundCode">基金代码</param>
        /// <returns></returns>
        public ResultInfo<FundDays> GetFundDay(string fundCode)
        {
            var provider = new InstanceProvider<FundManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetFundDay, fundCode, CoreBase.CallSuccess, provider.Close);
        }
    }
}
