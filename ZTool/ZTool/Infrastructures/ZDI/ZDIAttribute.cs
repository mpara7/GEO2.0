namespace ZTool.Infrastructures.ZDI;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
/// <summary>
/// 用于Property或者Field,可绕过 private 访问控制 直接注入
/// </summary>
public class ZDIAttribute : Attribute
{
    public ZDIAttribute()
    {
    }
}
