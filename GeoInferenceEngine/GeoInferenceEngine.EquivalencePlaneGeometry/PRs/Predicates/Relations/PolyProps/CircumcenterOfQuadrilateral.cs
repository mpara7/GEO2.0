using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("四边形外接圆", "四边形的外接圆")]

public class CircumcenterOfQuadrilateral : Knowledge
{
    public CircumcenterOfQuadrilateral(Quadriliateral quadriliateral, Circle circle)
    {
        Add(circle, quadriliateral);
        Normalize();
        SetHashCode();

    }

    public override string ToString() => $"{Properties[1]}是{Properties[0]}的外接圆";

    public override void Normalize()
    {

    }
}
