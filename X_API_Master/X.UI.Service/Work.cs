using System;
using System.Reflection;
using System.Threading;
using X.Util.Core;
using X.Util.Provider;
namespace X.UI.Service
{
    public enum EnumTimerLevel
    {
        //Minute = 1,
        //DoubleMinutes = 2,
        //FiveMinutes = 5,
        //FifteenMinutes = 15,
        //HalfHour = 30,
        //Hour = 60,
        //HalfDay = 720
    }

    public class Work
    {
        public Thread[] pool { get; set; }

        public void Start()
        {
            var list = (EnumTimerLevel[])Enum.GetValues(typeof(EnumTimerLevel));
            pool = new Thread[list.Length];
            for (var i = 0; i < list.Length; i++)
            {
                var method = typeof(Work).GetMethod(list[i].ToString() + "Invoke");
                if (method != null)
                {
                    pool[i] = new Thread((obj) =>
                    {
                        method.Invoke(null, new object[] { obj });
                    }) { IsBackground = true };
                    pool[i].Start(list[i].GetHashCode());
                }
            }
        }

        public void Stop()
        {
            if (pool != null && pool.Length > 0)
            {
                for (var i = 0; i < pool.Length; i++)
                {
                    if (pool[i] != null) pool[i].Abort();
                }
            }
        }

        public static void MinuteInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }

        public static void DoubleMinutesInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }

        public static void FiveMinutesInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }

        public static void FifteenMinutesInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }

        public static void HalfHourInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }

        public static void HourInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }

        public static void HalfDayInvoke(object obj)
        {
            while (true)
            {
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread BeginInvoke!"));
                var provider = new InstanceProvider<Business>();
                CoreAccess.TryCallAsync(LogDomain.Ui, provider.Instance.Task1, provider.Close, null);
                Logger.Info(MethodBase.GetCurrentMethod(), LogDomain.Ui, null, string.Format("current thread Sleep!"));
                Thread.Sleep(Convert.ToInt32(obj) * 1000 * 60);
            }
        }
    }
}
