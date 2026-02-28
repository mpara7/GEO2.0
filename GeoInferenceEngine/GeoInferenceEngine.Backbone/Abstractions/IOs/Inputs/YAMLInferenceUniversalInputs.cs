namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
/// <summary>
/// 打包的通用输入 方便传输保存
/// </summary>
public class YAMLInferenceUniversalInputs
{
    public void From(InferenceUniversalInputs uInputs)
    {
        foreach (var input in uInputs.Inputs)
        {
            Inputs.Add(input.GetType().AssemblyQualifiedName, YAML.Serialize(input));
        }
        EngineConfig = YAML.Serialize(uInputs.EngineConfig);
    }
    public InferenceUniversalInputs To()
    {
        InferenceUniversalInputs uInputs = new InferenceUniversalInputs();
        foreach (var kv in Inputs)
        {
            uInputs.Inputs.Add((AInferenceInput)YAML.Deserialize(kv.Key, kv.Value));
        }
        uInputs.EngineConfig = YAML.Deserialize<EngineConfig>(EngineConfig);
        return uInputs;

    }
    /// <summary>
    /// 用于反序列化
    /// </summary>
    public YAMLInferenceUniversalInputs() { }
    public YAMLInferenceUniversalInputs(Dictionary<string, string> inputs, string engineConfig)
    {
        Inputs = inputs;
        EngineConfig = engineConfig;
    }
    /// <summary>
    /// input的完整类型名称+input的yaml
    /// </summary>
    public Dictionary<string, string> Inputs { get; set; } = new();
    public string EngineConfig { get; set; } = null;
    public void AddInput(AInferenceInput input)
    {
        Inputs.Add(input.GetType().AssemblyQualifiedName, YAML.Serialize(input));
    }
    public void SetConfig(EngineConfig engineConfig)
    {
        EngineConfig = YAML.Serialize(engineConfig);
    }
}
