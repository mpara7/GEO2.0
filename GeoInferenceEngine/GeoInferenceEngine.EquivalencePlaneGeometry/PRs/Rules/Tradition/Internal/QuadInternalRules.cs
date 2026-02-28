using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.Rules.Internal
{
    [PlaneRuleType(PlaneRuleType.Internal)]
    internal class QuadInternalRules : PlaneRuleClass
    {
        #region 平行四边形系列
        public void RuleQi001正方形是长方形和菱形(Square square)
        {
            Rectangle parallelogram = new Rectangle((Point)square[0], (Point)square[1], (Point)square[2], (Point)square[3]);
            parallelogram.AddReason();
            parallelogram.AddCondition(square);
            AddProcessor.Add(parallelogram);
            Rhombus rhombus = new Rhombus((Point)square[0], (Point)square[1], (Point)square[2], (Point)square[3]);
            rhombus.AddReason();
            rhombus.AddCondition(square);
            AddProcessor.Add(parallelogram);
        }
        public void RuleQi002长方形是平行四边形(Rectangle rect)
        {
            Parallelogram parallelogram = new Parallelogram((Point)rect[0], (Point)rect[1], (Point)rect[2], (Point)rect[3]);
            parallelogram.AddReason();
            parallelogram.AddCondition(rect);
            AddProcessor.Add(parallelogram);
        }
        public void RuleQi003菱形是平行四边形特殊风筝形(Rhombus rhombus)
        {
            Parallelogram parallelogram = new Parallelogram((Point)rhombus[0], (Point)rhombus[1], (Point)rhombus[2], (Point)rhombus[3]);
            parallelogram.AddReason();
            parallelogram.AddCondition(rhombus);
            AddProcessor.Add(parallelogram);
            Kite kite = new Kite((Point)rhombus[0], (Point)rhombus[1], (Point)rhombus[2], (Point)rhombus[3]);
            kite.AddReason();
            kite.AddCondition(rhombus);
            AddProcessor.Add(kite);
        }
        public void RuleQi004风筝形是四边形(Kite quad)
        {
            Quadriliateral parallelogram = new Quadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
            parallelogram.AddReason();
            parallelogram.AddCondition(quad);
            AddProcessor.Add(parallelogram);
        }
        public void RuleQi005平行四边形是四边形(Parallelogram parallel)
        {
            Quadriliateral parallelogram = new Quadriliateral((Point)parallel[0], (Point)parallel[1], (Point)parallel[2], (Point)parallel[3]);
            parallelogram.AddReason();
            parallelogram.AddCondition(parallel);
            AddProcessor.Add(parallelogram);
        }
        #endregion

        #region 梯形系列
        public void RuleQi006等腰梯形是梯形(IsoscelesTrapezoid trapezoid)
        {
            Quadriliateral pred = new Quadriliateral((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            pred.AddReason();
            pred.AddCondition(trapezoid);
            AddProcessor.Add(pred);
        }
        public void RuleQi007直角梯形是梯形(RightTrapezoid trapezoid)
        {
            Quadriliateral pred = new Quadriliateral((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            pred.AddReason();
            pred.AddCondition(trapezoid);
            AddProcessor.Add(pred);
        }
        public void RuleQi008梯形是四边形(Trapezoid trapezoid)
        {
            Quadriliateral pred = new Quadriliateral((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            pred.AddReason();
            pred.AddCondition(trapezoid);
            AddProcessor.Add(pred);
        }
        #endregion
    }
}
