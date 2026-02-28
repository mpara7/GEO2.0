using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

/// <summary>
/// 直线的交点
/// </summary>
[Description("LineIntersection ")]

public class LineIntersection : Knowledge
{

    public LineIntersection(Point point, Segment line1, Segment line2)
    {
        Add(point, line1, line2);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[0]} is the intersection of {Properties[1]} and {Properties[2]}";

    public override void Normalize()
    {
        Sort(1, 2);
    }
}
