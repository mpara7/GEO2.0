namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Normal
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 三角形四心 : PlaneRuleClass
    {
        void RuleT020重心判定(Triangle triangle, Midpoint midpoint1, Midpoint midpoint2, Line line1, Line line2)
        {
            var ab = KnowledgeGetter.GetSegment(triangle[0], triangle[1]);
            var bc = KnowledgeGetter.GetSegment(triangle[1], triangle[2]);
            var ca = KnowledgeGetter.GetSegment(triangle[2], triangle[0]);
            Point opoint1, opoint2 = null;
            if (midpoint1[1] == ab)
            {
                opoint1 = (Point)triangle[2];
            }
            else if (midpoint1[1] == bc)
            {
                opoint1 = (Point)triangle[0];
            }
            else if (midpoint1[1] == ca)
            {
                opoint1 = (Point)triangle[1];
            }
            else return;
            if (midpoint2[1] == ab)
            {
                opoint2 = (Point)triangle[2];
            }
            else if (midpoint2[1] == bc)
            {
                opoint2 = (Point)triangle[0];
            }
            else if (midpoint2[1] == ca)
            {
                opoint2 = (Point)triangle[1];
            }
            else return;

            if (line1.Properties.Contains(opoint1) && line1.Properties.Contains(midpoint1[0]) &&
                line2.Properties.Contains(opoint2) && line2.Properties.Contains(midpoint2[0]))
            {
                var (i, _, _) = FindIntersection(line1.Properties, line2.Properties);
                if (i.Count == 1)
                {
                    TriangleOrthocenter pred = new TriangleOrthocenter((Point)i[0], triangle);
                    pred.AddReason();
                    pred.AddCondition(midpoint1, midpoint2, line1, line2);
                    AddProcessor.Add(pred);
                }
            }
        }
        void RuleT021外心判定(Triangle triangle, PointOnVerticalBisector verticalBisector1, PointOnVerticalBisector verticalBisector2)
        {
            if (verticalBisector1.Properties[0].Properties.TrueForAll(triangle.Properties.Contains) &&
                verticalBisector2.Properties[0].Properties.TrueForAll(triangle.Properties.Contains))
            {
                var (i, _, _) = FindIntersection(verticalBisector1.PointOnBisector, verticalBisector2.PointOnBisector);
                if (i.Count == 1)
                {
                    TriangleCentroid pred = new TriangleCentroid(i[0], triangle);
                    pred.AddReason();
                    pred.AddCondition(verticalBisector1, verticalBisector2);
                    AddProcessor.Add(pred);
                }
            }
        }
        void RuleT022垂心判定(Triangle triangle, LinePerpendicular perpendicular1, LinePerpendicular perpendicular2)
        {
            var ab = KnowledgeGetter.GetLine(triangle[0], triangle[1]);
            var bc = KnowledgeGetter.GetLine(triangle[1], triangle[2]);
            var ca = KnowledgeGetter.GetLine(triangle[2], triangle[0]);
            List<Line> lines = new List<Line>() { ab, bc, ca };
            for (int i = 0; i < lines.Count; i++)
            {
                var line1 = lines[i % 3];
                var line2 = lines[i + 1 % 3];
                Line pline1;
                Line pline2;
                if (perpendicular1.Line1 == line1 && perpendicular2.Line1 == line2)
                {
                    pline1 = perpendicular1.Line2;
                    pline2 = perpendicular2.Line2;
                }
                else if (perpendicular1.Line1 == line1 && perpendicular2.Line2 == line2)
                {
                    pline1 = perpendicular1.Line2;
                    pline2 = perpendicular2.Line1;
                }
                else if (perpendicular1.Line2 == line1 && perpendicular2.Line1 == line2)
                {
                    pline1 = perpendicular1.Line1;
                    pline2 = perpendicular2.Line2;
                }
                else if (perpendicular1.Line2 == line1 && perpendicular2.Line2 == line2)
                {
                    pline1 = perpendicular1.Line1;
                    pline2 = perpendicular2.Line1;
                }
                else return;


                if (pline1.Properties.Exists(p => p == triangle[i + 2 % 3]) &&
                       pline2.Properties.Exists(p => p == triangle[i + 0 % 3]))
                {
                    var (ig, _, _) = FindIntersection(pline1, pline2);
                    if (ig.Count == 1)
                    {
                        TriangleCentroid pred = new TriangleCentroid((Point)ig[0], triangle);
                        pred.AddReason();
                        pred.AddCondition(perpendicular1, perpendicular2);
                        AddProcessor.Add(pred);
                        break;
                    }
                }
            }

        }
        void RuleT023内心判定(Triangle triangle, PointOnAngularBisector bisector1, PointOnAngularBisector bisector2)
        {
            if (bisector1.Properties[0].Properties.TrueForAll(triangle.Properties.Contains) &&
               bisector2.Properties[0].Properties.TrueForAll(triangle.Properties.Contains))
            {
                var (i, _, _) = FindIntersection(bisector1.PointOnBisector, bisector2.PointOnBisector);
                if (i.Count == 1)
                {
                    TriangleCentroid pred = new TriangleCentroid(i[0], triangle);
                    pred.AddReason();
                    pred.AddCondition(bisector1, bisector2);
                    AddProcessor.Add(pred);
                }
            }
        }


        void RuleT024重心性质(TriangleCentroid centroit, Line line1)
        {
            for (int i = 0; i < line1.Properties.Count; i++)
            {
                var v = line1.Properties[i];
                if (line1.Properties.Contains(v) && line1.Properties.Contains(centroit[0]))
                {
                    foreach (var point in line1.Properties)
                    {
                        var pWithin = KnowledgeGetter.GetPointWithin((Point)point, (Point)line1.Properties[i + 1 % 3], (Point)line1.Properties[i + 2 % 3]);
                        if (pWithin is not NegatingKnowledge)
                        {
                            var longSeg = KnowledgeGetter.GetSegment(v, centroit[0]);
                            var shortSeg = KnowledgeGetter.GetSegment(point, centroit[0]);

                            //GeoEquation pred = new(shortSeg.Length.Mul(2), longSeg.Length);
                            //pred.AddReason();
                            //pred.AddCondition(centroit, line1);
                            //AddProcessor.Add(pred);
                        }
                    }
                }
            }
        }
        void RuleT025垂心性质(TriangleOrthocenter orthocenter)
        {
            for (int i = 0; i < orthocenter[1].Properties.Count; i++)
            {
                var line1 = KnowledgeGetter.GetLine(orthocenter[0], orthocenter[1][i]);
                var line2 = KnowledgeGetter.GetLine(orthocenter[1][i + 1 % 3], orthocenter[1][i + 2 % 3]);
                LinePerpendicular pred = new(line1, line2);
                pred.AddReason();
                pred.AddCondition(orthocenter);
                AddProcessor.Add(pred);
            }
        }
        void RuleT026内心性质(TriangleIncenter orthocenter)
        {
            for (int i = 0; i < orthocenter[1].Properties.Count; i++)
            {
                var angle = KnowledgeGetter.GetAngle(orthocenter[1][i + 1 % 3], orthocenter[1][i + 1 % 3], orthocenter[1][i + 2 % 3]);
                PointOnAngularBisector pred = new(angle, (Point)orthocenter[0]);
                pred.AddReason();
                pred.AddCondition(orthocenter);
                AddProcessor.Add(pred);
            }
        }
        void RuleT027外心性质(TriangleCircumcenter orthocenter)
        {
            for (int i = 0; i < orthocenter[1].Properties.Count; i++)
            {
                var angle1 = KnowledgeGetter.GetAngle(orthocenter[1][i % 3], orthocenter[1][i + 1 % 3], orthocenter[1][i + 2 % 3]);
                var angle2 = KnowledgeGetter.GetAngle(orthocenter[1][i % 3], orthocenter[1][i + 2 % 3], orthocenter[1][i + 1 % 3]);
                var angleC = KnowledgeGetter.GetAngle(orthocenter[1][i + 1 % 3], orthocenter[0], orthocenter[1][i + 2 % 3]);
                //GeoEquation pred = new(angle1.Size.Add(angle2.Size), angleC.Size);
                //pred.AddReason();
                //pred.AddCondition(orthocenter);
                //AddProcessor.Add(pred);
            }
        }
    }
}
