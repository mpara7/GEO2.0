using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作线上的点", "作点在线上")]
public class MakePointOnSegment : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakePointOnSegment(Segment segment, Point rPoint)
    {
        Add(segment, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}上的一点{Properties[1]}";

    public override void Normalize()
    {
    }
}
