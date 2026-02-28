using OneOf.Types;

namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;


/// <summary>
/// 打包的通用输出
/// </summary>
public class InferenceUniversalOutputs
{
    public bool HasWarning { get; set; } = false;
    public InferenceWarningInfo WarningInfo { get; set; } = new();
    public bool IsActiveStop { get; set; }
    public List<string> ActiveStopReasons { get; set; }=new List<string>();

    public bool IsCracked { get; set; } = false;
    public InferenceCrackedInfo CrackedInfo { get; set; } = new();

    public Dictionary<string, AInferenceOutput> Outputs { get; set; } = new();

    public override string ToString()
    {
        string result = "";
        if (HasWarning)
        {
            result += "存在警告：\n";
            foreach (string warning in WarningInfo.Warnings)
            {
                result += warning + "\n";
            }
        }
        if (IsActiveStop)
        {
            result += "推理机主动停机：\n";
            foreach (var reason in ActiveStopReasons)
            {
                result += reason + "\n";
            }
        }
        else if (IsCracked)
        {
            Console.WriteLine("已崩溃");
            result += CrackedInfo.ToString();
        }
        foreach (var output in Outputs)
        {
            result += result+"\n\n";
        }
        return result;
    }
}
