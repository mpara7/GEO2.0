using GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules
{
    [PlaneRuleType(PlaneRuleType.Constructive)]
    internal class MakeArcRules : PlaneRuleClass
    {
        public void RuleCCP001作三点弧(MakeThreePointArc makeArc)
        {
            Arc arc = (Arc)makeArc[3];
            arc.AddReason();
            arc.AddCondition(makeArc);
        }
    }
}
