using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作线段与圆的交点")]
public class MakeSegmentCircleInterectionPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeSegmentCircleInterectionPoint(Segment segment, Circle circle, Point rPoint)
    {
        Add(segment, circle, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]},{Properties[1]}的交点{Properties[2]}";

    public override void Normalize()
    {
        Sort();
    }
}
