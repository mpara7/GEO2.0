namespace ZTool.Infrastructures.AOP;
public class InvocationContext
{
    /// <summary>
    /// 上下文数据
    /// </summary>
    public Dictionary<string, object> Context { get; set; }
    /// <summary>
    /// 参数列表
    /// </summary>
    public object[] Parameters;
    /// <summary>
    /// 返回值
    /// </summary>
    public object ReturnValue;
}
