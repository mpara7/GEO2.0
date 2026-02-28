using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("弧的三等分点")]
public class TrisectionArc : Knowledge
{
    /// <summary>
    /// 中点关系
    /// </summary>
    /// <param name="p1">中点</param>
    /// <param name="p2">端点</param>
    /// <param name="p3">端点</param>
    public TrisectionArc(Point point, Point endPoint1, Point endPoint2)
    {
        Add(point, endPoint1, endPoint2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"{Properties[0]}是{Properties[1]}{Properties[2]}弧的中点";

    public override void Normalize()
    {

    }
}
