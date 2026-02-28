using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Cal
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 角度计算 : PlaneRuleClass
    {
        public void RuleCA001根据交点生成角_在角度计算(Line line1, Line line2)
        {
            var (i, l1, l2) = FindIntersection(line1, line2);
            if (i.Count() == 0) return;
            var pos1 = line1.Properties.IndexOf(i[0]);
            var pos2 = line2.Properties.IndexOf(i[0]);
            List<List<Knowledge>> linepart1 = new List<List<Knowledge>>();
            List<List<Knowledge>> linepart2 = new List<List<Knowledge>>();
            if (pos1 == 0)
            {
                linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
            }
            else if (pos1 == line1.Properties.Count - 1)
            {
                linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
            }
            else
            {
                linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
            }
            if (pos2 == 0)
            {
                linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
            }
            else if (pos2 == line2.Properties.Count - 1)
            {
                linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
            }
            else
            {
                linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
            }

            foreach (var part1 in linepart1)
            {
                foreach (var part2 in linepart2)
                {
                    //产生新知识
                    Angle pred = new Angle(part1.Select(p => (Point)p).ToList(), (Point)i[0], part2.Select(p => (Point)p).ToList());
                    pred.AddReason();
                    pred.AddCondition(line1, line2);
                    AddProcessor.Add(pred);
                }
            }
        }
        public void RuleCA002平角关系()
        {

        }
        public void RuleCA003圆周角关系()
        {

        }
        public void RuleCA004由垂直得直角(LinePerpendicular perpendicular)
        {
            var line1 = perpendicular.Line1;
            var line2 = perpendicular.Line2;
            var (i, l1, l2) = FindIntersection(line1, line2);
            if (i.Count() == 0) return;

            var pos1 = line1.Properties.IndexOf(i[0]);
            var pos2 = line2.Properties.IndexOf(i[0]);
            List<List<Knowledge>> linepart1 = new List<List<Knowledge>>();
            List<List<Knowledge>> linepart2 = new List<List<Knowledge>>();
            if (pos1 == 0)
            {
                linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
            }
            else if (pos1 == line1.Properties.Count - 1)
            {
                linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
            }
            else
            {
                linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
            }
            if (pos2 == 0)
            {
                linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
            }
            else if (pos2 == line2.Properties.Count - 1)
            {
                linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
            }
            else
            {
                linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
            }

            foreach (var part1 in linepart1)
            {
                foreach (var part2 in linepart2)
                {
                    var angle = KnowledgeGetter.GetAngle(part1[0], i[0], part2[0]);
                    if (angle is null) continue;
                    //产生新知识
                    AngleSize pred = new AngleSize(angle, 90);
                    pred.AddReason();
                    pred.AddCondition(perpendicular);
                    AddProcessor.Add(pred);
                }
            }
        }
        public void RuleCA005对顶角与互补(Line line1, Line line2)
        {
            var (i, l1, l2) = FindIntersection(line1, line2);
            if (i.Count() == 0) return;

            var pos1 = line1.Properties.IndexOf(i[0]);
            var pos2 = line2.Properties.IndexOf(i[0]);
            if (pos1 > 0 && pos1 < line1.Properties.Count - 1 && pos2 > 0 && pos2 < line2.Properties.Count - 1)
            {
                var part11 = line1.Properties.Skip(pos1 + 1).ToList();
                var part12 = line1.Properties.Take(pos1).Reverse().ToList();

                var part21 = line2.Properties.Skip(pos2 + 1).ToList();
                var part22 = line2.Properties.Take(pos2).Reverse().ToList();
                var angle1 = KnowledgeGetter.GetAngle(part11[0], i[0], part21[0]);
                var angle2 = KnowledgeGetter.GetAngle(part12[0], i[0], part22[0]);
                var angle3 = KnowledgeGetter.GetAngle(part11[0], i[0], part22[0]);
                var angle4 = KnowledgeGetter.GetAngle(part12[0], i[0], part21[0]);

                List<Knowledge> equations = new List<Knowledge>()
                {
                    new AngleSizeEqual(angle1, angle2),
                    new AngleSizeEqual(angle3, angle4),
                    new GeoEquation(angle1.Size.Add(angle3.Size), 180),
                    new GeoEquation(angle1.Size.Add(angle4.Size), 180),
                    new GeoEquation(angle2.Size.Add(angle3.Size), 180),
                    new GeoEquation(angle2.Size.Add(angle4.Size), 180),

                };
                foreach (var equation in equations)
                {
                    equation.AddReason();
                    equation.AddCondition(line1, line2);
                    AddProcessor.Add(equation);
                }

            }
        }
        /// <summary>
        /// a c b
        /// d f e
        /// </summary>
        /// <param name="parallel"></param>
        /// <param name="segment"></param>
        public void RuleCA006内错角与同旁内角(LineParallel parallel, Segment segment)
        {
            var line1 = (Line)parallel[0];
            var line2 = (Line)parallel[1];
            Point a = null, b = null, c = null, d = null, e = null, f = null;
            var pos1 = line1.Properties.IndexOf(segment[0]);
            var pos2 = line2.Properties.IndexOf(segment[1]);
            if (pos1 == -1 || pos2 == -1)
            {
                pos1 = line1.Properties.IndexOf(segment[1]);
                pos2 = line2.Properties.IndexOf(segment[0]);
                if (pos1 == -1 || pos2 == -1)
                {
                    return;
                }
                else
                {
                    c = (Point)segment[1];
                    f = (Point)segment[0];
                }
            }
            else
            {
                c = (Point)segment[0];
                f = (Point)segment[1];
            }

            if (pos1 == 0)
            {
                a = (Point)line1.Properties.Skip(pos1 + 1).First();
            }
            else if (pos1 == line1.Properties.Count - 1)
            {
                b = (Point)line1.Properties.Take(pos1).Reverse().First();
            }
            else
            {
                a = (Point)line1.Properties.Skip(pos1 + 1).First();
                b = (Point)line1.Properties.Take(pos1).Reverse().First();
            }
            if (pos2 == 0)
            {
                d = (Point)line2.Properties.Skip(pos2 + 1).First();
            }
            else if (pos2 == line2.Properties.Count - 1)
            {
                e = (Point)line2.Properties.Take(pos2).Reverse().First();
            }
            else
            {
                d = (Point)line2.Properties.Skip(pos2 + 1).First();
                e = (Point)line2.Properties.Take(pos2).Reverse().First();
            }
            //分别 ab de交换
            bool flag = false;
            if (a is null)
            {
                var temp = a;
                a = b;
                b = temp;
                flag = !flag;
            }
            if (d is null)
            {
                var temp = d;
                d = e;
                e = temp;
                flag = !flag;
            }

            var judge = KnowledgeGetter.GetJudgeIpsilateral(c, f, a, d);
            if (judge is PointsOnLineDifferentSide)
            {
                if (!flag)
                {
                    var temp = a;
                    a = d;
                    d = temp;
                }
            }


            Angle acf = null, bcf = null, dfc = null, efc = null;
            if (a is not null)
                acf = KnowledgeGetter.GetAngle(a, c, f);
            if (b is not null)
                bcf = KnowledgeGetter.GetAngle(b, c, f);
            if (d is not null)
                dfc = KnowledgeGetter.GetAngle(d, f, c);
            if (e is not null)
                efc = KnowledgeGetter.GetAngle(e, f, c);
            if (acf is not null && efc is not null)
            {
                var pred = new AngleSizeEqual(acf, efc);
                pred.AddReason();
                pred.AddCondition(parallel, segment);
                AddProcessor.Add(pred);
            }
            if (bcf is not null && dfc is not null)
            {
                var pred = new AngleSizeEqual(bcf, dfc);
                pred.AddReason();
                pred.AddCondition(parallel, segment);
                AddProcessor.Add(pred);
            }
            if (acf is not null && dfc is not null)
            {
                var pred = new GeoEquation(acf.Size.Add(dfc.Size), 180);
                pred.AddReason();
                pred.AddCondition(parallel, segment);
                AddProcessor.Add(pred);
            }
            if (bcf is not null && efc is not null)
            {
                var pred = new GeoEquation(bcf.Size.Add(efc.Size), 180);
                pred.AddReason();
                pred.AddCondition(parallel, segment);
                AddProcessor.Add(pred);
            }

        }

        /// <summary>
        /// g  
        /// c b
        /// f e
        /// </summary>
        /// <param name="parallel"></param>
        /// <param name="segment"></param>
        public void RuleCA007同位角(LineParallel parallel, Angle angle1, Angle angle2)
        {
            Line edge11 = KnowledgeGetter.GetLine(angle1.Vertex, angle1.Edge1[0]);
            Line edge12 = KnowledgeGetter.GetLine(angle1.Vertex, angle1.Edge2[0]);
            Line edge21 = KnowledgeGetter.GetLine(angle2.Vertex, angle2.Edge1[0]);
            Line edge22 = KnowledgeGetter.GetLine(angle2.Vertex, angle2.Edge2[0]);
            var (c, nc1, nc2) = FindCommon(edge11, edge12, edge21, edge22);
            if (c is null) return;
            if (nc1 == parallel.Line1 && nc2 == parallel.Line2 ||
                nc1 == parallel.Line2 && nc2 == parallel.Line1)
            {
                var pred = new AngleSizeEqual(angle1, angle2);
                pred.AddReason();
                pred.AddCondition(parallel, angle1, angle2);
                AddProcessor.Add(pred);
            }
        }
        public void RuleCA008角相加(Angle angle1, Angle angle2)
        {
            List<Point> cross = new List<Point>();
            List<Point> edge1 = new List<Point>();
            List<Point> edge2 = new List<Point>();
            if (angle1.Edge1.TrueForAll(angle2.Edge1.Contains))
            {
                cross = angle1.Edge1;
                edge1 = angle1.Edge2;
                edge2 = angle2.Edge2;
            }
            else if (angle1.Edge1.TrueForAll(angle2.Edge2.Contains))
            {
                cross = angle1.Edge1;
                edge1 = angle1.Edge2;
                edge2 = angle2.Edge1;
            }
            else if (angle1.Edge2.TrueForAll(angle2.Edge1.Contains))
            {
                cross = angle1.Edge2;
                edge1 = angle1.Edge1;
                edge2 = angle2.Edge2;
            }
            else if (angle1.Edge2.TrueForAll(angle2.Edge2.Contains))
            {
                cross = angle1.Edge2;
                edge1 = angle1.Edge1;
                edge2 = angle2.Edge1;
            }
            else
                return;

            var judge = KnowledgeGetter.GetJudgeIpsilateral((Point)cross[0], (Point)angle1[1], (Point)edge1[0], (Point)edge2[0]);
            if (judge is PointsOnLineDifferentSide diff)
            {
                var pWithin = KnowledgeGetter.GetPointWithin(angle1.Vertex, (Point)edge1[0], (Point)edge2[0]);
                if (pWithin is not NegatingKnowledge)
                {
                    var bigAngle = KnowledgeGetter.GetAngle(edge1[0], angle1.Vertex, edge2[0]);
                    if (bigAngle is null) return;
                    //var pred = new GeoEquation(angle1.Size.Add(angle2.Size), bigAngle.Size);
                    //pred.AddReason();
                    //pred.AddCondition(angle1, angle2, judge);
                    ////Debug.Assert(pred.ToString()!= "(角B-D-C的大小+角B-D-GE的大小)=角C-D-GE的大小");
                    //AddProcessor.Add(pred);
                }
            }
        }
        public void RuleCA009点在垂直平分线上性质(PointOnVerticalBisector verticalBisector)
        {
            var a = verticalBisector[0][0];
            var b = verticalBisector[0][1];

            foreach (var point in verticalBisector.PointOnBisector)
            {
                var seg1 = KnowledgeGetter.GetSegment(point, a);
                var seg2 = KnowledgeGetter.GetSegment(point, b);
                if (seg1 is null || seg2 is null) continue;

                SegmentLengthEqual pred = new SegmentLengthEqual(seg1, seg2);
                pred.AddReason();
                pred.AddCondition(verticalBisector);
                AddProcessor.Add(pred);
            }
        }
        public void RuleCA010点在角平分线上性质(PointOnAngularBisector angularBisector)
        {
            Angle angle = (Angle)angularBisector[0];
            List<Point> Edge1 = angle.Edge1;
            //角的顶点
            Point v = angle.Vertex;
            List<Point> Edge2 = angle.Edge2;

            foreach (var point in angularBisector.PointOnBisector)
            {
                var angle1 = KnowledgeGetter.GetAngle(Edge1[0], v, point);
                var angle2 = KnowledgeGetter.GetAngle(point, v, Edge2[0]);
                if (angle1 is null || angle2 is null) continue;
                var pred = new AngleSizeEqual(angle1, angle2);
                pred.AddReason();
                pred.AddCondition(angularBisector);
                AddProcessor.Add(pred);
            }
        }
        public void RuleCA011切线性质(Tangent tangent)
        {
            foreach (var point in tangent[0].Properties)
            {
                if (tangent[1].Properties.Skip(1).Contains(point))
                {
                    foreach (var leftPoint in tangent[0].Properties.Where(p => p != point))
                    {
                        var angle1 = KnowledgeGetter.GetAngle(tangent[1][0], point, leftPoint);
                        AngleSize pred = new AngleSize(angle1, 90);
                        pred.AddReason();
                        pred.AddCondition(tangent);
                        AddProcessor.Add(pred);
                    }
                }
            }
        }


    }
}
