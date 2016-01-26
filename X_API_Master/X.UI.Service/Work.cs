using System;
using System.Threading;
namespace X.UI.Service
{
    public enum EnumTimerLevel
    {
        Minute = 1,
        DoubleMinutes = 2,
        FiveMinutes = 5,
        FifteenMinutes = 15,
        HalfHour = 30,
        Hour = 60,
        HalfDay = 720
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

        protected static void MinuteInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }

        protected static void DoubleMinutesInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }

        protected static void FiveMinutesInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }

        protected static void FifteenMinutesInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }

        protected static void HalfHourInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }

        protected static void HourInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }

        protected static void HalfDayInvoke(object obj)
        {
            Console.WriteLine(obj);
            // code fix 
            Thread.Sleep(Convert.ToInt32(obj) * 1000);
        }
    }
}
