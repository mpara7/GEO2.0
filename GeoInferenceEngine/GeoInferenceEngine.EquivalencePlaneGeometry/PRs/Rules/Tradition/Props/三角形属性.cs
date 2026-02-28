namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Props
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 三角形属性L : PlaneRuleClass
    {
        public void RulePT001直角三角形的一个角为90度(RightTriangle rightTriangle)
        {
            Angle angle = KnowledgeGetter.GetAngle((Point)rightTriangle[1], (Point)rightTriangle[0], (Point)rightTriangle[2]);
            if (angle is null) return;
            AngleSize angleSize = new AngleSize(angle, 90);
            angleSize.AddReason();
            angleSize.AddCondition(rightTriangle);
            AddProcessor.Add(angleSize);
        }

        public void RulePT002等腰三角形的两个角相等(IsoscelesTriangle isoscelesTriangle)
        {
            Angle angle = KnowledgeGetter.GetAngle((Point)isoscelesTriangle[0], (Point)isoscelesTriangle[1], (Point)isoscelesTriangle[2]);
            Angle angle1 = KnowledgeGetter.GetAngle((Point)isoscelesTriangle[0], (Point)isoscelesTriangle[2], (Point)isoscelesTriangle[1]);

            AngleSizeEqual angleSizeEqual = new AngleSizeEqual(angle, angle1);
            angleSizeEqual.AddReason();
            angleSizeEqual.AddCondition(isoscelesTriangle);
            AddProcessor.Add(angleSizeEqual);
        }
        public void RulePT003等腰三角形两腰相等(IsoscelesTriangle isoscelesTriangle)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)isoscelesTriangle[0], (Point)isoscelesTriangle[1]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)isoscelesTriangle[0], (Point)isoscelesTriangle[2]);
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment1, segment2);
            segmentLengthEqual.AddReason();
            segmentLengthEqual.AddCondition(isoscelesTriangle);
            AddProcessor.Add(segmentLengthEqual);
        }

        public void RulePT004等腰直角三角形的两个角为45度(IsoscelesRightTriangle isoscelesRightTriangle)
        {
            Angle angle1 = KnowledgeGetter.GetAngle((Point)isoscelesRightTriangle[0], (Point)isoscelesRightTriangle[1], (Point)isoscelesRightTriangle[2]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)isoscelesRightTriangle[0], (Point)isoscelesRightTriangle[2], (Point)isoscelesRightTriangle[1]);
            AngleSize angleSize1 = new AngleSize(angle1, 45);
            AngleSize angleSize2 = new AngleSize(angle2, 45);
            angleSize1.AddReason();
            angleSize1.AddCondition(isoscelesRightTriangle);
            angleSize2.AddReason();
            angleSize2.AddCondition(isoscelesRightTriangle);
            AddProcessor.Add(angleSize1);
            AddProcessor.Add(angleSize2);
        }

        public void RulePT005等边三角形的任意两边相等(EquilateralTriangle equilateralTriangle)
        {
            Segment segment1 = KnowledgeGetter.GetSegment((Point)equilateralTriangle[0], (Point)equilateralTriangle[1]);
            Segment segment2 = KnowledgeGetter.GetSegment((Point)equilateralTriangle[0], (Point)equilateralTriangle[2]);
            Segment segment3 = KnowledgeGetter.GetSegment((Point)equilateralTriangle[1], (Point)equilateralTriangle[2]);
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment1, segment2);
            SegmentLengthEqual segmentLengthEqual1 = new SegmentLengthEqual(segment1, segment3);
            SegmentLengthEqual segmentLengthEqual2 = new SegmentLengthEqual(segment2, segment3);
            segmentLengthEqual.AddReason();
            segmentLengthEqual.AddCondition(equilateralTriangle);
            segmentLengthEqual1.AddReason();
            segmentLengthEqual1.AddCondition(equilateralTriangle);
            segmentLengthEqual2.AddReason();
            segmentLengthEqual2.AddCondition(equilateralTriangle);
            AddProcessor.Add(segmentLengthEqual);
            AddProcessor.Add(segmentLengthEqual1);
            AddProcessor.Add(segmentLengthEqual2);

        }
        public void RulePT006等边三角形的任意两角相等(EquilateralTriangle equilateralTriangle)
        {
            Angle angle = KnowledgeGetter.GetAngle((Point)equilateralTriangle[0], (Point)equilateralTriangle[1], (Point)equilateralTriangle[2]);
            Angle angle1 = KnowledgeGetter.GetAngle((Point)equilateralTriangle[1], (Point)equilateralTriangle[0], (Point)equilateralTriangle[2]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)equilateralTriangle[0], (Point)equilateralTriangle[2], (Point)equilateralTriangle[1]);
            AngleSizeEqual angleSizeEqual = new AngleSizeEqual(angle, angle1);
            AngleSizeEqual angleSizeEqual1 = new AngleSizeEqual(angle, angle2);
            AngleSizeEqual angleSizeEqual2 = new AngleSizeEqual(angle1, angle2);
            angleSizeEqual.AddReason();
            angleSizeEqual.AddCondition(equilateralTriangle);
            angleSizeEqual1.AddReason();
            angleSizeEqual1.AddCondition(equilateralTriangle);
            angleSizeEqual2.AddReason();
            angleSizeEqual2.AddCondition(equilateralTriangle);
            AddProcessor.Add(angleSizeEqual);
            AddProcessor.Add(angleSizeEqual1);
            AddProcessor.Add(angleSizeEqual2);
        }
        public void RulePT007等边三角形的所有角都为60度(EquilateralTriangle equilateralTriangle)
        {
            Angle angle = KnowledgeGetter.GetAngle((Point)equilateralTriangle[0], (Point)equilateralTriangle[1], (Point)equilateralTriangle[2]);
            Angle angle1 = KnowledgeGetter.GetAngle((Point)equilateralTriangle[1], (Point)equilateralTriangle[0], (Point)equilateralTriangle[2]);
            Angle angle2 = KnowledgeGetter.GetAngle((Point)equilateralTriangle[0], (Point)equilateralTriangle[2], (Point)equilateralTriangle[1]);
            AngleSize angleSize = new AngleSize(angle, 60);
            AngleSize angleSize1 = new AngleSize(angle1, 60);
            AngleSize angleSize2 = new AngleSize(angle2, 60);
            angleSize.AddReason();
            angleSize1.AddReason();
            angleSize2.AddReason();
            angleSize.AddCondition(equilateralTriangle);
            angleSize1.AddCondition(equilateralTriangle);
            angleSize2.AddCondition(equilateralTriangle);
            AddProcessor.Add(angleSize);
            AddProcessor.Add(angleSize1);
            AddProcessor.Add(angleSize2);
        }
    }
}
