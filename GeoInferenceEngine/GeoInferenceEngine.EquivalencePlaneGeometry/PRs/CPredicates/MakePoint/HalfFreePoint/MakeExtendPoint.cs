using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
[Description("作延长线上的点")]
public class MakeExtendPoint : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeExtendPoint(Point point1, Point point2, Point rPoint)
    {
        Add(point1, point2, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}{Properties[1]}延长线(有方向)上的一点{Properties[2]}";

    public override void Normalize()
    {
    }
}
