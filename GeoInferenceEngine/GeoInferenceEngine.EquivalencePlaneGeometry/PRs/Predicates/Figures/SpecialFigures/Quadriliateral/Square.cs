namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("正方形")]
/// <summary>
/// 正方形
/// </summary>
public class Square : PlaneFigure
{

    public Square(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"正方形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
