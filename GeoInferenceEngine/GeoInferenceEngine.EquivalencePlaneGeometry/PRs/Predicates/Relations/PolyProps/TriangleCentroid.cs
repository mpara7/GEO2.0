using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("三角形重心", "三角形的重心")]

public class TriangleCentroid : Knowledge
{
    public TriangleCentroid(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[1]}的重心是{Properties[0]}";
    public override void Normalize()
    {

    }
}
