namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("Triangle ")]
public class Triangle : PlaneFigure
{
    /// <summary>
    /// 三角形
    /// </summary>
    public Triangle(Point p1, Point p2, Point p3)
    {
        Add(p1, p2, p3);
        Normalize();
        SetHashCode();
    }
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
    public override string ToString() => $"三角形{Properties[0]}{Properties[1]}{Properties[2]}";
    public override void Normalize()
    {
        Sort();
    }

}
