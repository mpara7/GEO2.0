namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("直角梯形")]
/// <summary>
/// 梯形
/// </summary>
public class RightTrapezoid : PlaneFigure
{
    /// <summary>
    /// 梯形
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    public RightTrapezoid(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"直角梯形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        if (Properties[0].HashCode > Properties[3].HashCode)
        {
            var temp = Properties[0];
            Properties[0] = Properties[3];
            Properties[3] = temp;
            temp = Properties[1];
            Properties[1] = Properties[2];
            Properties[2] = temp;
        }
    }

}
