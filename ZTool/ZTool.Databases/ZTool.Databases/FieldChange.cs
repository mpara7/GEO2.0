namespace ZTool.Databases;
/// <summary>
/// 用于数据更新
/// MongoDB中似乎可以支持点点获取字段的字段
/// </summary>
/// <param name="Field"></param>
/// <param name="NewValue"></param>
public record FieldChange(string Field, dynamic NewValue);