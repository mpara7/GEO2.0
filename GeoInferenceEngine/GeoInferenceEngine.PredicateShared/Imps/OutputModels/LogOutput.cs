using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;

namespace GeoInferenceEngine.Knowledges.Imps.IOs.Outputs;
[Description("日志输出")]
public class LogOutput : AInferenceOutput
{
    public string Content { get; set; } = "";
    public override string ToString()
    {
        return Content;
    }
}

