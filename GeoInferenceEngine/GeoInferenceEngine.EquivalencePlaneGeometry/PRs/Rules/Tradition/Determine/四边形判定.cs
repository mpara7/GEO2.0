using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Determine
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 四边形判定 : PlaneRuleClass
    {
        public void RuleDQ001一组对边平行且相等的四边形是平行四边形(Quadriliateral quadriliateral, LineParallel parallel, SegmentLengthEqual equal)
        {
            //Line segment1 = KnowledgeGetter.GetLine((Point)quadriliateral[0], (Point)quadriliateral[1]);
            //Line segment2 = KnowledgeGetter.GetLine((Point)quadriliateral[2], (Point)quadriliateral[3]);
            //Line segment3 = KnowledgeGetter.GetLine((Point)quadriliateral[0], (Point)quadriliateral[3]);
            //Line segment4 = KnowledgeGetter.GetLine((Point)quadriliateral[1], (Point)quadriliateral[2]);
            //Line segment5 = (Line)parallel[0];
            //Line segment6 = (Line)parallel[1];
            //Segment segment7 = (Segment)equal[0];
            //Segment segment8 = (Segment)equal[1];
            //if ((segment7.Properties.TrueForAll(segment5.Properties.Contains) && segment8.Properties.TrueForAll(segment6.Properties.Contains)) ||
            //    segment7.Properties.TrueForAll(segment6.Properties.Contains) && segment8.Properties.TrueForAll(segment5.Properties.Contains))
            //{
            //    if ((segment1 == segment5 && segment2 == segment6) || (segment1 == segment6 && segment2 == segment5))
            //    {
            //        Parallelogram parallelogram = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
            //        parallelogram.AddReason();
            //        parallelogram.AddCondition(parallel, equal);
            //        if(parallelogram.StrContains("平行四边形ABQP"))
            //            Console.WriteLine();
            //        AddProcessor.Add(parallelogram);
            //    }
            //    if ((segment3 == segment5 && segment4 == segment6) || (segment3 == segment6 && segment4 == segment5))
            //    {
            //        Parallelogram parallelogram = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
            //        parallelogram.AddReason();
            //        parallelogram.AddCondition(parallel, equal);
            //        if (parallelogram.StrContains("平行四边形ABQP"))
            //            Console.WriteLine();
            //        AddProcessor.Add(parallelogram);
            //    }
            //}
        }
        public void RuleDQ002两组对边平行的四边形是平行四边形(Quadriliateral quadriliateral, LineParallel parallel1, LineParallel parallel2)
        {
            Line segment1 = KnowledgeGetter.GetLine((Point)quadriliateral[0], (Point)quadriliateral[1]);
            Line segment2 = KnowledgeGetter.GetLine((Point)quadriliateral[2], (Point)quadriliateral[3]);
            Line segment3 = KnowledgeGetter.GetLine((Point)quadriliateral[0], (Point)quadriliateral[3]);
            Line segment4 = KnowledgeGetter.GetLine((Point)quadriliateral[1], (Point)quadriliateral[2]);
            Line segment5 = (Line)parallel1[0];
            Line segment6 = (Line)parallel1[1];
            Line segment7 = (Line)parallel2[0];
            Line segment8 = (Line)parallel2[1];
            //flag为true就代表1，2有平行关系
            bool flag = (segment1 == segment5 && segment2 == segment6) || (segment1 == segment6 && segment2 == segment5) || (segment1 == segment7 && segment2 == segment8) || (segment1 == segment8 && segment2 == segment7);
            bool flag1 = (segment3 == segment5 && segment4 == segment6) || (segment3 == segment6 && segment4 == segment5) || (segment3 == segment7 && segment4 == segment8) || (segment3 == segment8 && segment4 == segment7);
            if (flag && flag1)
            {
                Parallelogram parallelogram = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
                parallelogram.AddReason();
                parallelogram.AddCondition(parallel1, parallel2);
                AddProcessor.Add(parallelogram);
            }

        }
        public void RuleDQ003两组对边相等的四边形是平行四边形(Quadriliateral quadriliateral, SegmentLengthEqual segmentLengthEqual, SegmentLengthEqual segmentLengthEqual1)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)quadriliateral[0], (Point)quadriliateral[1]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)quadriliateral[2], (Point)quadriliateral[3]);
            Segment segment3 = KnowledgeGetter.GetSegment((Point)quadriliateral[0], (Point)quadriliateral[3]);
            Segment segment4 = KnowledgeGetter.GetSegment((Point)quadriliateral[1], (Point)quadriliateral[2]);
            Segment segment5 = (Segment)segmentLengthEqual[0];
            Segment segment6 = (Segment)segmentLengthEqual[1];

            Segment segment7 = (Segment)segmentLengthEqual1[0];
            Segment segment8 = (Segment)segmentLengthEqual1[1];
            //flag为true就代表1，2有相等关系
            bool flag = (segment1 == segment5 && segment2 == segment6) || (segment1 == segment6 && segment2 == segment5) || (segment1 == segment7 && segment2 == segment8) || (segment1 == segment8 && segment2 == segment7);
            bool flag1 = (segment3 == segment5 && segment4 == segment6) || (segment3 == segment6 && segment4 == segment5) || (segment3 == segment7 && segment4 == segment8) || (segment3 == segment8 && segment4 == segment7);
            if (flag && flag1)
            {
                Parallelogram parallelogram = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
                parallelogram.AddReason();
                parallelogram.AddCondition(segmentLengthEqual, segmentLengthEqual1);
                AddProcessor.Add(parallelogram);
            }
        }

        public void RuleDQ004对角相等的四边形是平行四边形(Quadriliateral quadriliateral, AngleSizeEqual angleSizeEqual, AngleSizeEqual angleSizeEqual1)
        {
            return;
            //    "parallelogram_judgment_angle_and_angle(ABCD)": {
            //        "1": {
            //            "premise": "Polygon(ABCD)&Equal(MeasureOfAngle(DAB),MeasureOfAngle(BCD))&Equal(MeasureOfAngle(ABC),MeasureOfAngle(CDA))",
            //  "conclusion": [
            //    "Parallelogram(ABCD)"
            //          ]
            //}
            //    },
            Angle angle1 = KnowledgeGetter.GetAngle((Point)quadriliateral[1], (Point)quadriliateral[0], (Point)quadriliateral[3]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
            Angle angle3 = KnowledgeGetter.GetAngle((Point)quadriliateral[0], (Point)quadriliateral[3], (Point)quadriliateral[2]);
            Angle angle4 = KnowledgeGetter.GetAngle((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2]);

            Angle angle5 = (Angle)angleSizeEqual[0];
            Angle angle6 = (Angle)angleSizeEqual[1];

            Angle angle7 = (Angle)angleSizeEqual1[0];
            Angle angle8 = (Angle)angleSizeEqual1[1];

            //flag为true就代表1，2有平行关系
            bool flag = (angle1 == angle5 && angle2 == angle6) || (angle1 == angle6 && angle2 == angle5) || (angle1 == angle7 && angle2 == angle8) || (angle1 == angle8 && angle2 == angle7);
            bool flag1 = (angle3 == angle5 && angle4 == angle6) || (angle3 == angle6 && angle4 == angle5) || (angle3 == angle7 && angle4 == angle8) || (angle3 == angle8 && angle4 == angle7);
            if (flag && flag1)
            {
                Parallelogram parallelogram = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
                parallelogram.AddReason();
                parallelogram.AddCondition(angleSizeEqual, angleSizeEqual1);
                AddProcessor.Add(parallelogram);
            }

        }

        public void RuleDQ005对角线互相平分的四边形是平行四边形(Quadriliateral quadriliateral, Midpoint midpoint, Midpoint midpoint1)
        {
            //    "parallelogram_judgment_diagonal_bisection(ABCD,O)": {
            //        "1": {
            //            "premise": "Polygon(ABCD)&Collinear(AOC)&Collinear(BOD)&IsMidpointOfLine(O,AC)&IsMidpointOfLine(O,BD)",
            //  "conclusion": [
            //    "Parallelogram(ABCD)"
            //          ]
            //}
            //    },
            Point point = (Point)midpoint[0];
            Point point1 = (Point)midpoint1[0];
            if (point != point1)
            {
                return;
            }
            Segment segment1 = KnowledgeGetter.GetSegment((Point)quadriliateral[0], (Point)quadriliateral[2]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)quadriliateral[1], (Point)quadriliateral[3]);

            Segment segment3 =GetSegment(midpoint[1], midpoint[2]);
            Segment segment4 = GetSegment(midpoint1[1], midpoint1[2]);

            if ((segment3 == segment1 && segment4 == segment2) || (segment3 == segment2 && segment4 == segment1))
            {
                Parallelogram parallelogram = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
                parallelogram.AddReason();
                parallelogram.AddCondition(quadriliateral, midpoint, midpoint1);
                AddProcessor.Add(parallelogram);
            }

        }

        public void RuleDQ006有一个直角的平行四边形是矩形(Parallelogram parallelogram, AngleSize angleSize)
        {
            Expr expr1 = angleSize.Expr;
            Angle angle = (Angle)angleSize[0];
            if (expr1.CompareTo(90) == ExprCompareResult.Equal)
            {
                return;
            }
            var result = this.FindIntersection(parallelogram.Properties, angle.Properties);
            if (result.intersection.Count != 3)
            {
                return;
            }
            Rectangle rectangle = new Rectangle((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
            rectangle.AddReason();
            rectangle.AddCondition(parallelogram, angleSize);
            AddProcessor.Add(rectangle);
        }
        public void RuleDQ007对角线相等的平行四边形是矩形(Parallelogram parallelogram, SegmentLengthEqual segmentLengthEqual)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)parallelogram[0], (Point)parallelogram[2]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)parallelogram[1], (Point)parallelogram[3]);
            //没有连接对角线
            if (segment1 is null || segment2 is null) return;
            Segment segment3 = (Segment)segmentLengthEqual[0];
            Segment segment4 = (Segment)segmentLengthEqual[1];
            if ((segment1 == segment3 && segment2 == segment4) || (segment1 == segment4 && segment2 == segment3))
            {
                Rectangle rectangle = new Rectangle((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
                rectangle.AddReason();
                rectangle.AddCondition(parallelogram, segmentLengthEqual);
                AddProcessor.Add(rectangle);
            }
        }

        public void RuleDQ008两组邻边相等的四边形是筝形(Polygon polygon, SegmentLengthEqual segmentLengthEqual, SegmentLengthEqual segmentLengthEqual1)
        {
            //  "kite_judgment_equal_and_equal(ABCD)": {
            //      "1": {
            //          "premise": "Polygon(ABCD)&Equal(LengthOfLine(AB),LengthOfLine(AD))&Equal(LengthOfLine(CB),LengthOfLine(CD))",
            //"conclusion": [
            //  "Kite(ABCD)"
            //        ]

            //},
            Segment segment1 = KnowledgeGetter.GetSegment((Point)polygon[0], (Point)polygon[1]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)polygon[0], (Point)polygon[3]);
            Segment segment3 = KnowledgeGetter.GetSegment((Point)polygon[1], (Point)polygon[2]);
            Segment segment4 = KnowledgeGetter.GetSegment((Point)polygon[2], (Point)polygon[3]);

            Segment segment5 = (Segment)segmentLengthEqual[0];
            Segment segment6 = (Segment)segmentLengthEqual[1];

            Segment segment7 = (Segment)segmentLengthEqual[0];
            Segment segment8 = (Segment)segmentLengthEqual[1];
        }


        public void RuleDQ009既是筝形又是平行四边形的四边形是菱形(Kite kite, Parallelogram parallelogram)
        {
            //    "rhombus_judgment_parallelogram_and_kite(ABCD)": {
            //        "1": {
            //            "premise": "Parallelogram(ABCD)&Kite(ABCD)",
            //  "conclusion": [
            //    "Rhombus(ABCD)"
            //          ]
            //}
            //    },
            var result = this.FindIntersection(kite.Properties, parallelogram.Properties);
            if (result.intersection.Count == 4)
            {
                Rhombus rhombus = new Rhombus((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
                rhombus.AddReason();
                rhombus.AddCondition(kite, parallelogram);
                AddProcessor.Add(rhombus);
            }
        }
        public void RuleDQ010既是菱形又是矩形的四边形是正方形(Rhombus rhombus, Rectangle rectangle)
        {
            //    "square_judgment_rhombus_and_rectangle(ABCD)": {
            //        "1": {
            //            "premise": "Rhombus(ABCD)&Rectangle(ABCD)",
            //  "conclusion": [
            //    "Square(ABCD)"
            //          ]
            //}
            //    },
            var result = this.FindIntersection(rhombus.Properties, rectangle.Properties);
            if (result.intersection.Count == 4)
            {
                Square square = new Square((Point)rhombus[0], (Point)rhombus[1], (Point)rhombus[2], (Point)rhombus[3]);
                square.AddReason();
                square.AddCondition(rhombus, rectangle);
                AddProcessor.Add(square);
            }
        }

        #region 梯形
        public void RuleDQ011两腰相等的梯形形是等腰梯形(Trapezoid trapezoid, SegmentLengthEqual segmentLengthEqual)
        {
            Segment segment1 = (Segment)segmentLengthEqual[0];
            Segment segment2 = (Segment)segmentLengthEqual[1];
            Segment segment3 = KnowledgeGetter.GetSegment((Point)trapezoid[0], (Point)trapezoid[3]);
            Segment segment4 = KnowledgeGetter.GetSegment((Point)trapezoid[1], (Point)trapezoid[2]);
            if ((segment3 == segment1 && segment4 == segment2) || (segment3 == segment2 && segment4 == segment1))
            {
                IsoscelesTrapezoid isoscelesTrapezoid = new IsoscelesTrapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
                isoscelesTrapezoid.AddReason();
                isoscelesTrapezoid.AddCondition(trapezoid, segmentLengthEqual);
                AddProcessor.Add(isoscelesTrapezoid);
            }

        }
        public void RuleDQ012两脚相等的梯形是等腰梯形(Trapezoid trapezoid, AngleSizeEqual angleSizeEqual)
        {
            Angle angle1 = (Angle)angleSizeEqual[0];
            Angle angle2 = (Angle)angleSizeEqual[1];
            Angle angle3 = KnowledgeGetter.GetAngle((Point)trapezoid[0], (Point)trapezoid[3], (Point)trapezoid[2]);
            Angle angle4 = KnowledgeGetter.GetAngle((Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            if ((angle3 == angle1 && angle4 == angle2) || (angle3 == angle2 && angle4 == angle1))
            {
                IsoscelesTrapezoid isoscelesTrapezoid = new IsoscelesTrapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
                isoscelesTrapezoid.AddReason();
                isoscelesTrapezoid.AddCondition(trapezoid, angleSizeEqual);
                AddProcessor.Add(isoscelesTrapezoid);
            }

        }
        public void RuleDQ013等腰梯形的对角线判断(Trapezoid trapezoid, SegmentLengthEqual segmentLengthEqual)
        {
            Segment segment1 = (Segment)segmentLengthEqual[0];
            Segment segment2 = (Segment)segmentLengthEqual[1];
            Segment segment3 = KnowledgeGetter.GetSegment((Point)trapezoid[0], (Point)trapezoid[2]);
            Segment segment4 = KnowledgeGetter.GetSegment((Point)trapezoid[1], (Point)trapezoid[3]);
            if ((segment3 == segment1 && segment4 == segment2) || (segment3 == segment2 && segment4 == segment1))
            {
                IsoscelesTrapezoid isoscelesTrapezoid = new IsoscelesTrapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
                isoscelesTrapezoid.AddReason();
                isoscelesTrapezoid.AddCondition(trapezoid, segmentLengthEqual);
                AddProcessor.Add(isoscelesTrapezoid);
            }
        }
        #endregion
    }
}
