using System;
using System.Web.Http;
using X.Interface.Core;
using X.Interface.Dto;
using X.Interface.Dto.Interface;
using X.UI.API.Util;
using X.Util.Core;
using X.Util.Provider;

namespace X.UI.API.Controllers
{
    /// <summary>
    /// 工作日
    /// </summary>
    public class WorkDayController : VisitorBaseController
    {
        /// <summary>
        /// 获取当前工作日
        /// </summary>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<DateTime> GetCurrentWorkDay()
        {
            var provider = new InstanceProvider<IWorkDay>(typeof(WorkDayService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetCurrentWorkDay, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取指定日期的工作日
        /// </summary>
        /// <param name="dt">指定的日期</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<DateTime> GetWorkday(DateTime dt)
        {
            var provider = new InstanceProvider<IWorkDay>(typeof(WorkDayService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetWorkday, dt, ControllerHelper.CallSuccess, provider.Close);
        }

        /// <summary>
        /// 获取指定日期向前（后）推算指定天数的工作日
        /// </summary>
        /// <param name="dt">指定日期</param>
        /// <param name="day">指定天数</param>
        /// <returns>指定对象序列化</returns>
        [HttpGet]
        public ApiResult<DateTime> GetPointWorkday(DateTime dt, int day)
        {
            var provider = new InstanceProvider<IWorkDay>(typeof(WorkDayService));
            return CoreAccess.Call(ControllerHelper.EDomain, provider.Instance.GetPointWorkday, dt, day, ControllerHelper.CallSuccess, provider.Close);
        }
    }
}