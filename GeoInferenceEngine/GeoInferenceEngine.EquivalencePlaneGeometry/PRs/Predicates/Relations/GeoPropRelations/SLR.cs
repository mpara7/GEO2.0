using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("Ratio")]
/// <summary>
/// class SegmentLengthRatio: 共线线段长度比值
/// </summary>
public class SLR : Knowledge
{
    public Point point1 { get => (Point)Properties[0]; }
    public Point point2 { get => (Point)Properties[1]; }
    public Point point3 { get => (Point)Properties[2]; }
    

    
    // <summary>
    /// 构造函数
    /// </summary>
    public SLR(Point p1, Point i1, Point p2, Expr expr)
    {
        Add(p1,i1,p2,expr);
        //Debug.Assert(!ToString().Contains("CD与AD的比"));
        //Normalize();
        SetHashCode();

    }


    public override string ToString() => $"{Properties[0]}{Properties[1]} / {Properties[1]}{Properties[2]} 的值为 {Expr}";
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
