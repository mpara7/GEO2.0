using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作垂直平分线上的点")]
public class MakePointOnPerpendicularBisector : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakePointOnPerpendicularBisector(Segment segment, Point rPoint)
    {
        Add(segment, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}的垂直平分线上的点{Properties[1]}";

    public override void Normalize()
    {
    }
}
