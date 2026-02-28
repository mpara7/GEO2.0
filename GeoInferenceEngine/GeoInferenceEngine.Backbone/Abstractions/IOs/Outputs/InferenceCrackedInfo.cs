namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;

public class InferenceCrackedInfo
{
    public string CurAction { get; set; } ="未知";
    public string Detail { get; set; } = null;
    public override string ToString()
    {
        return $"{CurAction}    {Detail}";
    }
}
