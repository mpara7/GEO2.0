using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;

using System.Text;

namespace GeoInferenceEngine.Knowledges.Imps.IOs.Outputs;
public class InferenceTarget
{
    public int Index { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string Target { get; set; } = "";
    public override string ToString()
    {
        if (IsSuccess) return $"第{Index}问：{Target}，已成功解决";
        return $"第{Index}问：{Target}，未解决";
    }
}
[Description("求解目标库输出")]
public class TargetBaseOutput : AInferenceOutput
{
    public bool IsAllSucess { get; set; }
    /// <summary>
    /// 默认按顺序
    /// </summary>
    public List<InferenceTarget> Targets { get; set; } = new();
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in Targets)
        {
            sb.Append(item.ToString());
        }
        return sb.ToString();
    }
}