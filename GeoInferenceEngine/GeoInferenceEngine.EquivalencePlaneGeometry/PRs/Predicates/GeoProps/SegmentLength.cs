using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("线段的长度", "线段长度")]
/// <summary>
/// class SegmentLength: 线段长度
/// </summary>
public class SegmentLength : Knowledge
{

    public Segment Segment { get => (Segment)Properties[0]; }
    // <summary>
    /// 构造函数
    /// </summary>
    public SegmentLength(Segment segment, Expr expr)
    {
        Add(segment, expr);
        Normalize();
        SetHashCode();

    }

    public override string ToString() => $"{Properties[0]}的长度是{Expr}";

    public override void Normalize()
    {
    }

}
