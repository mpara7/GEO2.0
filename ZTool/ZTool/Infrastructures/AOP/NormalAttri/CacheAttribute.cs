namespace ZTool.Infrastructures.AOP.NormalAttri;

/// <summary>
/// 如果返回值是基础类型需要最好写成？如int?
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="R"></typeparam>
public class CacheAttribute<T, R> : InvokerAttribute
{
    CacheServer<T, R> cacheServer = new();
    public override void Invoke(InvocationContext invocationContext)
    {
        var s = cacheServer.Get((T)invocationContext.Parameters[0], out bool found);

        if (found)
        {
            invocationContext.ReturnValue = s;
        }
        else
        {
            Next();
            var r = (R)invocationContext.ReturnValue;
            cacheServer.Set((T)invocationContext.Parameters[0], r);
        }
    }
}
/// <summary>
/// 如果返回值是基础类型需要最好写成？如int?
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="R"></typeparam>
public class CacheAttribute<T1, T2, R> : InvokerAttribute
{
    CacheServer<T1, T2, R> cacheServer = new();
    public override void Invoke(InvocationContext invocationContext)
    {
        var s = cacheServer.Get((T1)invocationContext.Parameters[0], (T2)invocationContext.Parameters[1], out bool found);

        if (found)
        {
            invocationContext.ReturnValue = s;
        }
        else
        {
            Next();
            var r = (R)invocationContext.ReturnValue;
            cacheServer.Set((T1)invocationContext.Parameters[0], (T2)invocationContext.Parameters[1], r);
        }

    }
}
/// <summary>
/// 如果返回值是基础类型需要最好写成？如int?
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="R"></typeparam>
public class CacheAttribute<T1, T2, T3, R> : InvokerAttribute
{
    CacheServer<T1, T2, T3, R> cacheServer = new();
    public override void Invoke(InvocationContext invocationContext)
    {
        var s = cacheServer.Get((T1)invocationContext.Parameters[0], (T2)invocationContext.Parameters[1], (T3)invocationContext.Parameters[2], out bool found);

        if (found)
        {
            invocationContext.ReturnValue = s;
        }
        else
        {
            Next();
            var r = (R)invocationContext.ReturnValue;
            cacheServer.Set((T1)invocationContext.Parameters[0], (T2)invocationContext.Parameters[1], (T3)invocationContext.Parameters[2], r);
        }
    }
}