﻿using System;
using System.Threading;
using X.Util.Core.Kernel;
using X.Util.Entities;
using X.Util.Entities.Enums;
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
            var provider = new InstanceProvider<TaskBusiness>(LogDomain.Ui);
            while (true)
            {
                switch (i)
                {
                    case (int) EnumTaskLevel.Minute:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.MinuteInvoke, provider, null,new LogOptions());
                        break;
                    case (int) EnumTaskLevel.DoubleMinutes:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.DoubleMinuteInvoke, provider,null,new LogOptions());
                        break;
                    case (int) EnumTaskLevel.FiveMinutes:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.FiveMinuteInvoke, provider, null,new LogOptions());
                        break;
                    case (int) EnumTaskLevel.FifteenMinutes:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.FifteenMinuteInvoke, provider, null, new LogOptions());
                        break;
                    case (int) EnumTaskLevel.HalfHour:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.HalfHourInvoke, provider, null, new LogOptions());
                        break;
                    case (int) EnumTaskLevel.Hour:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.HourInvoke, provider, null, new LogOptions());
                        break;
                    case (int) EnumTaskLevel.HalfDay:
                        CoreAccess<TaskBusiness>.TryCallAsync(provider.Client.HalfDayInvoke, provider, null, new LogOptions());
                        break;
                }
                Thread.Sleep(1000 * 60 * i);
            }
        }
    }
}
