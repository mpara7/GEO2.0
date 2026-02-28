using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作垂直线上点")]
public class MakeOnVLinePoint : ConstructiveKnowledge
{
    /// <summary>
    /// 更靠近p
    /// </summary>
    /// <param name="points"></param>
    public MakeOnVLinePoint(Point point1, Line perpendicular, Point rPoint, Line rSeg)
    {
        Add(point1, perpendicular, rPoint, rSeg);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}与{Properties[1]}垂直的点{Properties[2]}";

    public override void Normalize()
    {
    }
}
