using System;
using X.Business.Core;
using X.Interface.Dto;
using X.Interface.Dto.Interface;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Interface.Core
{
    public class WorkDayService : IWorkDay
    {
        private const LogDomain EDomain = LogDomain.Business;
        public ApiResult<DateTime> GetCurrentWorkDay()
        {
            var provider = new InstanceProvider<WorkdayBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetCurrentWorkDay, CoreBase.CallSuccess, provider.Close);
            return new ApiResult<DateTime> { Success = !Equals(iresult, default(DateTime)), Data = iresult };
        }

        public ApiResult<DateTime> GetWorkday(DateTime dt)
        {
            var provider = new InstanceProvider<WorkdayBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetWorkday, dt, CoreBase.CallSuccess, provider.Close);
            return new ApiResult<DateTime> { Success = !Equals(iresult, default(DateTime)), Data = iresult };
        }

        public ApiResult<DateTime> GetPointWorkday(DateTime dt, int day)
        {
            var provider = new InstanceProvider<WorkdayBusiness>();
            var iresult = CoreAccess.Call(EDomain, provider.Instance.GetPointWorkday, dt, day, CoreBase.CallSuccess, provider.Close);
            return new ApiResult<DateTime> { Success = !Equals(iresult, default(DateTime)), Data = iresult };
        }
    }
}
