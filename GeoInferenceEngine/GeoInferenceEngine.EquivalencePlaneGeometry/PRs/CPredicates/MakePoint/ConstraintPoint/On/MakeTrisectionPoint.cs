using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
//[Description("作三等分点")]
public class MakeTrisectionPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 更靠近p
    /// </summary>
    /// <param name="points"></param>
    public MakeTrisectionPoint(Point point1, Point point2, Point rPoint1, Point rPoint2)
    {
        Add(point1, point2, rPoint1, rPoint2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}{Properties[1]}的三等分点{Properties[2]}";

    public override void Normalize()
    {
    }
}
