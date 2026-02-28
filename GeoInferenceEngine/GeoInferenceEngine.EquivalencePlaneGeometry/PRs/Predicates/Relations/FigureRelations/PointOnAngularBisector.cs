using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("角平分线")]
public class PointOnAngularBisector : Knowledge
{
    public List<Point> PointOnBisector { get; set; }
    public PointOnAngularBisector(Angle angle, params Point[] points)
    {
        PointOnBisector = new List<Point>(points);
        Add(angle);
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
    }
    public override string ToString()
    {
        return $"{Properties.Skip(1).Select(k => k.ToString()).Aggregate((last, current) => { return last + current; })}在{Properties[0]}的角平分线上";
    }
}
