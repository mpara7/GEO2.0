using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作圆上一点", "作圆上的点", "作点在圆上")]
public class MakePointOnCircle : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakePointOnCircle(Circle circle, Point rPoint)
    {
        Add(circle, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}上的一点{Properties[1]}";

    public override void Normalize()
    {
    }
}
