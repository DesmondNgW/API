using Em.Entities;
using System.Collections.Generic;
using X.Data.Core.CoreReader;
using X.Util.Core;
using X.Util.Extend.Core;
using X.Util.Provider;

namespace X.Business.Core
{
    public class AngieBusiness
    {
        private const LogDomain EDomain = LogDomain.Cache;
        /// <summary>
        /// 基金白名单
        /// </summary>
        /// <returns></returns>
        public ResultInfo<List<WhiteLists>> GetFundWhiteList()
        {
            var provider = new InstanceProvider<AngieOneManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetFundWhiteList, CoreBase.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ResultInfo<Fund> GetSpecialAccountsFund()
        {
            var provider = new InstanceProvider<AngieOneManager>();
            return CoreAccess.Call(EDomain, provider.Instance.GetSpecialAccountsFund, CoreBase.CallSuccess, provider.Close);
        }
    }
}
