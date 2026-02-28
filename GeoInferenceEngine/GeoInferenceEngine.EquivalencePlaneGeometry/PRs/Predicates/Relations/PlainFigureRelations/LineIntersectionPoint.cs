using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

/// <summary>
/// 直线的交点
/// </summary>
[Description("Intersect", "线段的交点", "LineIntersectionl")]

public class LineIntersectionPoint : Knowledge
{

    public LineIntersectionPoint(Point point, Line line1, Line line2)
    {
        Add(point, line1, line2);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $" {Properties[1]} 和 {Properties[2]} 的交点为 {Properties[0]}";

    public override void Normalize()
    {
        Sort(1, 2);
    }
}
