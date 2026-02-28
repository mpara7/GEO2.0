using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("线与圆相切", "切线")]

public class Tangent : Knowledge
{
    public Tangent(Line line, Circle circle)
    {
        Add(line, circle);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
    }
    public override string ToString()
    {
        return $"{Properties[0]}是{Properties[1]}的切线";
    }
}
