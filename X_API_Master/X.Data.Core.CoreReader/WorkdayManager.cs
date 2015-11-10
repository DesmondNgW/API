using System;
using X.Data.Core.CoreReader.Services;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Data.Core.CoreReader
{
    public class WorkdayManager
    {
        private const LogDomain EDomain = LogDomain.Core;

        public DateTime GetCurrentWorkDay()
        {
            var client = new WcfProvider<IDateService>(EDomain);
            return GetWorkday(CoreAccess.TryCall(EDomain, client.Instance.FetchDateTime, CoreBase.CallSuccess, client.Dispose, true, client.EndpointAddress));
        }

        public DateTime GetWorkday(DateTime dt)
        {
            var client = new WcfProvider<IDateService>(EDomain);
            return CoreAccess.TryCall(EDomain, client.Instance.FetchCurrWorkDay, dt, CoreBase.CallSuccess, client.Dispose, true, client.EndpointAddress);
        }

        public DateTime GetPointWorkday(DateTime dt, int day)
        {
            var client = new WcfProvider<IDateService>(EDomain);
            return CoreAccess.TryCall(EDomain, client.Instance.FetchPointWorkday, dt, day, CoreBase.CallSuccess, client.Dispose, true, client.EndpointAddress);
        }
    }
}
