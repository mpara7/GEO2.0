using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作点对称点")]
public class MakeCentroSymmetricalPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeCentroSymmetricalPoint(Point point, Point symmetrical, Point rPoint)
    {
        Add(point, symmetrical, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}关于{Properties[1]}的对称点{Properties[2]}";

    public override void Normalize()
    {
        Sort();
    }
}
