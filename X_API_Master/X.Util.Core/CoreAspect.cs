using System;

namespace X.Util.Core
{
    public class CoreAspect<T>
    {
        private Func<Func<T>, T> _chain;
        public static CoreAspect<T> Define => new CoreAspect<T>();
        public Delegate WorkDelegate { get; set; }

        public T Run(Func<T> work)
        {
            WorkDelegate = work;
            return Equals(_chain, null) ? work() : _chain(work);
        }

        public CoreAspect<T> Combine(Func<Func<T>, T> newAspectDelegate)
        {
            if (Equals(_chain, null)) _chain = newAspectDelegate;
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1>(Func<Func<T>, T1, T> newAspectDelegate, T1 t1)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2>(Func<Func<T>, T1, T2, T> newAspectDelegate, T1 t1, T2 t2)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3>(Func<Func<T>, T1, T2, T3, T> newAspectDelegate, T1 t1, T2 t2, T3 t3)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4>(Func<Func<T>, T1, T2, T3, T4, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5>(Func<Func<T>, T1, T2, T3, T4, T5, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14));
                _chain = callAnother;
            }
            return this;
        }

        public CoreAspect<T> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<Func<T>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T> newAspectDelegate, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            if (Equals(_chain, null)) _chain = work => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            else
            {
                var existingChain = _chain;
                Func<Func<T>, T> callAnother = work => existingChain(() => newAspectDelegate(work, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15));
                _chain = callAnother;
            }
            return this;
        }
    }
}
