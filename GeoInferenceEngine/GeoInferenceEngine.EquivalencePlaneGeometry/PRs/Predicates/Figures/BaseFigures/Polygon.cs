namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[PrimitiveKnowledge]
[Description("多边形")]
public class Polygon : PlaneFigure
{
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
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public Polygon(params Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"多边形{StringTool.ComposeList(Properties, "")}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
