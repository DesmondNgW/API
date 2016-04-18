using System;

namespace X.Util.Core
{
    /// <summary>
    /// 反复重试调用方法多次
    /// </summary>
    public class CoreSecurity<TS> where TS : Exception
    {
        public static T TryAgain<T>(Func<T> func, int maxTimes) 
        {
            try
            {
                return func();
            }
            catch (TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1>(Func<T1, T> func, T1 t1, int maxTimes)
        {
            try
            {
                return func(t1);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2>(Func<T1, T2, T> func, T1 t1, T2 t2, int maxTimes)
        {
            try
            {
                return func(t1, t2);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3>(Func<T1, T2, T3, T> func, T1 t1, T2 t2, T3 t3, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4>(Func<T1, T2, T3, T4, T> func, T1 t1, T2 t2, T3 t3, T4 t4, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, int maxTimes)
        {
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, maxTimes);
                throw;
            }
        }

        public static T TryAgain<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, int maxTimes)
        {            
            try
            {
                return func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) return TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, maxTimes);
                throw;
            }          
        }

        public static void TryAgain(Action func, int maxTimes)
        {
            try
            {
                func();
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1>(Action<T1> func, T1 t1, int maxTimes)
        {
            try
            {
               func(t1);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2>(Action<T1, T2> func, T1 t1, T2 t2, int maxTimes)
        {
            try
            {
                func(t1, t2);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3>(Action<T1, T2, T3> func, T1 t1, T2 t2, T3 t3, int maxTimes)
        {
            try
            {
                func(t1, t2, t3);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, maxTimes);
                else throw;
            }
        }

        public static void TryAgain<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16, int maxTimes)
        {
            try
            {
                func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            }
            catch(TS)
            {
                maxTimes--;
                if (maxTimes > 0) TryAgain(func, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, maxTimes);
                else throw;
            }
        }
    }
}
