namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
/// <summary>
/// 打包的通用输入
/// </summary>
public class InferenceUniversalInputs
{
    public List<AInferenceInput> Inputs { get; set; } = new List<AInferenceInput>();
    public EngineConfig EngineConfig { get; set; } = null;
}
