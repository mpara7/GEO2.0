using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作垂直线")]
public class MakeVerti : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeVerti(Point point1, Line line, Line rLine)
    {
        Add(point1, line, rLine);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}作{Properties[1]}的垂直线{Properties[2]}";

    public override void Normalize()
    {
    }
}
