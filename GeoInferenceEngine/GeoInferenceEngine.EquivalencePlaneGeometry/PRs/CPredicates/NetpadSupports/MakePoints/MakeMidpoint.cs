using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作中点")]
public class MakeMidpoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeMidpoint(Point endPoint1, Point endPoint2, Point rPoint)
    {
        Add(endPoint1, endPoint2, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}的中点{Properties[1]}";

    public override void Normalize()
    {
        Sort(0,1);
    }
}
