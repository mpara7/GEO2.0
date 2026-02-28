namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[PrimitiveKnowledge]
[Description("四边形")]
/// <summary>
/// 四边形
/// </summary>
public class Quadriliateral : PlaneFigure
{
    GeoProp area;
    public GeoProp Area
    {
        get
        {
            if (area is null)
            {
                area = new GeoProp(this, GeoProp.Area);
            }
            return area;
        }
    }
    GeoProp perimeter = null;
    public GeoProp Perimeter
    {
        get
        {
            if (perimeter is null)
            {
                perimeter = new GeoProp(this, GeoProp.Perimeter);
            }
            return perimeter;
        }
    }
    /// <summary>
    /// 四边形
    /// </summary>
    public Quadriliateral(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"四边形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
