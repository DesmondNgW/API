using System;

namespace X.Interface.Dto.Interface
{
    /// <summary>
    /// 工作日接口
    /// </summary>
    public interface IWorkDay
    {
        /// <summary>
        /// 获取当前工作日
        /// </summary>
        /// <returns>指定对象序列化</returns>
        ApiResult<DateTime> GetCurrentWorkDay();

        /// <summary>
        /// 获取指定日期的工作日
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns>指定对象序列化</returns>
        ApiResult<DateTime> GetWorkday(DateTime dt);

        /// <summary>
        /// 获取指定日期向前（后）推算指定天数的工作日
        /// </summary>
        /// <param name="dt">指定日期</param>
        /// <param name="day">指定天数</param>
        /// <returns>指定对象序列化</returns>
        ApiResult<DateTime> GetPointWorkday(DateTime dt, int day);
    }
}
