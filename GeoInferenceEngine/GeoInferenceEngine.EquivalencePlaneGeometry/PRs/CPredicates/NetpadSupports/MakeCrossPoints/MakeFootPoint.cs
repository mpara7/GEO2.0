using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作垂足")]
public class MakeFootPoint : ConstructiveKnowledge
{
    /// <summary>
    /// 作垂足
    /// </summary>
    /// <param name="line1">垂直参照线</param>
    /// <param name="point">过点</param>
    /// <param name="rPoint">垂足</param>
    public MakeFootPoint(Point point, Segment line1, Point rPoint, Segment rSeg)
    {
        Add(point, line1, rPoint, rSeg);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}，{Properties[1]}的垂足{Properties[2]},垂线是{Properties[3]}";

    public override void Normalize()
    {
        Sort(1, 2);
    }
}
