using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作切线")]
public class MakeCircleTangent : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeCircleTangent(Circle circle, Point point, Point rPoint, Segment rSegment)
    {
        Add(circle, point, rPoint, rSegment);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}上{Properties[1]}的切线{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
    }
}
