namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("等边三角形", "正三角形")]
/// <summary>
/// 等边三角形
/// </summary>
public class EquilateralTriangle : PlaneFigure
{
    /// <summary>
    /// 等边三角形
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public EquilateralTriangle(Point p1, Point p2, Point p3)
    {
        Add(p1, p2, p3);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"等边三角形{Properties[0]}{Properties[1]}{Properties[2]}";

    public override void Normalize()
    {
        Sort();
    }
}
