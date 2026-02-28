namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[PrimitiveKnowledge]
[Description("风筝")]
/// <summary>
/// 四边形
/// </summary>
public class Kite : PlaneFigure
{
    /// <summary>
    ///   p2
    /// p1与   p3
    ///   p4
    /// </summary>
    public Kite(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"风筝形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        Sort(0, 2);
    }
}
