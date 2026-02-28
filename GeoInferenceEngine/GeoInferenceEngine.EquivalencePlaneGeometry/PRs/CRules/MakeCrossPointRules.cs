using EmptyBlazorApp1.CKnowledges;
using GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeSegment;
using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CRules
{
    [PlaneRuleType(PlaneRuleType.Constructive)]
    internal class MakeCrossPointRules : PlaneRuleClass
    {
        public void RuleCCP001作交点(MakeSegmentInterectionPoint makePoint)
        {
            LineIntersectionPoint pred = new((Point)makePoint[2], (Line)makePoint[0], (Line)makePoint[1]);
            pred.AddReason();
            pred.AddCondition(makePoint);
            AddProcessor.Add(pred);
        }
        public void RuleCCP002平行相交的交点(MakeParallelIntersection makePoint)
        {
            LineIntersectionPoint pred = new((Point)makePoint[3], (Line)makePoint[2], (Line)makePoint[4]);
            pred.AddReason();
            pred.AddCondition(makePoint);
            AddProcessor.Add(pred);
            LineParallel lineParallel = new LineParallel((Line)makePoint[1], (Line)makePoint[4]);
            lineParallel.AddReason();
            lineParallel.AddCondition(makePoint);
            AddProcessor.Add(lineParallel);
        }
        public void RuleCCP002作垂直相交的交点(MakeVerticalIntersection makePoint)
        {
            LineIntersectionPoint pred = new((Point)makePoint[3], (Line)makePoint[2], (Line)makePoint[4]);
            pred.AddReason();
            pred.AddCondition(makePoint);
            AddProcessor.Add(pred);
            LinePerpendicular pred2 = new LinePerpendicular((Line)makePoint[1], (Line)makePoint[4]);
            pred2.AddReason();
            pred2.AddCondition(makePoint);
            AddProcessor.Add(pred2);
        }
        public void RuleCCP002作过点交于线的交点(MakeThroughPointIntersection makePoint)
        {
            LineIntersectionPoint pred = new((Point)makePoint[3], (Line)makePoint[2], (Line)makePoint[4]);
            pred.AddReason();
            pred.AddCondition(makePoint);
            AddProcessor.Add(pred);
        }
        public void RuleCCP001作垂足(MakeFootPoint makePoint)
        {
            List<Knowledge> points1 = new List<Knowledge>(makePoint[3].Properties);
            Line line = new Line((Point)(points1[0]), (Point)(points1[1]));
            List<Knowledge> points = new List<Knowledge>(makePoint[1].Properties);
            points.Add(makePoint[2]);
            points = points.Distinct().ToList();
            Line line2 = new Line(points.Select(p => (Point)p).ToArray());

            line.AddReason();
            line.AddCondition(makePoint);
            line2.AddReason();
            line2.AddCondition(makePoint);
            line = (Line)AddProcessor.Add(line);
            line2 = (Line)AddProcessor.Add(line2);

            LinePerpendicular pred = new LinePerpendicular(line, line2);
            pred.AddReason();
            pred.AddCondition(makePoint);
            AddProcessor.Add(pred);
        }
    }
}
