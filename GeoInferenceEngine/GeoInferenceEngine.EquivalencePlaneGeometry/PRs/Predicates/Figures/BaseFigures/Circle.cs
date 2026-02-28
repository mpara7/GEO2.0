namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
/// <summary>
/// 圆
/// </summary>
[Description("圆")]
[PrimitiveKnowledge]
public class Circle : PlaneFigure
{
    GeoProp diameter = null;
    public GeoProp Diameter
    {
        get
        {
            if (diameter is null)
            {
                diameter = new GeoProp(this, GeoProp.Diameter);
            }
            return diameter;
        }
    }
    GeoProp radius = null;
    public GeoProp Radius
    {
        get
        {
            if (radius is null)
            {
                radius = new GeoProp(this, GeoProp.Radius);
            }
            return radius;
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
    GeoProp area = null;
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

    /// <summary>
    /// n>1 1为圆心
    /// </summary>
    /// <param name="points"></param>
    public Circle(Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }

    public override string ToString()
    {
        var last = StringTool.ComposeList(Properties.Skip(1));
        return $"圆形{Properties[0]}-{last}";
    }
    public override void Normalize()
    {

    }
}