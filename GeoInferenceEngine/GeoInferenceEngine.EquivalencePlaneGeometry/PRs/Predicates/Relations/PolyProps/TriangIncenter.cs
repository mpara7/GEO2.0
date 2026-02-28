using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("三角形内心", "三角形的内心")]

public class TriangleIncenter : Knowledge
{
    public TriangleIncenter(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => $"{Properties[1]}的内心是{Properties[0]}";

    public override void Normalize()
    {

    }
}