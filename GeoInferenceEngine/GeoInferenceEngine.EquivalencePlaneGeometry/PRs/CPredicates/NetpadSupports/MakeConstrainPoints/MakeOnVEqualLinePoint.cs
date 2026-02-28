using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作垂直且相等的点")]
public class MakeOnVEqualLinePoint : ConstructiveKnowledge
{
    /// <summary>
    /// 更靠近p
    /// </summary>
    /// <param name="points"></param>
    public MakeOnVEqualLinePoint(Point point1, Line perpendicular, Point rPoint, Segment rSeg)
    {
        Add(point1, perpendicular, rPoint, rSeg);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}与{Properties[1]}垂直且相等的点{Properties[2]}与连接线{Properties[3]}";

    public override void Normalize()
    {
    }
}
