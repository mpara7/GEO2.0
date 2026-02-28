using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作圆外一点切线")]
public class MakePointCircletTangent : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakePointCircletTangent(Circle circle, Point point, Point rPoint1, Point rPoint2, Segment rSegment1, Segment rSegment2)
    {
        Add(circle, point, rPoint1, rPoint2, rSegment1, rSegment2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}上{Properties[1]}的切线{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
    }
}
