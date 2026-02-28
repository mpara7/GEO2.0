using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("Midpoint ")]
public class Midpoint : Knowledge
{
    public Point Point { get => (Point)Properties[0]; }
    public Point EndPoint1 { get => (Point)Properties[1]; }
    public Point EndPoint2 { get => (Point)Properties[2]; }
    /// <summary>
    /// 中点关系
    /// </summary>
    /// <param name="p1">中点</param>
    /// <param name="p2">端点</param>
    /// <param name="p3">端点</param>
    public Midpoint(Point p1, Point endPoint1, Point endPoint2)
    {
        Add(p1, endPoint1, endPoint2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"{Properties[0]}是{Properties[1]}{Properties[2]}的中点";

    public override void Normalize()
    {

    }
}
