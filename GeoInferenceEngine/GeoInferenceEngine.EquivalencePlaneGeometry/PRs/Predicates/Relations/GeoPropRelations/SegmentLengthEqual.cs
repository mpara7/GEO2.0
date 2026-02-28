using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("SegmentLengthEqual ", "线段的长度相等")]
public class SegmentLengthEqual : Knowledge
{

    // <summary>
    /// 构造函数
    /// </summary>
    public SegmentLengthEqual(Segment segment1, Segment segment2)
    {
        Add(segment1, segment2);
        Normalize();
        SetHashCode();
    }
    public Segment Seg1 { get => (Segment)Properties[0]; }

    public Segment Seg2 { get => (Segment)Properties[1]; }
    public override string ToString() => $"{Properties[0]}与{Properties[1]}的长度相等";

    public override void Normalize()
    {
        Sort();
    }
}

[Description("线段长度相除")]
public class SegmentLengthDivision : Knowledge
{
    /// <summary>
    /// segment1/segment2
    /// </summary>
    /// <param name="segment1"></param>
    /// <param name="segment2"></param>
    public SegmentLengthDivision(Segment segment1, Segment segment2)
    {
        Add(segment1, segment2);
        Normalize();
        SetHashCode();
    }

    public override void Normalize()
    {
    }

    public override string ToString() => $"{Properties[0]}/{Properties[1]}";
}

[Description("线段长度等式")]
public class SegmentLengthEquation : Knowledge
{
    public SegmentLengthEquation(Knowledge left, Knowledge right)
    {
        Add(left, right);
        //Normalize();
        SetHashCode();
    }

    public Knowledge Left { get => Properties[0]; set => Properties[0] = value; }
    public Knowledge Right { get => Properties[1]; set => Properties[1] = value; }

    public override void Normalize()
    {
        var compareResult = Left.GetType().Name.CompareTo(Right.GetType().Name);
        if (compareResult == 1)
        {
            Knowledge temp;
            temp = Left;
            Left = Right;
            Right = temp;
        }
        else if (compareResult == 0)
        {
            Sort();
        }
    }

    public override string ToString() => $"{Properties[0]}={Properties[1]}";
}

[Description("连乘表达式")]
public class MultipleExpression : Knowledge
{
    public MultipleExpression(params Knowledge[] knowledges)
    {
        Add(knowledges);
        Normalize();
        SetHashCode();
    }

    public override void Normalize()
    {
        Sort(0);
    }

    public override string ToString()
    {
        String strings = null;

        int count = 1;
        foreach (Knowledge knowledge in Properties)
        {
            //如若不是最后一个元素
            if (count != Properties.Count)
            {
                strings += $"{knowledge}·";
            }
            //处理最后一个
            else
                strings += $"{knowledge}";
        }

        return strings;
    }
}


[Description("加减表达式")]
public class AddExpression : Knowledge
{
    //public class AddExpression()
    //{ 

    //}
    public override void Normalize()
    {

    }


}