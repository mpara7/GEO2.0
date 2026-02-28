using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules
{
    [PlaneRuleType(PlaneRuleType.Constructive)]
    internal class MakeCircleRules : PlaneRuleClass
    {
        public void RuleCCP001作两点圆(MakeTwoPointCircle makeCircle)
        {
            Circle circle = (Circle)makeCircle[2];
            circle.AddReason();
            circle.AddCondition(makeCircle);
        }
        public void RuleCCP001作三点圆(MakeThreePointCircle makeCircle)
        {
            Circle circle = (Circle)makeCircle[3];
            circle.AddReason();
            circle.AddCondition(makeCircle);
        }
        public void RuleCCP001作点径圆(MakePointRCircle makeCircle)
        {
            Circle circle = (Circle)makeCircle[1];
            circle.AddReason();
            circle.AddCondition(makeCircle);
            //GeoEquation geoEquation = new GeoEquation(circle.Diameter, makeCircle.Expr);
            //geoEquation.AddReason();
            //geoEquation.AddCondition(makeCircle);
            //AddProcessor.Add(geoEquation);
        }
    }
}
