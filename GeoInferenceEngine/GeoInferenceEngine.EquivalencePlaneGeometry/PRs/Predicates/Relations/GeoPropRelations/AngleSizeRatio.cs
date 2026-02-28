using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("角大小的比值", "角大小比值", "角比值")]
/// <summary>
/// class SegmentLengthRatio: 线段长度比值
/// </summary>
public class AngleSizeRatio : Knowledge
{
    public Angle Angle1 { get => (Angle)Properties[0]; }
    public Angle Angle2 { get => (Angle)Properties[1]; }
    // <summary>
    /// 构造函数
    /// </summary>
    public AngleSizeRatio(Angle angle1, Angle angle2, Expr expr)
    {
        Add(angle1, angle2, expr);
        Normalize();
        SetHashCode();
    }


    public override string ToString() => $"{Properties[0]}与{Properties[1]}的比值是{Expr}";

    public override void Normalize()
    {
        if (Properties[0].PosIndex > Properties[1].PosIndex)
        {
            var temp = Properties[0];
            Properties[0] = Properties[1];
            Properties[1] = temp;
            Expr = Expr.Invert();
        }
    }

}
