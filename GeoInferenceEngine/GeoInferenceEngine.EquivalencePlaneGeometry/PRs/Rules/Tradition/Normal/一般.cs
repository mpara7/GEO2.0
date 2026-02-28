namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Normal
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 一般 : PlaneRuleClass
    {
        public void RuleN001同位角相等(LineParallel parallel, Line line)
        {

            //**new ("同位角相等","直线平行(ABCD)&共线(EAC)",["角=角(EAB,ECD)"]),
            //缺少相对位置
            //new ("内错角相等","直线平行(ABCD)&共线(EAC)",["角=角(EAB,ECD)"]),
            ////缺少相对位置
            //new ("同旁内角180度","直线平行(ABCD)",["角+角=角(ABC,BCD,180)"]),
            // * >>/
            var iPoint1 = FindIntersection(parallel[0], line).intersection.FirstOrDefault();
            var iPoint2 = FindIntersection(parallel[1], line).intersection.FirstOrDefault();
            if (iPoint1 is null || iPoint2 is null) return;
        }
        //public void RuleN001点在角平分线上性质(PointOnAngularBisector angularBisector, AngleSize angleSize1, AngleSize angleSize2)
        //{
        //    if (angleSize1.Expr.CompareTo(90) != ExprCompareResult.Equal || angleSize2.Expr.CompareTo(90) != ExprCompareResult.Equal) return;
        //    Angle bigAngle = (Angle)angularBisector[0];
        //    var bv = angularBisector[0][1];
        //    Angle angle1 = (Angle)angleSize1[0];
        //    var v1 = angle1[1];
        //    Angle angle2 = (Angle)angleSize1[0];
        //    var v2 = angle2[1];
        //    if (bigAngle.Edge1.Contains(v1) && bigAngle.Edge2.Contains(v2))
        //    {
        //        foreach (var point in angularBisector.PointOnBisector)
        //        {
        //            if (angle1.Edge1.Contains(bv) && angle1.Edge2.Contains(point)
        //                && angle2.Edge1.Contains(bv) && angle1.Edge2.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else if (angle1.Edge1.Contains(bv) && angle1.Edge2.Contains(point)
        //                && angle2.Edge2.Contains(bv) && angle1.Edge1.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else if (angle1.Edge2.Contains(bv) && angle1.Edge1.Contains(point)
        //                && angle2.Edge1.Contains(bv) && angle1.Edge2.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else if (angle1.Edge2.Contains(bv) && angle1.Edge1.Contains(point)
        //                && angle2.Edge2.Contains(bv) && angle1.Edge1.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else return;
        //        }
        //    }
        //    else if (bigAngle.Edge1.Contains(v2) && bigAngle.Edge2.Contains(v1))
        //    {
        //        foreach (var point in angularBisector.PointOnBisector)
        //        {
        //            if (angle1.Edge1.Contains(bv) && angle1.Edge2.Contains(point)
        //                && angle2.Edge1.Contains(bv) && angle1.Edge2.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else if (angle1.Edge1.Contains(bv) && angle1.Edge2.Contains(point)
        //                && angle2.Edge2.Contains(bv) && angle1.Edge1.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else if (angle1.Edge2.Contains(bv) && angle1.Edge1.Contains(point)
        //                && angle2.Edge1.Contains(bv) && angle1.Edge2.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else if (angle1.Edge2.Contains(bv) && angle1.Edge1.Contains(point)
        //                && angle2.Edge2.Contains(bv) && angle1.Edge1.Contains(point))
        //            {
        //                var seg1 = KnowledgeGetter.GetSegment(v1, point);
        //                var seg2 = KnowledgeGetter.GetSegment(v2, point);
        //                GeoEquation pred = new GeoEquation(seg1.Length, seg2.Length);
        //                pred.AddReason();
        //                pred.AddCondition(angularBisector, angleSize1, angleSize2);
        //                AddProcessor.Add(pred);
        //            }
        //            else return;
        //        }
        //    }

        //}
        ///// <summary>
        ///// ?
        ///// </summary>
        ///// <param name="bisector"></param>
        ///// <param name="line"></param>
        //public void RuleN002三角形内角平分线性质(PointOnAngularBisector bisector, Line line)
        //{
        //    foreach (var o in bisector.PointOnBisector)
        //    {
        //        var a = bisector[0][0];

        //        foreach (var b in ((Angle)bisector[0]).Edge1)
        //        {
        //            foreach (var c in ((Angle)bisector[0]).Edge2)
        //            {
        //                var pWithin = KnowledgeGetter.GetPointWithin(o, b, c);
        //                if (pWithin is not NegatingKnowledge)
        //                {
        //                    var ao = KnowledgeGetter.GetSegment(o, a);
        //                    var bc = KnowledgeGetter.GetSegment(b, c);
        //                    var co = KnowledgeGetter.GetSegment(c, o);
        //                    var ab = KnowledgeGetter.GetSegment(a, b);

        //                    var ac = KnowledgeGetter.GetSegment(a, c);
        //                    var bo = KnowledgeGetter.GetSegment(b, o);
        //                    if (ao is not null && bc is not null && co is not null && ab is not null)
        //                    {
        //                        GeoEquation pred = new GeoEquation(ao.Length.Mul(bc.Length), co.Length.Mul(ab.Length));
        //                        pred.AddReason();
        //                        pred.AddCondition(bisector);
        //                        AddProcessor.Add(pred);
        //                    }
        //                    if (ao is not null && ab is not null && ac is not null && bo is not null && co is not null)
        //                    {
        //                        GeoEquation pred2 = new GeoEquation(ao.Length.Mul(ao.Length), ab.Length.Mul(ac.Length).Sub(bo.Length.Mul(co.Length)));
        //                        pred2.AddReason();
        //                        pred2.AddCondition(bisector);
        //                        AddProcessor.Add(pred2);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //public void RuleN003中位线的判定(Midpoint midpoint1, Midpoint midpoint2)
        //{
        //    var (c, nc1, nc2) = FindCommon(midpoint1[1], midpoint2[1]);
        //    if (nc1 is null) return;
        //    Line line1 = KnowledgeGetter.GetLine(midpoint1[0], midpoint2[0]);
        //    Line line2 = KnowledgeGetter.GetLine(nc1, nc2);
        //    Segment seg1 = KnowledgeGetter.GetSegment(midpoint1[0], midpoint2[0]);
        //    Segment seg2 = KnowledgeGetter.GetSegment(nc1, nc2);

        //    LineParallel pred = new LineParallel(line1, line2);
        //    pred.AddReason();
        //    pred.AddCondition(midpoint1, midpoint2);
        //    AddProcessor.Add(pred);

        //    GeoEquation equal = new GeoEquation(seg2.Length.Div(seg1.Length), 2);
        //    equal.AddReason();
        //    equal.AddCondition(midpoint1, midpoint2);
        //    AddProcessor.Add(equal);
        //}

        public void RuleN004中位线的逆定理判定(Triangle triangle, LineParallel parallel, Midpoint midpoint)
        {
            //new("三角形中位线逆定理", "直线平行(DE,BC)&线段=线段(AD,DC)&共线(DAC)&共线(EAB)", ["中点(DAC)", "中点(EAB)", "线段/线段=数值(DE,BC,2)"]),
            //var (c, nc1, nc2) = FindCommon(midpoint1[1], midpoint2[2]);
            //if (nc1 == null) return;
            //Line line1 = KnowledgeGetter.GetLine(midpoint1[0], midpoint2[0]);
            //Line line2 = KnowledgeGetter.GetLine(nc1, nc2);
            //Segment seg1 = KnowledgeGetter.GetSegment(midpoint1[0], midpoint2[0]);
            //Segment seg2 = KnowledgeGetter.GetSegment(nc1, nc2);

            //LineParallel pred = new LineParallel(line1, line2);
            //pred.AddReason();
            //pred.AddCondition(midpoint1, midpoint1);
            //AddProcessor.AddNew(pred);

            //GeoEquation equal = new GeoEquation(seg2.Length / seg1.Length, 2);
            //equal.AddReason();
            //equal.AddCondition(midpoint1, midpoint1);
            //Cal.AddEquation(equal);
        }
        public void RuleN005梯形的中线判定(Triangle triangle, LineParallel parallel, Midpoint midpoint)
        {
            // new ("梯形中线关系","梯形(ABCD)中点(EAD)&中点(FBC)&共线",["线段/线段=线段*数值(AB,CD,EF,2)","线段平行(AB,EF)","线段平行(CD,EF)"]),
        }

        public void RuleN006圆外到切点距离相等(Tangent tangent1, Tangent tangent2)
        {
            // new ("同点切向长度相同","圆切线(PA,O)&圆切线(PB,O)",["线段=线段(PA,PB,2)"]),
        }
        public void RuleN007弦得中垂线(Circle circle)
        {
            // new ("弦得中垂线","圆切线(PA,O)&圆切线(PB,O)",["线段=线段(PA,PB,2)"]),
        }

        public void RuleN008三线合一中点()
        {
            // new ("等腰三角形三线合一","等腰三角形(ABC)&中点(OBC)",["直线垂直(AO,OB)","点在角平分线上(O,BAC)"]),
        }
        public void RuleN009三线合一垂直(IsoscelesTriangle isoscelesTriangle, LinePerpendicular perpendicular)
        {
            if (isoscelesTriangle.StrContains("FDE") && perpendicular.StrContains("FG"))
                Console.WriteLine();
            Line bottom = KnowledgeGetter.GetLine(isoscelesTriangle[1], isoscelesTriangle[2]);
            if (perpendicular.Line1 == bottom)
            {
                if (perpendicular.Line2.Points.Contains(isoscelesTriangle[0]))
                {
                    var (i, _, _) = FindIntersection(perpendicular.Line1, perpendicular.Line2);
                    Segment bottomSeg = KnowledgeGetter.GetSegment(isoscelesTriangle[1], isoscelesTriangle[2]);
                    Angle angle = KnowledgeGetter.GetAngle(isoscelesTriangle[1], isoscelesTriangle[0], isoscelesTriangle[2]);
                    Midpoint pred1 = new Midpoint((Point)i[0], (Point)isoscelesTriangle[1], (Point)isoscelesTriangle[2]);
                    PointOnAngularBisector pred2 = new PointOnAngularBisector(angle, (Point)i[0]);

                    pred1.AddReason();
                    pred1.AddCondition(isoscelesTriangle, perpendicular);
                    AddProcessor.Add(pred1);
                    pred2.AddReason();
                    pred2.AddCondition(isoscelesTriangle, perpendicular);
                    AddProcessor.Add(pred2);
                }
            }
            else if (perpendicular.Line2 == bottom)
            {
                if (perpendicular.Line1.Points.Contains(isoscelesTriangle[0]))
                {
                    var (i, _, _) = FindIntersection(perpendicular.Line1, perpendicular.Line2);
                    Segment bottomSeg = KnowledgeGetter.GetSegment(isoscelesTriangle[1], isoscelesTriangle[2]);
                    Angle angle = KnowledgeGetter.GetAngle(isoscelesTriangle[1], isoscelesTriangle[0], isoscelesTriangle[2]);
                    Midpoint pred1 = new Midpoint((Point)i[0], (Point)isoscelesTriangle[1], (Point)isoscelesTriangle[2]);
                    PointOnAngularBisector pred2 = new PointOnAngularBisector(angle, (Point)i[0]);

                    pred1.AddReason();
                    pred1.AddCondition(isoscelesTriangle, perpendicular);
                    AddProcessor.Add(pred1);
                    pred2.AddReason();
                    pred2.AddCondition(isoscelesTriangle, perpendicular);
                    AddProcessor.Add(pred2);
                }
            }
        }
        public void RuleN010三线合一角平分线()
        {
            //   new ("等腰三角形三线合一","等腰三角形(ABC)&点在角平分线上(O,BAC)&共线(BOC)",["中点(O,BC)","直线垂直(AO,OB)"]),
        }









    }
}
