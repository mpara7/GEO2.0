using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("三角形垂心", "三角形的垂心")]

public class TriangleOrthocenter : Knowledge
{
    public TriangleOrthocenter(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => $"{Properties[1]}的垂心是{Properties[0]}";

    public override void Normalize()
    {

    }
}