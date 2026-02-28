using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("弧中点", "弧的中点")]
public class MidpointArc : Knowledge
{
    /// <summary>
    /// 中点关系
    /// </summary>
    /// <param name="p1">中点</param>
    /// <param name="p2">端点</param>
    /// <param name="p3">端点</param>
    public MidpointArc(Point point, Arc arc)
    {
        Add(point, arc);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"{Properties[0]}是{Properties[1]}弧的中点";

    public override void Normalize()
    {

    }
}
