namespace ZTool.InnerTypeTools.BaseTyps;
public static class ActionTool
{
    public static Action Pack<T1>(this Action<T1> action, T1 arg0)
    {
        return () => { action(arg0); };
    }
    public static Action Pack<T1, T2>(this Action<T1, T2> action, T1 arg0, T2 arg1)
    {
        return () => { action(arg0, arg1); };
    }
    public static Action Pack<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg0, T2 arg1, T3 arg2)
    {
        return () => { action(arg0, arg1, arg2); };
    }
    public static Action Pack<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg0, T2 arg1, T3 arg2, T4 arg3)
    {
        return () => { action(arg0, arg1, arg2, arg3); };
    }
    public static Action Pack<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
    {
        return () => { action(arg0, arg1, arg2, arg3, arg4); };
    }
    public static Action Pack<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4, T6 arg5)
    {
        return () => { action(arg0, arg1, arg2, arg3, arg4, arg5); };
    }
}
