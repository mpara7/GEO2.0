namespace ZTool.Infrastructures.AOP;
/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public abstract class InvokerAttribute : Attribute
{
    /// <summary>
    /// 调用下一层函数
    /// </summary>
    public Action Next;
    public abstract void Invoke(InvocationContext invocationContext);
}
