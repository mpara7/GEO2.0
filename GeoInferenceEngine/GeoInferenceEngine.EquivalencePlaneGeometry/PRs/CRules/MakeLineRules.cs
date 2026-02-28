using EmptyBlazorApp1.CKnowledges;
using GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
using GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeAngle;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules
{
    [PlaneRuleType(PlaneRuleType.Constructive)]
    internal class MakeLineRules : PlaneRuleClass
    {
        public void RuleCL001作直线(MakeSegment makeLine)
        {
            Segment pred = new Segment((Point)makeLine[0], (Point)makeLine[1]);
            pred.AddReason();
            pred.AddCondition(makeLine);
            AddProcessor.Add(pred);
            Line pred2 = new Line((Point)makeLine[0], (Point)makeLine[1]);
            pred2.AddReason();
            pred2.AddCondition(makeLine);
            AddProcessor.Add(pred2);
        }
        public void RuleCL002作角平分线(MakeAngleBisector makeLine)
        {
            PointOnAngularBisector pred = new PointOnAngularBisector((Angle)makeLine[0], (Point)makeLine[0][1], (Point)makeLine[1]);
            pred.AddReason();
            pred.AddCondition(makeLine);
            AddProcessor.Add(pred);
        }
        public void RuleCL003作垂直线(MakeVerti makeLine)
        {
            LinePerpendicular pred = new LinePerpendicular((Line)makeLine[1], (Line)makeLine[2]);
            pred.AddReason();
            pred.AddCondition(makeLine);
            AddProcessor.Add(pred);
        }
        public void RuleCL004作平行线(MakeParallel makeLine)
        {
            LineParallel pred = new LineParallel((Line)makeLine[1], (Line)makeLine[2]);
            pred.AddReason();
            pred.AddCondition(makeLine);
            AddProcessor.Add(pred);
        }
        public void RuleCCP005作切线(MakeTangentPoint makeLine)
        {
        }
    }
}
