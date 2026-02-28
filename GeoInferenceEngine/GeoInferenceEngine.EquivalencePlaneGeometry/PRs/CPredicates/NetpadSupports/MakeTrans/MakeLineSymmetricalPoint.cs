using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作线对称点")]
public class MakeLineSymmetricalPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeLineSymmetricalPoint(Point point1, Segment segment, Point rPoint)
    {
        Add(point1, segment, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}关于{Properties[1]}的对称点{Properties[2]}";

    public override void Normalize()
    {
    }
}
