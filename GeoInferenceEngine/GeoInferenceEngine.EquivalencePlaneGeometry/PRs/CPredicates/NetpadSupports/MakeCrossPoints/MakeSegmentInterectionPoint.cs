using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作线段交点", "作线段的交点")]
public class MakeSegmentInterectionPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeSegmentInterectionPoint(Line line1, Line line2, Point rPoint)
    {
        Add(line1, line2, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}，{Properties[1]}的交点{Properties[2]}";

    public override void Normalize()
    {
        Sort(1, 2);
    }
}
