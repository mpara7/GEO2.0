using EmptyBlazorApp1.CKnowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules;
[PlaneRuleType(PlaneRuleType.Constructive)]
internal class MakeTransPointRules : PlaneRuleClass
{
    public void RuleCP001作旋转点(MakeRotatedPoint makePoint)
    {
        var angle = new Angle([(Point)makePoint[0]], (Point)makePoint[1], [(Point)makePoint[2]]);
        angle = (Angle)AddProcessor.Add(angle);
        var expr = makePoint.Expr.Clone();
        if (makePoint.Expr.CompareTo(Expr.Zero) == ExprCompareResult.Less)
        {
            expr = expr.Opposite();
        }
        AngleSize pred = new AngleSize(angle, expr);
        pred.AddReason();
        pred.AddCondition(makePoint);
        AddProcessor.Add(pred);
    }
    public void RuleCP002作中心对称点(MakeCentroSymmetricalPoint makePoint)
    {
        //Segment segment = new Segment((Point)makePoint[0], (Point)makePoint[2]);
        //segment.AddReason();
        //segment.AddCondition(makePoint);
        //AddProcessor.Add(segment);
        Midpoint pred = new Midpoint((Point)makePoint[1], (Point)makePoint[0], (Point)makePoint[2]);
        pred.AddReason();
        pred.AddCondition(makePoint);
        AddProcessor.Add(pred);
    }
    public void RuleCP003作轴对称点(MakeLineSymmetricalPoint makePoint)
    {
        Segment segment = new Segment((Point)makePoint[0], (Point)makePoint[2]);
        segment.AddReason();
        segment.AddCondition(makePoint);
        segment = (Segment)AddProcessor.Add(segment);
        PointOnVerticalBisector pred = new PointOnVerticalBisector(segment, (Point)makePoint[1][0], (Point)makePoint[1][1]);
        pred.AddReason();
        pred.AddCondition(makePoint);
        AddProcessor.Add(pred);
    }
}
