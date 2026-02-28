using EmptyBlazorApp1.CKnowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules
{
    [PlaneRuleType(PlaneRuleType.Constructive)]
    internal class MakeHalfConstrainPointRules : PlaneRuleClass
    {
        public void RuleCCP001作点在线上(MakePointOnSegment makePoint)
        {
            PointWithInPoints pred = new((Point)makePoint[1], (Point)makePoint[0][0], (Point)makePoint[0][1]);
            pred.AddReason();
            pred.AddCondition(makePoint);
            AddProcessor.Add(pred);
        }
        public void RuleCCP002作点在圆上(MakePointOnCircle makePoint)
        {
            Circle circle = (Circle)makePoint[0];
            circle.Properties.Add(makePoint[1]);
        }
        public void RuleCCP003作垂直线上点(MakeOnVLinePoint makePoint)
        {

        }
        public void RuleCCP004作平行线上点(MakeOnPLinePoint makePoint)
        {
        }
        public void RuleCCP003作垂直线上点(MakeOnVEqualLinePoint makePoint)
        {
        }
        public void RuleCCP004作平行线上点(MakeOnPEqualLinePoint makePoint)
        {
        }
        public void RuleCCP004作平行线上点(MakeTangentPoint makePoint)
        {
        }
    }
}
