using EmptyBlazorApp1.CKnowledges;

using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules;
[PlaneRuleType(PlaneRuleType.Constructive)]
internal class MakePointRules : PlaneRuleClass
{
    public void RuleCP001作自由点(MakeFreePoint makeFreePoint)
    {
        var point = KnowledgeGetter.GetPoint(makeFreePoint.Name);
        point.AddReason();
        point.AddCondition(makeFreePoint);
        AddProcessor.Add(point);
    }
    public void RuleCP002作中点(MakeMidpoint makeMidpoint)
    {
        Midpoint pred = new Midpoint((Point)makeMidpoint[2], (Point)makeMidpoint[0], (Point)makeMidpoint[1]);
        pred.AddReason();
        pred.AddCondition(makeMidpoint);
        AddProcessor.Add(pred);
    }
    public void RuleCP003作圆心(MakeCircleCentrePoint makePoint)
    {
        Circle circle = (Circle)makePoint[0];
        Point center = (Point)makePoint[1];
        circle.Properties[0] = center;
    }
}
