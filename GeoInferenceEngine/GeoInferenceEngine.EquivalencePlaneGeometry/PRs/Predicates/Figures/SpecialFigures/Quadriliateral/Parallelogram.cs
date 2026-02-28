namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("平行四边形")]
/// <summary>
/// 平行四边形
/// </summary>
public class Parallelogram : PlaneFigure
{

    public Parallelogram(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"平行四边形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
