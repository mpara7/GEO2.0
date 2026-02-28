using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作弧上一点")]
public class MakePointOnArc : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakePointOnArc(Arc arc, Point rPoint)
    {
        Add(arc, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}上的一点{Properties[1]}";

    public override void Normalize()
    {
    }
}
