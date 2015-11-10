using Em.Entities;
using Em.FundTrade.Route.Entities;
using System.Reflection;
using X.Data.Core.CoreReader.Services;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Data.Core.CoreReader
{
    public class RouterManager
    {
        private const LogDomain EDomain = LogDomain.Core;

        public RouteInfo GetRouteInfoByCustomerNo(string customerNo)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<IRouter>(EDomain);
                var iresult = CoreAccess.TryCall(EDomain, client.Instance.GetRouteInfoByCustomerNo, customerNo, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
                return new ResultInfo<RouteInfo> { Succeed = iresult != null, Result = iresult };
            }, MethodBase.GetCurrentMethod(), new object[] { customerNo }, AppConfig.RouterCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30).Result;
        }

        public RouteInfo GetRouteInfoByLoginInfo(Em.FundTrade.Route.Entities.LoginInfo info)
        {
            var client = new WcfProxyPoolProvider<IRouter>(EDomain);
            return CoreAccess.TryCall(EDomain, client.Instance.GetRouteInfoByLoginInfo, info, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
        }
    }
}
