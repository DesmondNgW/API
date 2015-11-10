using Em.Entities;
using System;
using X.Data.Core.CoreReader;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Business.Core
{
    public class CustomerBusiness
    {
        private const LogDomain EDomain = LogDomain.Cache;

        public ResultInfo<Customer> GetCustomerInfo(string certificateType, string certificateNo)
        {
            var provider = new InstanceProvider<CustomerManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetCustomerInfo, certificateType, certificateNo, CoreBase.CallSuccess, provider.Close);
        }

        public ResultInfo<Customer> GetCustomerInfoByCustomerNo(string customerNo)
        {
            var provider = new InstanceProvider<CustomerManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetCustomerInfoByCustomerNo, customerNo, CoreBase.CallSuccess, provider.Close);
        }

        public ResultInfo<Customer> Login(string certificateType, string certificateNo, string password)
        {
            var provider = new InstanceProvider<CustomerManager>();
            return CoreAccess.Call(EDomain, provider.Instance.Login, certificateType, certificateNo, password, CoreBase.CallSuccess, provider.Close);
        }

        public ResultInfo<Customer> Login_Default(string loginName, string password)
        {
            var provider = new InstanceProvider<CustomerManager>();
            return CoreAccess.TryCall(EDomain, provider.Instance.Login_Default, loginName, password, CoreBase.CallSuccess, provider.Close);
        }


        public static void ClearCache4CustomerInfo(string certificateType, string certificateNo)
        {
            var provider = new InstanceProvider<CustomerManager>();
            CoreCache.ClearCache(((Func<string, string, ResultInfo<Customer>>)provider.Instance.GetCustomerInfo).Method, new object[] { certificateType, certificateNo });
        }

        public static void ClearCache4CustomerInfo(string customerNo)
        {
            var provider = new InstanceProvider<CustomerManager>();
            CoreCache.ClearCache(((Func<string, ResultInfo<Customer>>)provider.Instance.GetCustomerInfoByCustomerNo).Method, new object[] { customerNo });
        }

        public static void ClearAllCache4Customer(string customerNo)
        {
            ClearCache4CustomerInfo(customerNo);
            RouterBusiness.ClearCache4RouteInfo(customerNo);
        }
    }
}
