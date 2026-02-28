using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作平行线上点")]
public class MakeOnPLinePoint : ConstructiveKnowledge
{
    /// <summary>
    /// 更靠近p
    /// </summary>
    /// <param name="points"></param>
    public MakeOnPLinePoint(Point point1, Line parallel, Point rPoint, Segment rSeg)
    {
        Add(point1, parallel, rPoint, rSeg);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}与{Properties[1]}平行的点{Properties[2]}";

    public override void Normalize()
    {
    }
}
