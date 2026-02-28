namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("扇形")]
[PrimitiveKnowledge]
public class Sector : PlaneFigure
{
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
    public Sector(Point p1, Point point2, Point point3)
    {
        Add(p1, point2, point3);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"扇形{Properties[0]}";
    public override void Normalize()
    {

    }
}

