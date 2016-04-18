using System;
using System.Threading;
using X.Util.Core;
using X.Util.Provider;

namespace X.Win.TaskService
{
    /// <summary>
    /// 任务级别
    /// </summary>
    public enum EnumTaskLevel
    {
        Minute = 1,
        DoubleMinutes = 2,
        FiveMinutes = 5,
        FifteenMinutes = 15,
        HalfHour = 30,
        Hour = 60,
        HalfDay = 720
    }

    public class Task
    {
        protected static Thread[] Pool { get; set; }
        public static void CreateThreads()
        {
            var list = (EnumTaskLevel[])Enum.GetValues(typeof(EnumTaskLevel));
            Pool = new Thread[list.Length];
            for (var i = 0; i < list.Length; i++)
            {
                Pool[i] = new Thread(LoopCall) { IsBackground = true };
                Pool[i].Start(list[i].GetHashCode());
            }
        }

        public static void AbortThreads()
        {
            if (Pool == null || Pool.Length <= 0) return;
            foreach (var t in Pool)
            {
                t.Abort();
            }
        }

        private static void LoopCall(object obj)
        {
            var i = Convert.ToInt32(obj);
            var provider = new InstanceProvider<TaskBusiness>();
            while (true)
            {
                switch (i)
                {
                    case (int) EnumTaskLevel.Minute:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.MinuteInvoke, provider.Close, null);
                        break;
                    case (int) EnumTaskLevel.DoubleMinutes:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.DoubleMinuteInvoke, provider.Close, null);
                        break;
                    case (int) EnumTaskLevel.FiveMinutes:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.FiveMinuteInvoke, provider.Close, null);
                        break;
                    case (int) EnumTaskLevel.FifteenMinutes:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.FifteenMinuteInvoke, provider.Close, null);
                        break;
                    case (int) EnumTaskLevel.HalfHour:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.HalfHourInvoke, provider.Close, null);
                        break;
                    case (int) EnumTaskLevel.Hour:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.HourInvoke, provider.Close, null);
                        break;
                    case (int) EnumTaskLevel.HalfDay:
                        CoreAccess<Exception>.TryCallAsync(LogDomain.Ui, provider.Instance.HalfDayInvoke, provider.Close, null);
                        break;
                }
                Thread.Sleep(1000 * 60 * i);
            }
        }
    }
}
