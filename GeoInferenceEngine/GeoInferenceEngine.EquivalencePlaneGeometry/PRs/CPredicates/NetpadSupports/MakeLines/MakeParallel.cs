using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作平行线")]
public class MakeParallel : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeParallel(Point point1, Line line, Line rLine)
    {
        Add(point1, line, rLine);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}作{Properties[1]}的平行线{Properties[2]}";

    public override void Normalize()
    {
    }
}
