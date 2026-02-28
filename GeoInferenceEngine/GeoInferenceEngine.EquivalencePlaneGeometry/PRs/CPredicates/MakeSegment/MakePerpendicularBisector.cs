using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作垂直平分线")]
public class MakePerpendicularBisector : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakePerpendicularBisector(Segment segment, Point rPoint1, Point rPoint2, Segment rSegment)
    {
        Add(segment, rPoint1, rPoint2, rSegment);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}的垂直平分线{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
    }
}
