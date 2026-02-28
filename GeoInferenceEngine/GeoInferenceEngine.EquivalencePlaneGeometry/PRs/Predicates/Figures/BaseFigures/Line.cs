namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("Collinear", "Line ", "共线")]
[PrimitiveKnowledge]
public class Line : Figure
{
    public List<Point> Points { get => Properties.Select(p => (Point)p).ToList(); }
    public Line(params Point[] points)
    {
        if (points.Count() < 2)
        {
            throw new ArgumentException();
        }
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        List<Point> points = new List<Point>(Properties.Select(p => (Point)p));
        if (points[0].X != points[1].X)
        {
            points.Sort((a, b) => a.X.CompareTo(b.X));
        }
        else
        {
            points.Sort((a, b) => a.Y.CompareTo(b.Y));
        }
        Properties.Clear();
        foreach (Point p in points)
        {
            Properties.Add(p);
        }
    }
    public override string ToString() => $"{StringTool.ComposeList(Properties, "")}";

}
