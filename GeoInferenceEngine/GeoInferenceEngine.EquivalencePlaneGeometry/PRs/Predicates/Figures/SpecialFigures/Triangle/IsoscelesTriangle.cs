namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("等腰三角形")]
/// <summary>
/// 等腰三角形
/// </summary>
public class IsoscelesTriangle : PlaneFigure
{
    /// <summary>
    /// 等腰三角形
    /// </summary>
    /// <param name="p1">顶点</param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public IsoscelesTriangle(Point p1, Point p2, Point p3)
    {
        Add(p1, p2, p3);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"等腰三角形{Properties[0]}{Properties[1]}{Properties[2]}";

    public override void Normalize()
    {
        if (Properties[1].PosIndex > Properties[2].PosIndex)
        {
            var temp = Properties[1];
            Properties[1] = Properties[2];
            Properties[2] = temp;
        }
    }

}
