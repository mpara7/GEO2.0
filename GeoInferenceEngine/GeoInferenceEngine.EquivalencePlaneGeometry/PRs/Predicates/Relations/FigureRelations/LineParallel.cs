using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("直线平行", "线段平行")]
public class LineParallel : Knowledge
{
    public LineParallel(Line line1, Line line2)
    {
        Add(line1, line2);
        Normalize();
        SetHashCode();
    }
    public Line Line1 { get => (Line)Properties[0]; }
    public Line Line2 { get => (Line)Properties[1]; }
    public override void Normalize()
    {
        Sort();
    }
    public override string ToString() => $"{Properties[0]}//{Properties[1]}";
}
