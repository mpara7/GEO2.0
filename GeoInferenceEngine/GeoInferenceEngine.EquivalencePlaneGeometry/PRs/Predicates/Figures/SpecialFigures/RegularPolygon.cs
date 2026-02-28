namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("正多边形")]
public class RegularPolygon : PlaneFigure
{
    /// <summary>
    /// 正多边形
    /// </summary>
    /// <param name="points"></param>
    public RegularPolygon(Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"正多边形{StringTool.ComposeList(Properties, "")}";

    public override void Normalize()
    {
        Sort();
    }

}
