using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作切点")]
public class MakeTangentPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 更靠近p
    /// </summary>
    /// <param name="points"></param>
    public MakeTangentPoint(Point point1, Circle circle, Point rPoint)
    {
        Add(point1, circle, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}与{Properties[1]}的切点{Properties[2]}";

    public override void Normalize()
    {
    }
}
