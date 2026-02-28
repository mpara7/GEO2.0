using System.Linq;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Determine
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    public class 线关系判定 : PlaneRuleClass
    {
        public void RuleDL001直线平行的传递性(LineParallel parallel1, LineParallel parallel2)
        {
            var (c, nc1, nc2) = FindCommon(parallel1, parallel2);
            if (c is not null)
            {
                LineParallel parallel = new LineParallel((Line)nc1, (Line)nc2);
                parallel.AddReason();
                parallel.AddCondition(parallel1, parallel2);
                AddProcessor.Add(parallel);
            }
        }
        public void RuleDL002垂直与同一条直线的直线互相平行或共线(LinePerpendicular parallel1, LinePerpendicular parallel2)
        {
            var (c, nc1, nc2) = FindCommon(parallel1, parallel2);
            if (c is not null)
            {
                if (!nc1.Properties.Exists(nc2.Properties.Contains))
                {
                    LineParallel parallel = new LineParallel((Line)nc1, (Line)nc2);
                    parallel.AddReason();
                    parallel.AddCondition(parallel1, parallel2);
                    AddProcessor.Add(parallel);

                }
                else
                {
                    Line parallel = new Line(nc1.Properties.Union(nc2.Properties).Select(p => (Point)p).Distinct().ToArray());
                    parallel.AddReason();
                    parallel.AddCondition(parallel1, parallel2);
                    AddProcessor.Add(parallel);
                }

            }
        }
        public void RuleDL003同位角或内错角得直线平行(AngleSizeEqual angleSizeEqual)
        {
            var angle1 = (Angle)angleSizeEqual[0];
            var angle2 = (Angle)angleSizeEqual[0];
            var line11 = KnowledgeGetter.GetLine(angle1.Vertex, angle1.Edge1.First());
            var line12 = KnowledgeGetter.GetLine(angle1.Vertex, angle1.Edge2.First());
            var line21 = KnowledgeGetter.GetLine(angle2.Vertex, angle2.Edge1.First());
            var line22 = KnowledgeGetter.GetLine(angle2.Vertex, angle2.Edge2.First());
            var (c, nc1, nc2) = FindCommon(line11, line12, line21, line22);

            if (c is not null)
            {
                if (nc1 == nc2) return;
                LineParallel parallel = new LineParallel((Line)nc1, (Line)nc2);
                var judge = KnowledgeGetter.GetJudgeIpsilateral((Point)c[0], (Point)c[1], (Point)nc1[0], (Point)nc2[0]);
                if (judge is PointsOnLineSameSide)
                {
                    parallel.AddReason();
                }
                else
                {
                    parallel.AddReason();
                }
                parallel.AddCondition(angleSizeEqual);
                AddProcessor.Add(parallel);
            }
        }
        public void RuleDL004直线与两条平行线中的一条垂直那么它也与另一条直线垂直(LinePerpendicular perpendicular, LineParallel parallel)
        {
            var (c, nc1, nc2) = FindCommon(perpendicular, parallel);
            if (c is not null)
            {
                LinePerpendicular pred = new LinePerpendicular((Line)nc1, (Line)nc2);
                pred.AddReason();
                pred.AddCondition(perpendicular, parallel);
                AddProcessor.Add(pred);
            }
        }
        public void RuleDL005根据直角得垂直(AngleSize angleSize)
        {
            if (angleSize.Expr.CompareTo(90) == ExprCompareResult.Equal)
            {
                var line1 = KnowledgeGetter.GetLine(angleSize.Angle.Edge1[0], angleSize.Angle.Vertex);
                var line2 = KnowledgeGetter.GetLine(angleSize.Angle.Edge2[0], angleSize.Angle.Vertex);
                LinePerpendicular pred = new LinePerpendicular(line1, line2);
                pred.AddReason();
                pred.AddCondition(angleSize, angleSize);
                AddProcessor.Add(pred);
            }
        }
        public void RuleDL006根据平角得共线(AngleSize angleSize)
        {
            if (angleSize.Expr.CompareTo(180) == ExprCompareResult.Equal)
            {
                var line1 = KnowledgeGetter.GetLine(angleSize.Angle.Edge1[0], angleSize.Angle.Vertex);
                var line2 = KnowledgeGetter.GetLine(angleSize.Angle.Edge2[0], angleSize.Angle.Vertex);
                var points = line1.Points.Union(line2.Points);
                points = points.Distinct();
                Line pred = new Line(points.ToArray());
                pred.AddReason();
                pred.AddCondition(angleSize, angleSize);
                AddProcessor.Add(pred);
            }
        }


        public void RuleDL007垂直平分线_中点_垂直(Midpoint midpoint, LinePerpendicular perpendicular)
        {
            if (perpendicular.Line1.Properties.Contains(midpoint.EndPoint1)&&
                perpendicular.Line1.Properties.Contains(midpoint.EndPoint2))
            {
                if (perpendicular.Line2.Properties.Contains(midpoint.Point))
                {
                    Segment seg= GetSegment(midpoint.EndPoint1, midpoint.EndPoint2);
                    PointOnVerticalBisector pred = new PointOnVerticalBisector(seg, perpendicular.Line2.Points.ToArray());
                    pred.AddReason();
                    pred.AddCondition(midpoint, perpendicular);
                    AddProcessor.Add(pred);
                }
            }
            else if (perpendicular.Line2.Properties.Contains(midpoint.EndPoint1) &&
                perpendicular.Line2.Properties.Contains(midpoint.EndPoint2))
            {
                if (perpendicular.Line1.Properties.Contains(midpoint.Point))
                {
                    Segment seg = GetSegment(midpoint.EndPoint1, midpoint.EndPoint2);
                    PointOnVerticalBisector pred = new PointOnVerticalBisector(seg, perpendicular.Line1.Points.ToArray());
                    pred.AddReason();
                    pred.AddCondition(midpoint, perpendicular);
                    AddProcessor.Add(pred);
                }
            }
        }
        public void RuleDL008垂直平分线_到两端距离相等(SegmentLengthEqual equal, Point point)
        {
            var (c, nc1, nc2) = FindCommon(equal[0], equal[1]);
            if (c is null) return;
            var pWithin = KnowledgeGetter.GetPointWithin(point, (Point)nc1, (Point)nc2);
            if (pWithin is not NegatingKnowledge)
            {
                var seg = KnowledgeGetter.GetSegment(nc1, nc2);
                if (seg is not null)
                {
                    PointOnVerticalBisector pred = new PointOnVerticalBisector(seg, (Point)c);
                    pred.AddReason();
                    pred.AddCondition(equal, point);
                    AddProcessor.Add(pred);
                }

            }
        }
        public void RuleDL009垂直平分线判定_角相等(AngleSizeEqual equal, Point point)
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
            var seg = KnowledgeGetter.GetSegment((Point)angle1[1], (Point)angle2[1]);
            PointOnVerticalBisector pred = new PointOnVerticalBisector(seg, i[0]);
            pred.AddReason();
            pred.AddCondition(equal, point);
            AddProcessor.Add(pred);
        }

        public void RuleDL010角平分线_垂直平分(PointOnVerticalBisector vb)
        {
            if (vb.PointOnBisector.Count < 2) return;
            foreach (var point in vb.PointOnBisector)
            {
                var line = KnowledgeGetter.GetColine(point, vb[0][0], vb[0][1]);
                if (line is not NegatingKnowledge)
                    continue;

                var angle = KnowledgeGetter.GetAngle(vb[0][0], vb.PointOnBisector[0], vb[0][1]);
                if (angle is not null)
                {
                    PointOnAngularBisector pred = new PointOnAngularBisector(angle, point);
                    pred.AddReason();
                    pred.AddCondition(vb);
                    AddProcessor.Add(pred);
                }
            }
        }
        public void RuleDL011角平分线_角相等(AngleSizeEqual equal, Point point)
        {
            //交点
            List<Point> cross = new List<Point>();
            List<Point> edge1 = new List<Point>();
            List<Point> edge2 = new List<Point>();
            if (equal.Angle1.Edge1.TrueForAll(equal.Angle2.Edge1.Contains))
            {
                cross = equal.Angle1.Edge1;
                edge1 = equal.Angle1.Edge2;
                edge2 = equal.Angle2.Edge2;
            }
            else if (equal.Angle1.Edge1.TrueForAll(equal.Angle2.Edge2.Contains))
            {
                cross = equal.Angle1.Edge1;
                edge1 = equal.Angle1.Edge2;
                edge2 = equal.Angle2.Edge1;
            }
            else if (equal.Angle1.Edge2.TrueForAll(equal.Angle2.Edge1.Contains))
            {
                cross = equal.Angle1.Edge2;
                edge1 = equal.Angle1.Edge1;
                edge2 = equal.Angle2.Edge2;
            }
            else if (equal.Angle1.Edge2.TrueForAll(equal.Angle2.Edge2.Contains))
            {
                cross = equal.Angle1.Edge2;
                edge1 = equal.Angle1.Edge1;
                edge2 = equal.Angle2.Edge1;
            }
            else
                return;
            //另一边的交点
            List<Point> cross2 = FindIntersection(edge1, edge2).intersection;
            if (cross2.Count == 0) return;

            var judge = KnowledgeGetter.GetJudgeIpsilateral(cross[0], (Point)cross2[0], (Point)edge1[0], (Point)edge2[0]);
            if (judge is PointsOnLineDifferentSide)
            {
                var pWithin = KnowledgeGetter.GetPointWithin((Point)cross[0], (Point)edge1[0], (Point)edge2[0]);
                if (pWithin is not NegatingKnowledge)
                {
                    var line = KnowledgeGetter.GetLine(point, cross[0]);
                    var angle = KnowledgeGetter.GetAngle(edge1[0], cross[0], edge2[0]);
                    if (angle is null) return;
                    PointOnAngularBisector pred = new PointOnAngularBisector(angle, line.Properties.Select(p => (Point)p).ToArray());
                    pred.AddReason();
                    pred.AddCondition(equal);
                    AddProcessor.Add(pred);
                }
            }

        }

        public void RuleDL012圆切线(Circle circle, AngleSize angleSize)
        {
            if (angleSize.Expr.CompareTo(90) == ExprCompareResult.Equal)
            {
                if (angleSize.Angle.Edge1.Contains(circle[0]) && circle.Properties.Skip(1).Contains(angleSize.Angle[1]))
                {
                    var t = KnowledgeGetter.GetLine(angleSize.Angle[1], angleSize.Angle.Edge2[0]);
                    Tangent pred = new Tangent(t, circle);
                    pred.AddReason();
                    pred.AddCondition(circle, angleSize);
                    AddProcessor.Add(pred);
                }
                else if (angleSize.Angle.Edge2.Contains(circle[0]) && circle.Properties.Skip(1).Contains(angleSize.Angle[1]))
                {
                    var t = KnowledgeGetter.GetLine(angleSize.Angle[1], angleSize.Angle.Edge1[0]);
                    Tangent pred = new Tangent(t, circle);
                    pred.AddReason();
                    pred.AddCondition(circle, angleSize);
                    AddProcessor.Add(pred);
                }
            }
        }

        public void RuleDL013角平分线到角的两端的距离相等(PointOnAngularBisector angularBisector, Pedal pedal1, Pedal pedal2)
        {
            //获取垂足1的两边线段
            Line line1 = (Line)pedal1[1];
            Line line2 = (Line)pedal1[2];
            //获取垂足2的两边线段
            Line line3 = (Line)pedal2[1];
            Line line4 = (Line)pedal2[2];

            Angle angle = (Angle)angularBisector[0];
            List<Point> points_bisector = angularBisector.PointOnBisector;
            
            List<Point> edge1 = new List<Point>(angle.Edge1);
            edge1.Insert(0, (Point)angle.Vertex);
            List<Point> edge2 = new List<Point>(angle.Edge2);
            edge2.Insert(0, (Point)angle.Vertex);
            //获取角的两边共线关系
            Line line_edge1 = KnowledgeGetter.GetLine(edge1.ToArray());
            Line line_edge2 = KnowledgeGetter.GetLine(edge2.ToArray());
            if ((line1 == line_edge1 && line3 == line_edge2) || (line1 == line_edge2 && line3 == line_edge1))
            {
                List<Point> intersection = line2.Points.Intersect(line4.Points).ToList();
                Point intersect_point;
                if (intersection != null && (intersect_point = intersection.FirstOrDefault(item => points_bisector.Contains(item))) is not null)
                {
                    SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(KnowledgeGetter.GetSegment(intersect_point, (Point)pedal1[0]), KnowledgeGetter.GetSegment(intersect_point, (Point)pedal2[0]));
                    segmentLengthEqual.AddReason();
                    segmentLengthEqual.AddCondition(angularBisector, pedal1, pedal2);
                    AddProcessor.Add(segmentLengthEqual);

                }


            }
            else if ((line1 == line_edge1 && line4 == line_edge2) || (line1 == line_edge2 && line4 == line_edge1))
            {
                List<Point> intersection = line2.Points.Intersect(line3.Points).ToList();
                Point intersect_point;
                if (intersection != null && (intersect_point = intersection.FirstOrDefault(item => points_bisector.Contains(item))) is not null)
                {
                    SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(KnowledgeGetter.GetSegment(intersect_point, (Point)pedal1[0]), KnowledgeGetter.GetSegment(intersect_point, (Point)pedal2[0]));
                    segmentLengthEqual.AddReason();
                    segmentLengthEqual.AddCondition(angularBisector, pedal1, pedal2);
                    AddProcessor.Add(segmentLengthEqual);

                }

            }
            else if ((line2 == line_edge1 && line3 == line_edge2) || (line2 == line_edge2 && line3 == line_edge1))
            {
                List<Point> intersection = line1.Points.Intersect(line4.Points).ToList();
                Point intersect_point;
                if (intersection != null && (intersect_point = intersection.FirstOrDefault(item => points_bisector.Contains(item))) is not null)
                {
                    SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(KnowledgeGetter.GetSegment(intersect_point, (Point)pedal1[0]), KnowledgeGetter.GetSegment(intersect_point, (Point)pedal2[0]));
                    segmentLengthEqual.AddReason();
                    segmentLengthEqual.AddCondition(angularBisector, pedal1, pedal2);
                    AddProcessor.Add(segmentLengthEqual);

                }

            }
            else if ((line2 == line_edge1 && line4 == line_edge2) || (line2 == line_edge2 && line4 == line_edge1))
            {
                List<Point> intersection = line1.Points.Intersect(line3.Points).ToList();
                Point intersect_point;
                if (intersection != null && (intersect_point = intersection.FirstOrDefault(item => points_bisector.Contains(item))) is not null)
                {
                    SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(KnowledgeGetter.GetSegment(intersect_point, (Point)pedal1[0]), KnowledgeGetter.GetSegment(intersect_point, (Point)pedal2[0]));
                    segmentLengthEqual.AddReason();
                    segmentLengthEqual.AddCondition(angularBisector, pedal1, pedal2);
                    AddProcessor.Add(segmentLengthEqual);
                }
            }

        }

    }
}
