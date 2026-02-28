namespace GeoInferenceEngine.Backbone.Abstractions.Parts;
/// <summary>
/// 插件
/// </summary>
public interface IInferencePlugIn
{
    /// <summary>
    /// 在初始化中添加响应位置
    /// </summary>
    public void Init();
}

/// <summary>
/// 独立插件特性标记
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class InferenceStandPlugInAttribute : Attribute
{
}
