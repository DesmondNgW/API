using System;
using System.Globalization;
using Em.FundTrade.EncryptHelper;
using X.Interface.Dto;
using X.Interface.Dto.HttpResponse;
using X.Interface.Dto.Interface;

namespace X.Interface.Other
{
    public class KmsManagerService : IKms
    {
        public EncryptResult MobileEncrypt(string mobile)
        {
            return EncryptHelper.Instance.MobileEncrypt(mobile);
        }

        public ApiResult<DateTimeDto> Now()
        {
            var ret = new ApiResult<DateTimeDto> {Success = true, Data = new DateTimeDto {DateTime = DateTime.Now}};
            var gc = new GregorianCalendar();
            var weekOfYear = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            ret.Data.WeekOfYear = weekOfYear;
            ret.Data.DayOfYear = ret.Data.DateTime.DayOfYear;
            return ret;
        }
    }
}
