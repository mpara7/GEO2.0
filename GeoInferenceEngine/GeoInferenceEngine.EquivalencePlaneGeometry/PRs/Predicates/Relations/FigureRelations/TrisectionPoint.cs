using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("三等分点")]
public class TrisectionPoint : Knowledge
{
    /// <summary>
    /// 三等分点关系
    /// </summary>
    /// <param name="p1">三等分点</param>
    /// <param name="p2">三等分点</param>
    /// <param name="p3">p1靠近的点</param>
    ///  <param name="p4">p2靠近的点</param>
    public TrisectionPoint(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"{Properties[0]}和{Properties[1]}是线段{Properties[2]}{Properties[3]}的三等分点";

    public override void Normalize()
    {
        //Sort(1, 2);
    }
}