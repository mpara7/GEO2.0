namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Cal
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 圆计算 : PlaneRuleClass
    {
        public void RuleCC001圆半径相等(Circle circle)
        {
            for (int i = 2; i < circle.Properties.Count; i++)
            {
                var first = KnowledgeGetter.GetSegment(circle[0], circle[1]);
                var other = KnowledgeGetter.GetSegment(circle[0], circle[i]);
                if (first is null || other is null) continue;
                //var pred = new GeoEquation(first.Length, other.Length);
                //pred.AddReason();
                //pred.AddCondition(circle);
                //AddProcessor.Add(pred);
            }
        }
        public void RuleCC002圆直径相等(Circle circle)
        {
            List<Segment> segments = new List<Segment>();
            for (int i = 1; i < circle.Properties.Count; i++)
            {
                for (int j = i + 1; j < circle.Properties.Count; j++)
                {
                    var pWithin = KnowledgeGetter.GetPointWithin((Point)circle[0], (Point)circle[i], (Point)circle[j]);
                    if (pWithin is not NegatingKnowledge)
                    {
                        var segment = KnowledgeGetter.GetSegment(circle[i], circle[j]);
                        if (segment is not null)
                        {
                            segments.Add(segment);
                        }

                    }
                }
            }
            for (int i = 0; i < segments.Count; i++)
            {
                //var pred2 = new GeoEquation(segments[i].Length, circle.Diameter);
                //pred2.AddReason();
                //pred2.AddCondition(circle);
                //AddProcessor.Add(pred2);

                //for (int j = i + 1; j < segments.Count; j++)
                //{
                //    var pred = new GeoEquation(segments[i].Length, segments[j].Length);
                //    pred.AddReason();
                //    pred.AddCondition(circle);
                //    AddProcessor.Add(pred);
                //}
            }
        }

        public void RuleCC003圆直径相等_基于90度(Circle circle, AngleSize angleSize)
        {
            if (angleSize.Expr.CompareTo(90) == ExprCompareResult.Equal)
            {
                foreach (var p1 in angleSize.Angle.Edge1)
                {
                    foreach (var p2 in angleSize.Angle.Edge2)
                    {
                        if (circle.Properties.Skip(1).Contains(angleSize.Angle[1]) &&
                            circle.Properties.Skip(1).Contains(p1) &&
                            circle.Properties.Skip(1).Contains(p2))
                        {
                            var seg = KnowledgeGetter.GetSegment((Point)p1, (Point)p2);
                            //var pred = new GeoEquation(seg.Length, circle.Diameter);
                            //pred.AddReason();
                            //pred.AddCondition(circle, angleSize);
                            //AddProcessor.Add(pred);
                        }
                    }
                }
            }
        }
        public void RuleCC004圆周长(Circle circle)
        {
            //var pred = new GeoEquation(circle.Radius.Mul(2).Mul(Expr.Pi), circle.Perimeter);
            //pred.AddReason();
            //pred.AddCondition(circle);
            //AddProcessor.Add(pred);
        }
        public void RuleCC005圆面积(Circle circle)
        {
            //var pred = new GeoEquation(circle.Radius.Pow(2).Mul(Expr.Pi), circle.Area);
            //pred.AddReason();
            //pred.AddCondition(circle);
            //AddProcessor.Add(pred);
        }
        public void RuleCC006圆半径与直径(Circle circle)
        {
            //var pred = new GeoEquation(circle.Radius.Mul(2), circle.Diameter);
            //pred.AddReason();
            //pred.AddCondition(circle);
            //AddProcessor.Add(pred);
        }

        public void RuleCC007圆周角是圆心角一半(Circle circle)
        {
            for (int i = 1; i < circle.Properties.Count; i++)
            {
                for (int j = i + 1; j < circle.Properties.Count; j++)
                {
                    //优弧一侧
                    List<Angle> angle1s = new List<Angle>();
                    //劣弧另一侧
                    List<Angle> angle2s = new List<Angle>();
                    for (int k = j + 1; k < circle.Properties.Count; k++)
                    {
                        var angleO = KnowledgeGetter.GetAngle((Point)circle[i], (Point)circle[0], (Point)circle[j]);
                        var angleR = KnowledgeGetter.GetAngle((Point)circle[i], (Point)circle[k], (Point)circle[j]);
                        if (angleO is null || angleR is null) continue;
                        //var pred = new GeoEquation(angleO.Size, angleR.Size);
                        //pred.AddReason();
                        //pred.AddCondition(circle);
                        //AddProcessor.Add(pred);
                        var judge = KnowledgeGetter.GetJudgeIpsilateral((Point)circle[i], (Point)circle[j], (Point)circle[0], (Point)circle[k]);
                        if (judge is PointsOnLineSameSide)
                        {
                            angle1s.Add(angleR);
                        }
                        else if (judge is PointsOnLineDifferentSide)
                        {
                            angle2s.Add(angleR);
                        }

                    }

                    foreach (var angle1 in angle1s)
                    {
                        foreach (var angle2 in angle2s)
                        {
                            //var pred = new GeoEquation(angle1.Size.Add(angle2.Size), 180);
                            //pred.AddReason("圆内互补");
                            //pred.AddCondition(circle);
                            //AddProcessor.Add(pred);
                        }
                    }
                }
            }
        }
        //    new ("弧长","",["(线段+线段)*线段/2=四边形面积(AB,CD,BC,2,ABCD)"]),
        //    new ("弧角","",["(线段+线段)*线段/2=四边形面积(AB,CD,BC,2,ABCD)"]),

        //    new ("半圆周长","",["(线段+线段)*线段/2=四边形面积(AB,CD,BC,2,ABCD)"]),
        //    new ("半圆面积","",["(线段+线段)*线段/2=四边形面积(AB,CD,BC,2,ABCD)"]),
        //    new ("扇形周长","",["(线段+线段)*线段/2=四边形面积(AB,CD,BC,2,ABCD)"]),
        //    new ("扇形面积","",["(线段+线段)*线段/2=四边形面积(AB,CD,BC,2,ABCD)"]),

    }
}
