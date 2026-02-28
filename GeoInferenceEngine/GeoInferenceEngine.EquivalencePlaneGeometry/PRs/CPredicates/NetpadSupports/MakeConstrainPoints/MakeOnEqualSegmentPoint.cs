using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作相等线段的点")]
public class MakeOnEqualSegmentPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 更靠近p
    /// </summary>
    /// <param name="points"></param>
    public MakeOnEqualSegmentPoint(Point point1, Segment mock, Point rPoint, Segment rSeg)
    {
        Add(point1, mock, rPoint, rSeg);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作距离{Properties[0]}{Properties[1]}长度相等的{Properties[2]}线{Properties[2]}";

    public override void Normalize()
    {
    }
}
