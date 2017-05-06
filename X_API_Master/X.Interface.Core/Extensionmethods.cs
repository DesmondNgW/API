using System;
using X.Interface.Dto;
using X.Util.Entities;
using X.Util.Extend.Core;

namespace X.Interface.Core
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 转换实体
        /// </summary>
        /// <typeparam name="T">UI实体</typeparam>
        /// <typeparam name="TS">数据库实体</typeparam>
        /// <param name="iresult">接口数据</param>
        /// <param name="init">初始化UI实体</param>
        /// <returns></returns>
        public static ApiResult<T> Convert<T, TS>(this CacheResult<TS> iresult, Func<TS, T> init)
        {
            if (Equals(iresult, null)) iresult = new CacheResult<TS> { Succeed = false, Message = CoreBase.CoreCacheMesssage };
            var result = new ApiResult<T>
            {
                Success = iresult.Succeed,
                DebugError = iresult.Message,
                Error = iresult.Message,
                Code = iresult.ErrorCode
            };
            if (CoreBase.CallSuccess(iresult))
            {
                result.Success = true;
                if (init != null) result.Data = init(iresult.Result);
            }
            else if (string.IsNullOrEmpty(iresult.Message)) result.DebugError = result.Error = CoreBase.CoreCacheMesssage;
            return result;
        }
    }
}
