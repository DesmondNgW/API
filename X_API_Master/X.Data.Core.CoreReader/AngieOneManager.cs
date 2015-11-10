using Em.Entities;
using System.Collections.Generic;
using System.Reflection;
using X.Data.Core.CoreReader.Services;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Data.Core.CoreReader
{
    public class AngieOneManager
    {
        private const LogDomain EDomain = LogDomain.Core;

        public ResultInfo<List<WhiteLists>> GetFundWhiteList()
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProvider<IAngieOneService>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetFundWhiteList, CoreBase.CallSuccess, client.Dispose, true, client.EndpointAddress);
            }, MethodBase.GetCurrentMethod(), new object[] { }, AppConfig.WhiteListCacheApp, true, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 45);
        }

        public ResultInfo<Fund> GetSpecialAccountsFund()
        {
            return CoreCache.GetAbsoluteCacheData(() =>
            {
                var client = new WcfProvider<IAngieOneService>(EDomain);
                return CoreAccess.TryCall(EDomain, client.Instance.GetSpecialAccountsFund, CoreBase.CallSuccess, client.Dispose, true, client.EndpointAddress);
            }, MethodBase.GetCurrentMethod(), new object[] { }, AppConfig.WhiteListCacheApp, true, true, EnumCacheType.MemBoth, EnumCacheTimeLevel.Minute, 30);
        }
    }
}
