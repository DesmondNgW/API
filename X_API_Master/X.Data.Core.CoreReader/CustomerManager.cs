using Em.Entities;
using System.Reflection;
using X.Data.Core.CoreReader.Services;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Data.Core.CoreReader
{
    public class CustomerManager
    {
        private const LogDomain EDomain = LogDomain.Core;
        public ResultInfo<Customer> GetCustomerInfoByCustomerNo(string customerNo)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<ICustomerManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetCustomerInfoByCustomerNo, customerNo, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { customerNo }, AppConfig.CustomerCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }

        public ResultInfo<Customer> GetCustomerInfo(string certificateType, string certificateNo)
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProxyPoolProvider<ICustomerManager>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetCustomerInfo, certificateType, certificateNo, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
            }, MethodBase.GetCurrentMethod(), new object[] { certificateType, certificateNo }, AppConfig.CustomerCacheApp, false, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }

        public ResultInfo<Customer> Login(string certificateType, string certificateNo, string password)
        {
            var client = new WcfProxyPoolProvider<ICustomerManager>(EDomain);
            return CoreAccess.TryCall(EDomain, client.Instance.Login, certificateType, certificateNo, password, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
        }

        public ResultInfo<Customer> Login_Default(string loginName, string password)
        {
            var client = new WcfProxyPoolProvider<ICustomerManager>(EDomain);
            return CoreAccess.TryCall(EDomain, client.Instance.Login_Default, loginName, password, CoreBase.CallSuccess, client.Dispose, true, client.ServiceUri);
        }
    }
}
