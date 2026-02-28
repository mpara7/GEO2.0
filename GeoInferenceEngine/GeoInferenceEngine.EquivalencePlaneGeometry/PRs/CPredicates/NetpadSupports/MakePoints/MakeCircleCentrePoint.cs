using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作圆心")]
public class MakeCircleCentrePoint : ConstructiveKnowledge
{
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeCircleCentrePoint(Circle segment, Point rPoint)
    {
        Add(segment, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}的中点{Properties[1]}";

    public override void Normalize()
    {
        Sort();
    }
}
