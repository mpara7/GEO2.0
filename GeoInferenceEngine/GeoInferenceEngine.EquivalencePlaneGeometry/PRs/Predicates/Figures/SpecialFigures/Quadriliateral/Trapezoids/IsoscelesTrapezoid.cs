namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("等腰梯形")]
/// <summary>
/// 梯形
/// </summary>
public class IsoscelesTrapezoid : PlaneFigure
{
    /// <summary>
    /// 梯形
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    public IsoscelesTrapezoid(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"等腰梯形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }

}
