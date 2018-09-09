
using System;
using X.UI.Helper;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enums;
using X.Util.Provider;

namespace X.Win.TaskService
{
    public class TaskBusiness
    {
        /// <summary>
        /// 一分钟执行一次
        /// </summary>
        public void MinuteInvoke()
        {

        }

        /// <summary>
        /// 2分钟执行一次
        /// </summary>
        public void DoubleMinuteInvoke()
        {

        }

        /// <summary>
        /// 5分钟执行一次
        /// </summary>
        public void FiveMinuteInvoke()
        {

        }

        /// <summary>
        /// 15分钟执行一次
        /// </summary>
        public void FifteenMinuteInvoke()
        {

        }

        /// <summary>
        /// 30分钟执行一次
        /// </summary>
        public void HalfHourInvoke()
        {
            if (DateTime.Now.Hour >= 16)
            {
                var provider = new InstanceProvider<StockTask>(LogDomain.Ui);
                CoreAccess<StockTask>.TryCallAsync(provider.Client.Task1, provider, null, new LogOptions());
                CoreAccess<StockTask>.TryCallAsync(provider.Client.Task2, provider, null, new LogOptions());
            }
        }

        /// <summary>
        /// 60分钟执行一次
        /// </summary>
        public void HourInvoke()
        {

        }

        /// <summary>
        /// 720分钟执行一次
        /// </summary>
        public void HalfDayInvoke()
        {

        }
    }
}
