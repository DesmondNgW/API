using System;

namespace X.Util.Entities
{
    public class ActionContext
    {
        public Action Action { get; set; }
    }

    public class ActionContext<T>
    {
        public Action<T> Action { get; set; }

        public T Argument { get; set; }
    }

    public class ActionContext<T1, T2>
    {
        public Action<T1,T2> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
    }

    public class ActionContext<T1, T2, T3>
    {
        public Action<T1, T2, T3> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4>
    {
        public Action<T1, T2, T3, T4> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5>
    {
        public Action<T1, T2, T3, T4, T5> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6>
    {
        public Action<T1, T2, T3, T4, T5, T6> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public T14 Argument14 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public T14 Argument14 { get; set; }
        public T15 Argument15 { get; set; }
    }

    public class ActionContext<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Action { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public T14 Argument14 { get; set; }
        public T15 Argument15 { get; set; }
        public T16 Argument16 { get; set; }
    }

    public class FuncContext<TResult>
    {
        public Func<TResult> Func { get; set; }

        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T>
    {
        public Func<T, TResult> Func { get; set; }
        public T Argument { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2>
    {
        public Func<T1, T2, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3>
    {
        public Func<T1, T2, T3, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4>
    {
        public Func<T1, T2, T3, T4, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5>
    {
        public Func<T1, T2, T3, T4, T5, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6>
    {
        public Func<T1, T2, T3, T4, T5, T6, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public T14 Argument14 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public T14 Argument14 { get; set; }
        public T15 Argument15 { get; set; }
        public TResult Result { get; set; }
    }

    public class FuncContext<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Func { get; set; }
        public T1 Argument1 { get; set; }
        public T2 Argument2 { get; set; }
        public T3 Argument3 { get; set; }
        public T4 Argument4 { get; set; }
        public T5 Argument5 { get; set; }
        public T6 Argument6 { get; set; }
        public T7 Argument7 { get; set; }
        public T8 Argument8 { get; set; }
        public T9 Argument9 { get; set; }
        public T10 Argument10 { get; set; }
        public T11 Argument11 { get; set; }
        public T12 Argument12 { get; set; }
        public T13 Argument13 { get; set; }
        public T14 Argument14 { get; set; }
        public T15 Argument15 { get; set; }
        public T16 Argument16 { get; set; }
        public TResult Result { get; set; }
    }
}
