using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.IO.Outputs;

[Description("求解目标输出生成器")]
public class TargetBaseOutputMaker : IInferenceOutputMaker<TargetBaseOutput>
{
    [ZDI]
    TargetBase tBase;
    string name;
    public string Name { get => name; set => name = value; }
    public TargetBaseOutput Make()
    {
        TargetBaseOutput output = new TargetBaseOutput();
        foreach (var item in tBase.ToProves)
        {
            output.Targets.Add(new InferenceTarget()
            {
                Index = item.Index + 1,
                IsSuccess = item.IsSuccess,
                Target = item.ToString()
            });
        }
        foreach (var item in tBase.ToSolves)
        {
            output.Targets.Add(new InferenceTarget()
            {
                Index = item.Index + 1,
                IsSuccess = item.IsSuccess,
                Target = item.ToString()
            });
        }
        output.Targets.Sort((a, b) => a.Index.CompareTo(b.Index));
        return output;
    }
}
