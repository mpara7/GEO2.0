using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("ratio", "线段长度比值", "线段长度比例", "线段比值", "线段比例" )]
/// <summary>
/// class SegmentLengthRatio: 线段长度比值
/// </summary>
public class SegmentLengthRatio : Knowledge
{
    public Segment Seg1 { get => (Segment)Properties[0]; }

    public Segment Seg2 { get => (Segment)Properties[1]; }

    
    // <summary>
    /// 构造函数
    /// </summary>
    public SegmentLengthRatio(Segment segment1, Segment segment2, Expr expr)
    {
        Add(segment1, segment2, expr);
        //Debug.Assert(!ToString().Contains("CD与AD的比"));
        Normalize();
        SetHashCode();

    }



    public override string ToString() => $"{Properties[0]} / {Properties[1]} 的值为 {Expr}";
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
