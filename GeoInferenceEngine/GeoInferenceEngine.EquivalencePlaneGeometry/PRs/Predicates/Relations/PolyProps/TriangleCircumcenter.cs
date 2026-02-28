using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("三角形外心", "三角形的外心")]

public class TriangleCircumcenter : Knowledge
{
    public TriangleCircumcenter(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => $"{Properties[1]}的外心是{Properties[0]}";

    public override void Normalize()
    {

    }
}