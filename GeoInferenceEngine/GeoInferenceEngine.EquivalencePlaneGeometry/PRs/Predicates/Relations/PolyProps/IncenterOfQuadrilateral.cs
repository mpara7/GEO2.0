using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("四边形内接圆", "四边形的内接圆")]
public class IncenterOfQuadrilateral : Knowledge
{
    public IncenterOfQuadrilateral(Quadriliateral quadriliateral, Circle circle)
    {
        Add(circle, quadriliateral);
        Normalize();
        SetHashCode();

    }

    public override string ToString() => $"{Properties[1]}是{Properties[0]}的内接圆";

    public override void Normalize()
    {

    }
}
