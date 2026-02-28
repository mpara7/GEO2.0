namespace ZTool.InnerTypeTools.BaseTyps;
public static class FuncTool
{
    public static Func<R> Pack<T1, R>(this Func<T1, R> func, T1 arg0)
    {
        return () => { return func(arg0); };
    }
    public static Func<R> Pack<T1, T2, R>(this Func<T1, T2, R> func, T1 arg0, T2 arg1)
    {
        return () => { return func(arg0, arg1); };
    }
    public static Func<R> Pack<T1, T2, T3, R>(this Func<T1, T2, T3, R> func, T1 arg0, T2 arg1, T3 arg2)
    {
        return () => { return func(arg0, arg1, arg2); };
    }
    public static Func<R> Pack<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> func, T1 arg0, T2 arg1, T3 arg2, T4 arg3)
    {
        return () => { return func(arg0, arg1, arg2, arg3); };
    }
    public static Func<R> Pack<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
    {
        return () => { return func(arg0, arg1, arg2, arg3, arg4); };
    }
    public static Func<R> Pack<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4, T6 arg5)
    {
        return () => { return func(arg0, arg1, arg2, arg3, arg4, arg5); };
    }
}
