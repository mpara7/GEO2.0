using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Props
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 四边形属性 : PlaneRuleClass
    {
        public void RulePQ001梯形得平行(Trapezoid isoscelesTrapezoid)
        {
            var line1 = KnowledgeGetter.GetLine(isoscelesTrapezoid[0], isoscelesTrapezoid[1]);
            var line3 = KnowledgeGetter.GetLine(isoscelesTrapezoid[2], isoscelesTrapezoid[3]);

            LineParallel pred = new LineParallel(line1, line3);
            pred.AddReason();
            pred.AddCondition(isoscelesTrapezoid);
            AddProcessor.Add(pred);
        }
        public void RulePQ002等腰梯形的对角线相等(IsoscelesTrapezoid isoscelesTrapezoid)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)isoscelesTrapezoid[0], (Point)isoscelesTrapezoid[2]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)isoscelesTrapezoid[1], (Point)isoscelesTrapezoid[3]);
            if (segment1 is null && segment2 is null) return;
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment1, segment2);
            segmentLengthEqual.AddReason();
            segmentLengthEqual.AddCondition(isoscelesTrapezoid);
            AddProcessor.Add(segmentLengthEqual);

            //上底
            Angle angle1 = KnowledgeGetter.GetAngle((Point)isoscelesTrapezoid[0], (Point)isoscelesTrapezoid[3], (Point)isoscelesTrapezoid[2]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)isoscelesTrapezoid[1], (Point)isoscelesTrapezoid[2], (Point)isoscelesTrapezoid[3]);
            AngleSizeEqual angleSizeEqual = new AngleSizeEqual(angle1, angle2);
            angleSizeEqual.AddReason();
            angleSizeEqual.AddCondition(isoscelesTrapezoid);
            AddProcessor.Add(angleSizeEqual);
            //下底
            Angle angle3 = KnowledgeGetter.GetAngle((Point)isoscelesTrapezoid[3], (Point)isoscelesTrapezoid[0], (Point)isoscelesTrapezoid[1]);
            Angle angle4 = KnowledgeGetter.GetAngle((Point)isoscelesTrapezoid[0], (Point)isoscelesTrapezoid[1], (Point)isoscelesTrapezoid[2]);
            angleSizeEqual = new AngleSizeEqual(angle3, angle4);
            angleSizeEqual.AddReason();
            angleSizeEqual.AddCondition(isoscelesTrapezoid);
            AddProcessor.Add(angleSizeEqual);
        }
        public void RulePQ003直角梯形的(RightTrapezoid isoscelesTrapezoid)
        {
            var line1 = KnowledgeGetter.GetLine(isoscelesTrapezoid[0], isoscelesTrapezoid[1]);
            var line2 = KnowledgeGetter.GetLine(isoscelesTrapezoid[1], isoscelesTrapezoid[2]);
            var line3 = KnowledgeGetter.GetLine(isoscelesTrapezoid[2], isoscelesTrapezoid[3]);

            LinePerpendicular pred = new LinePerpendicular(line1, line2);
            pred.AddReason();
            pred.AddCondition(isoscelesTrapezoid);
            AddProcessor.Add(pred);

            LinePerpendicular pred2 = new LinePerpendicular(line3, line2);
            pred2.AddReason();
            pred2.AddCondition(isoscelesTrapezoid);
            AddProcessor.Add(pred2);
        }

        public void RulePQ004筝形有一条对角线是另一条对角线的垂直平方线(Kite kite)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)kite[1], (Point)kite[3]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)kite[0], (Point)kite[2]);
            if (segment1 is null && segment2 is null) return;
            //segment2是被平分的线段
            PointOnVerticalBisector verticalBisector = new PointOnVerticalBisector(segment2, (Point)segment1[0], (Point)segment1[1]);
            verticalBisector.AddReason();
            verticalBisector.AddCondition(kite);
            AddProcessor.Add(kite);

        }
        public void RulePQ005筝形有组对角相等(Kite kite)
        {
            // 获取对角
            Angle angle1 = KnowledgeGetter.GetAngle((Point)kite[1], (Point)kite[0], (Point)kite[3]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)kite[1], (Point)kite[0], (Point)kite[3]);
            if (angle1 is null && angle2 is null) return;
            AngleSizeEqual angleSizeEqual = new AngleSizeEqual(angle1, angle2);
            angleSizeEqual.AddReason();
            angleSizeEqual.AddCondition(kite);
            AddProcessor.Add(angleSizeEqual);

        }
        public void RulePQ006筝形有组两组线段长度相等(Kite kite)
        {
            Segment ab = KnowledgeGetter.GetSegment((Point)kite[0], (Point)kite[1]);
            Segment bc = KnowledgeGetter.GetSegment((Point)kite[1], (Point)kite[2]);
            Segment cd = KnowledgeGetter.GetSegment((Point)kite[2], (Point)kite[3]);
            Segment da = KnowledgeGetter.GetSegment((Point)kite[3], (Point)kite[0]);
            SegmentLengthEqual pred = new SegmentLengthEqual(ab, da);
            pred.AddReason();
            pred.AddCondition(kite);
            AddProcessor.Add(kite);
            SegmentLengthEqual pred2 = new SegmentLengthEqual(bc, cd);
            pred2.AddReason();
            pred2.AddCondition(kite);
            AddProcessor.Add(kite);

        }
        public void RulePQ007平行四边形的对边平行且相等(Parallelogram parallelogram)
        {
            //1,2 3,4是两组对边
            Segment segment1 = KnowledgeGetter.GetSegment((Point)parallelogram[0], (Point)parallelogram[1]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)parallelogram[2], (Point)parallelogram[3]);
            Segment segment3 = KnowledgeGetter.GetSegment((Point)parallelogram[0], (Point)parallelogram[3]);
            Segment segment4 = KnowledgeGetter.GetSegment((Point)parallelogram[1], (Point)parallelogram[2]);
            Line l1 = KnowledgeGetter.GetLine((Point)parallelogram[0], (Point)parallelogram[1]);
            Line l2 = KnowledgeGetter.GetLine((Point)parallelogram[2], (Point)parallelogram[3]);
            Line l3 = KnowledgeGetter.GetLine((Point)parallelogram[0], (Point)parallelogram[3]);
            Line l4 = KnowledgeGetter.GetLine((Point)parallelogram[1], (Point)parallelogram[2]);
            LineParallel segmentParallel = new LineParallel(l1, l2);
            LineParallel segmentParallel1 = new LineParallel(l3, l4);
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment1, segment2);
            SegmentLengthEqual segmentLengthEqual1 = new SegmentLengthEqual(segment3, segment4);
            segmentParallel.AddReason();
            segmentParallel1.AddReason();
            segmentLengthEqual.AddReason();
            segmentLengthEqual1.AddReason();
            segmentParallel.AddCondition(parallelogram);
            segmentParallel1.AddCondition(parallelogram);
            segmentLengthEqual.AddCondition(parallelogram);
            segmentLengthEqual1.AddCondition(parallelogram);
            AddProcessor.Add(segmentParallel);
            AddProcessor.Add(segmentParallel1);
            AddProcessor.Add(segmentLengthEqual);
            AddProcessor.Add(segmentLengthEqual1);
        }
        public void RulePQ008平行四边形的对角相等(Parallelogram parallelogram)
        {
            Angle angle1 = KnowledgeGetter.GetAngle((Point)parallelogram[1], (Point)parallelogram[0], (Point)parallelogram[3]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
            Angle angle3 = KnowledgeGetter.GetAngle((Point)parallelogram[0], (Point)parallelogram[3], (Point)parallelogram[2]);
            Angle angle4 = KnowledgeGetter.GetAngle((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2]);
            AngleSizeEqual angleSizeEqual = new AngleSizeEqual(angle1, angle2);
            AngleSizeEqual angleSizeEqual1 = new AngleSizeEqual(angle3, angle4);
            angleSizeEqual.AddReason();
            angleSizeEqual.AddCondition(parallelogram);
            AddProcessor.Add(angleSizeEqual);
            angleSizeEqual1.AddReason();
            angleSizeEqual1.AddCondition(parallelogram);
            AddProcessor.Add(angleSizeEqual1);
        }
        /// <summary>
        /// / new ("平行四边形","平行四边形(ABCD)&共线(AOC)",["中点(OAC)"]),
        /// </summary>
        /// <param name="parallelogram"></param>
        /// <param name="lineIntersectionPoint"></param>
        public void RulePQ009平行四边形对角线互相平分(Parallelogram parallelogram, LineIntersectionPoint lineIntersectionPoint)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)parallelogram[0], (Point)parallelogram[2]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)parallelogram[1], (Point)parallelogram[3]);
            //没有连接对角线
            if (segment1 is null || segment2 is null) return;
            Line segment3 = (Line)lineIntersectionPoint[1];
            Line segment4 = (Line)lineIntersectionPoint[2];
            if ((segment1 == segment3 && segment2 == segment4) || (segment1 == segment4 && segment2 == segment3))
            {
                Point point = (Point)lineIntersectionPoint[0];
                Midpoint midpoint = new Midpoint(point, (Point)parallelogram[0], (Point)parallelogram[2]);
                Midpoint midpoint1 = new Midpoint(point, (Point)parallelogram[1], (Point)parallelogram[3]);
                midpoint.AddReason();
                midpoint.AddCondition(parallelogram, lineIntersectionPoint);
                AddProcessor.Add(midpoint);
                midpoint1.AddReason();
                midpoint1.AddCondition(parallelogram, lineIntersectionPoint);
                AddProcessor.Add(midpoint1);

            }
        }



        public void RulePQ010菱形的邻边相等(Rhombus rhombus)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)rhombus[0], (Point)rhombus[1]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)rhombus[1], (Point)rhombus[2]);
            Segment segment3 = KnowledgeGetter.GetSegment((Point)rhombus[2], (Point)rhombus[3]);
            Segment segment4 = KnowledgeGetter.GetSegment((Point)rhombus[3], (Point)rhombus[0]);
            List<SegmentLengthEqual> preds = new List<SegmentLengthEqual>();
            preds.Add(new SegmentLengthEqual(segment1, segment2));
            preds.Add(new SegmentLengthEqual(segment2, segment3));
            preds.Add(new SegmentLengthEqual(segment3, segment4));
            preds.Add(new SegmentLengthEqual(segment4, segment1));
            foreach (var pred in preds)
            {
                pred.AddReason();
                pred.AddCondition(rhombus);
                AddProcessor.Add(pred);
            }
        }
        public void RulePQ011菱形的对角线垂直平分(Rhombus rhombus)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)rhombus[0], (Point)rhombus[2]);
            if (segment1 is not null)
            {
                PointOnVerticalBisector pred = new PointOnVerticalBisector(segment1, (Point)rhombus[1], (Point)rhombus[3]);
                pred.AddReason();
                pred.AddCondition(rhombus);
                AddProcessor.Add(pred);
            }
            Segment segment2 = KnowledgeGetter.GetSegment((Point)rhombus[1], (Point)rhombus[3]);
            if (segment2 is not null)
            {
                PointOnVerticalBisector pred = new PointOnVerticalBisector(segment1, (Point)rhombus[0], (Point)rhombus[2]);
                pred.AddReason();
                pred.AddCondition(rhombus);
                AddProcessor.Add(pred);
            }

        }

        public void RulePQ012菱形的对角线在角平分线上(Rhombus rhombus)
        {
            for (int i = 0; i < 4; i++)
            {
                Angle segment1 = KnowledgeGetter.GetAngle((Point)rhombus[0 + i % 4], (Point)rhombus[(1 + i) % 4], (Point)rhombus[(2 + i) % 4]);
                PointOnAngularBisector pred = new PointOnAngularBisector(segment1, (Point)rhombus[(3 + i) % 4]);
                pred.AddReason();
                pred.AddCondition(rhombus);
                AddProcessor.Add(pred);
            }

        }
        public void RulePQ013矩形的邻边垂直(Rectangle rectangle)
        {
            Line segment1 = KnowledgeGetter.GetLine((Point)rectangle[0], (Point)rectangle[1]);
            Line segment2 = KnowledgeGetter.GetLine((Point)rectangle[1], (Point)rectangle[2]);
            Line segment3 = KnowledgeGetter.GetLine((Point)rectangle[2], (Point)rectangle[3]);
            Line segment4 = KnowledgeGetter.GetLine((Point)rectangle[3], (Point)rectangle[0]);
            List<LinePerpendicular> preds = new List<LinePerpendicular>();
            preds.Add(new LinePerpendicular(segment1, segment2));
            preds.Add(new LinePerpendicular(segment2, segment3));
            preds.Add(new LinePerpendicular(segment3, segment4));
            preds.Add(new LinePerpendicular(segment4, segment1));
            foreach (var pred in preds)
            {
                pred.AddReason();
                pred.AddCondition(rectangle);
                AddProcessor.Add(pred);
            }
        }
        public void RulePQ014矩形的对角线相等(Rectangle rectangle)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)rectangle[0], (Point)rectangle[2]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)rectangle[1], (Point)rectangle[3]);
            if (segment1 is null && segment2 is null) return;
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment1, segment2);
            segmentLengthEqual.AddReason();
            segmentLengthEqual.AddCondition(rectangle);
            AddProcessor.Add(segmentLengthEqual);
        }

    }
}
