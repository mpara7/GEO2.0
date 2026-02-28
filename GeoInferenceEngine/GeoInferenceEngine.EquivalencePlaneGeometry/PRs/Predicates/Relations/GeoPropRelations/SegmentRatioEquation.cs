using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("ratio equation")]
/// <summary>
/// class SegmentLengthRatio: 线段长度比值
/// </summary>
public class SegmentRatioEquation : Knowledge
{
    public SegmentLengthRatio SegLR1 { get => (SegmentLengthRatio)Properties[0]; }
    public SegmentLengthRatio SegLR2 { get => (SegmentLengthRatio)Properties[1]; }
    public SegmentLengthRatio SegLR3 { get => (SegmentLengthRatio)Properties[2]; }

    // <summary>
    /// 构造函数
    /// </summary>
    public SegmentRatioEquation(SegmentLengthRatio s1, SegmentLengthRatio s2, SegmentLengthRatio s3)
    {
        Add(s1,s2,s3);
        //Debug.Assert(!ToString().Contains("CD与AD的比"));
        //Normalize();
        SetHashCode();

    }


    public override string ToString() => $"({Properties[0].Properties[0]}/{Properties[0].Properties[1]})*({Properties[1].Properties[0]}/{Properties[1].Properties[1]})*({Properties[2].Properties[0]}/{Properties[2].Properties[1]})= 1";

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
