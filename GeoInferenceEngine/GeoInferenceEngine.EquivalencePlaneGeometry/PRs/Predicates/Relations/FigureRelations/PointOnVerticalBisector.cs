using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("垂直平分线")]

public class PointOnVerticalBisector : Knowledge
{
    public List<Point> PointOnBisector { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="segment">平分别的线段的线段 </param>
    /// <param name="segment1">被平分的线段</param>
    public PointOnVerticalBisector(Segment segment, params Point[] points)
    {
        PointOnBisector = points.ToList();
        Add(segment);
        Add(points);
        Normalize();
        SetHashCode();
    }

    public override string ToString()
    {
        return $"{Properties.Skip(1).Select(k => k.ToString()).Aggregate((last, current) => { return last + current; })}在{Properties[0]}的垂直平分线上";
    }



    public override void Normalize()
    {

    }
}