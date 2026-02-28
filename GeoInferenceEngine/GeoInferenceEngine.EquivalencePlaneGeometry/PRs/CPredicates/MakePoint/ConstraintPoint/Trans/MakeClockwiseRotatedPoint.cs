using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace EmptyBlazorApp1.CKnowledges;
//[Description("作顺时针旋转点")]
public class MakeClockwiseRotatedPoint : ConstructiveKnowledge
{
    public string Direction { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="center">圆心</param>
    /// <param name="point1"></param>
    /// <param name="expr"></param>
    public MakeClockwiseRotatedPoint(Point center, Point point, Expr expr, Point rPoint)
    {
        Add(center, point, expr, rPoint);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[1]}以{Properties[0]}为圆心顺时针旋转{Expr}度的点{Properties[2]}";

    public override void Normalize()
    {
    }
    public override void SetHashCode()
    {
        base.SetHashCode();
        HashCode += (ulong)Expr.GetHashCode();
    }
}
