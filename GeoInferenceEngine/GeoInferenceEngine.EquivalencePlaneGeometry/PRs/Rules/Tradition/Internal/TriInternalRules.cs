namespace GeoInferenceEngine.PlaneKnowledges.PRs.Rules.Internal
{
    [PlaneRuleType(PlaneRuleType.Internal)]
    internal class TriInternalRules : PlaneRuleClass
    {
        public void RuleTi001等边三角形是等腰三角形(EquilateralTriangle tri)
        {
            IsoscelesTriangle pred = new IsoscelesTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
            pred.AddReason();
            pred.AddCondition(tri);
            AddProcessor.Add(pred);
            pred = new IsoscelesTriangle((Point)tri[1], (Point)tri[2], (Point)tri[0]);
            pred.AddReason();
            pred.AddCondition(tri);
            AddProcessor.Add(pred);
            pred = new IsoscelesTriangle((Point)tri[2], (Point)tri[0], (Point)tri[1]);
            pred.AddReason();
            pred.AddCondition(tri);
            AddProcessor.Add(pred);
        }
        public void RuleTi002等腰直角三角形是等腰三角形和直角三角形(IsoscelesRightTriangle tri)
        {
            IsoscelesTriangle pred = new IsoscelesTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
            pred.AddReason();
            pred.AddCondition(tri);
            AddProcessor.Add(pred);
            RightTriangle pred2 = new RightTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
            pred2.AddReason();
            pred2.AddCondition(tri);
            AddProcessor.Add(pred2);
        }
        public void RuleTi003等腰三角形是三角形(IsoscelesTriangle tri)
        {
            Triangle pred = new Triangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
            pred.AddReason();
            pred.AddCondition(tri);
            AddProcessor.Add(pred);
        }
        public void RuleTi004直角三角形是三角形(RightTriangle tri)
        {
            Triangle pred = new Triangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
            pred.AddReason();
            pred.AddCondition(tri);
            AddProcessor.Add(pred);
        }

    }
}
