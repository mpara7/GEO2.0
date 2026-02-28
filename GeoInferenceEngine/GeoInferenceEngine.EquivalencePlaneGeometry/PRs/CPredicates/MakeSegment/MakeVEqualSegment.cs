using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作垂直且相等的线")]
public class MakeVEqualSegment : ConstructiveKnowledge
{
    public int Direct { get; set; }
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeVEqualSegment(Point point1, Segment segment, int direct, Point rPoint, Segment rSegment)
    {
        Direct = direct;
        Add(point1, segment, rPoint, rSegment);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作过{Properties[0]}与{Properties[1]}的垂直且相等的点";

    public override void SetHashCode()
    {
        base.SetHashCode();
        if (Direct == 1) { HashCode++; }
    }
    public override void Normalize()
    {
    }
}
