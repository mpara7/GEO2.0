namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("直角三角形")]
/// <summary>
/// 直角三角形
/// </summary>
public class RightTriangle : PlaneFigure
{
    /// <summary>
    /// 直角三角形
    /// </summary>
    /// <param name="p1">直角的</param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public RightTriangle(Point p1, Point p2, Point p3)
    {
        Add(p1, p2, p3);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"直角三角形{Properties[0]}{Properties[1]}{Properties[2]}";

    public override void Normalize()
    {
        this.Sort(1, 2);
    }

}
