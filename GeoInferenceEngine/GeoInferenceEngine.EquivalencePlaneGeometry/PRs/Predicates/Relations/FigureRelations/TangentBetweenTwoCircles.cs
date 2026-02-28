using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("两圆相切", "两圆的切点")]

public class TangentBetweenTwoCircles : Knowledge
{
    public TangentBetweenTwoCircles(Point point, Circle circle1, Circle circle2)
    {
        Add(point, circle1, circle2);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[1]}相切{Properties[2]}于{Properties[0]}";

    public override void Normalize()
    {

    }
}
