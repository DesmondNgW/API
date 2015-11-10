using Em.FundTrade.Route.Entities;
using X.Data.Core.CoreReader;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Business.Core
{
    public class RouterBusiness
    {
        private const LogDomain EDomain = LogDomain.Cache;
        /// <summary>
        /// 路由节点信息
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public RouteInfo GetRouteInfoByCustomerNo(string customerNo)
        {
            var provider = new InstanceProvider<RouterManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetRouteInfoByCustomerNo, customerNo, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 登录获取路由信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public RouteInfo GetRouteInfoByLoginInfo(string loginName)
        {
            var provider = new InstanceProvider<RouterManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetRouteInfoByLoginInfo, new LoginInfo { Name= loginName ,Type=LoginType.Default}, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 路由分区节点
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public Zone GetRouteZoneByCustomerNo(string customerNo)
        {
            var info = GetRouteInfoByCustomerNo(customerNo);
            return info?.Zone ?? Zone.TradeOne;
        }

        /// <summary>
        /// 登录获取路由分区节点
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public Zone GetRouteZoneByLoginInfo(string loginName)
        {
            var info = GetRouteInfoByLoginInfo(loginName);
            return info?.Zone ?? Zone.TradeOne;
        }

        /// <summary>
        /// 清除路由缓存
        /// </summary>
        /// <param name="customerNo"></param>
        /// <returns></returns>
        public static void ClearCache4RouteInfo(string customerNo)
        {
            var provider = new InstanceProvider<RouterManager>();
            CoreCache.ClearCache(((System.Func<string, RouteInfo>)provider.Instance.GetRouteInfoByCustomerNo).Method, new object[] { customerNo });
        }
    }
}
