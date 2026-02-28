using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作两圆的交点")]
public class MakeCircleInterectionPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeCircleInterectionPoint(Circle line1, Circle line2, Point rPoint1, Point rPoint2)
    {
        Add(line1, line2, rPoint1, rPoint2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}{Properties[1]}的交点{Properties[2]},{Properties[3]}";

    public override void Normalize()
    {
        Sort();
    }
}
