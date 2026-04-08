namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[OrdinaryKnowledge]
[Description("点在圆上")]



public class PointOnCircle : PlainRelation
{
    public PointOnCircle(Circle circle, params Point[] points)
    {
        Add(circle);
        foreach (Point point in points)
        {
            Add(point);
        }
        Normalize();
        SetHashCode();
    }

    public Circle Circle => (Circle)Properties[0];

    public List<Point> Points => Properties
        .Skip(1)
        .Cast<Point>()
        .ToList();

    public override string ToString()
    {
        return $"{string.Join("、", Points)}在{Circle}上";
    }

    public override void Normalize()
    {
        Circle circle = (Circle)Properties[0];

        List<Point> points = Properties
            .Skip(1)
            .Cast<Point>()
            .Distinct()
            .OrderBy(p => p.HashCode)
            .ToList();

        Properties.Clear();
        Add(circle);
        foreach (Point point in points)
        {
            Add(point);
        }
    }
}




    //public PointOnCircle(Point point, Circle circle)
    //{
    //    Add(point, circle);
    //    Normalize();
    //    SetHashCode();
    //}

    //public Point Point => (Point)Properties[0];
    //public Circle Circle => (Circle)Properties[1];


    //public override string ToString() => $"{Properties[0]}在{Properties[1]}上";

    //public override void Normalize()
    //{
    //}

