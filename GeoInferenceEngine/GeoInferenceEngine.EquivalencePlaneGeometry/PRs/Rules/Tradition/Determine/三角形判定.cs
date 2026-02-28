using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Determine
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 三角形判定 : PlaneRuleClass
    {
        public void RuleDT001直角三角形_垂直(AngleSize angleSize)
        {
            if (angleSize.Expr.CompareTo(90) != ExprCompareResult.Equal) return;
            foreach (var point1 in angleSize.Angle.Edge1)
            {
                foreach (var point2 in angleSize.Angle.Edge2)
                {

                    if (KnowledgeGetter.HasAllSegment((Point)angleSize.Angle.Vertex, (Point)point1, (Point)point2))
                    {
                        var triangle = KnowledgeGetter.GetTriangle((Point)angleSize.Angle.Vertex, (Point)point1, (Point)point2);
                        if (triangle is null) return;
                        RightTriangle pred = new RightTriangle((Point)angleSize.Angle.Vertex, (Point)point1, (Point)point2);
                        pred.AddReason();
                        pred.AddCondition(angleSize);
                        AddProcessor.Add(pred);
                    }
                }
            }
        }
        public void RuleDT002直角三角形_中点关系(Midpoint midpoint, SegmentLengthEqual equal)
        {
            var bo = KnowledgeGetter.GetSegment(midpoint.Point, midpoint.EndPoint1);
            var oc = KnowledgeGetter.GetSegment(midpoint.Point, midpoint.EndPoint2);
            var o = midpoint[0];
            Knowledge a = null;
            if (equal[0] == bo)
            {
                if (equal[1][0] == o)
                {
                    a = equal[1][1];
                }
                else if (equal[1][1] == o)
                {
                    a = equal[1][0];
                }
            }
            else if (equal[1] == bo)
            {
                if (equal[0][0] == o)
                {
                    a = equal[0][1];
                }
                else if (equal[0][1] == o)
                {
                    a = equal[0][0];
                }
            }
            if (a is null) return;

            if (KnowledgeGetter.HasAllSegment((Point)a, (Point)midpoint.EndPoint1, (Point)midpoint.EndPoint2))
            {
                var triangle = KnowledgeGetter.GetTriangle((Point)a, (Point)midpoint[1], (Point)midpoint[2]);
                if (triangle is null) return;
                RightTriangle pred = new RightTriangle((Point)a, (Point)midpoint[1], (Point)midpoint[2]);
                pred.AddReason();
                pred.AddCondition(midpoint, equal);
                AddProcessor.Add(pred);
            }
        }

        public void RuleDT003有两条边相等的三角形是等腰三角形(SegmentLengthEqual segmentLengthEqual)
        {
            var (c, nc1, nc2) = FindCommon(segmentLengthEqual[0], segmentLengthEqual[1]);
            if (c is null) return;
            var triangle = KnowledgeGetter.GetTriangle((Point)c, (Point)nc1, (Point)nc2);
            if (triangle is null) return;
            IsoscelesTriangle isoscelesTriangle = new IsoscelesTriangle((Point)c, (Point)nc1, (Point)nc2);
            isoscelesTriangle.AddReason();
            isoscelesTriangle.AddCondition(segmentLengthEqual);
            AddProcessor.Add(isoscelesTriangle);
        }
        public void RuleDT004有两个角相等的三角形是等腰三角形(AngleSizeEqual equal)
        {
            var angle1 = equal.Angle1;
            var angle2 = equal.Angle2;
            List<Point> edge1, edge2;
            if (angle1.Edge1.Contains(angle2[1]) && angle2.Edge1.Contains(angle1[1]))
            {
                edge1 = angle1.Edge2;
                edge2 = angle2.Edge2;
            }
            else if (angle1.Edge1.Contains(angle2[1]) && angle2.Edge2.Contains(angle1[1]))
            {
                edge1 = angle1.Edge2;
                edge2 = angle2.Edge1;
            }
            else if (angle1.Edge2.Contains(angle2[1]) && angle2.Edge1.Contains(angle1[1]))
            {
                edge1 = angle1.Edge1;
                edge2 = angle2.Edge2;
            }
            else if (angle1.Edge2.Contains(angle2[1]) && angle2.Edge2.Contains(angle1[1]))
            {
                edge1 = angle1.Edge1;
                edge2 = angle2.Edge1;
            }
            else
                return;
            var (i, _, _) = FindIntersection(edge1, edge2);
            if (i.Count == 0) return;
            var triangle = KnowledgeGetter.GetTriangle((Point)i[0], (Point)angle1[1], (Point)angle2[1]);
            if (triangle is null) return;
            IsoscelesTriangle isoscelesTriangle = new IsoscelesTriangle((Point)i[0], (Point)angle1[1], (Point)angle2[1]);
            isoscelesTriangle.AddReason();
            isoscelesTriangle.AddCondition(equal);
            AddProcessor.Add(isoscelesTriangle);

        }

        public void RuleDT005两个等腰三角形判定等边三角形(IsoscelesTriangle triangle1, IsoscelesTriangle triangle2)
        {
            if (triangle1.Properties.TrueForAll(triangle2.Properties.Contains))
            {
                EquilateralTriangle pred = new EquilateralTriangle((Point)triangle1[0], (Point)triangle1[1], (Point)triangle1[2]);
                pred.AddReason();
                pred.AddCondition(triangle1, triangle2);
                AddProcessor.Add(pred);
            }
        }


    }
}
