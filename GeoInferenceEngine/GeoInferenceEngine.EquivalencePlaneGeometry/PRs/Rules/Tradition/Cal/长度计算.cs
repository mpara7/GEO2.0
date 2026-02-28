using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Cal
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 长度计算 : PlaneRuleClass
    {
        public void RuleCL001中点得线段长度相等(Midpoint midpoint)
        {
            var ab = KnowledgeGetter.GetSegment(midpoint.Point, midpoint.EndPoint1);
            var bc = KnowledgeGetter.GetSegment(midpoint.Point, midpoint.EndPoint2);
            var ac = KnowledgeGetter.GetSegment(midpoint.EndPoint1, midpoint.EndPoint2);
            SegmentLengthEqual pred = new SegmentLengthEqual(ab, bc);
            pred.AddReason();
            pred.AddCondition(midpoint);
            AddProcessor.Add(pred);

            SegmentLengthRatio pred1 = new SegmentLengthRatio(ac, ab,2);
            pred1.AddReason();
            pred1.AddCondition(midpoint);
            AddProcessor.Add(pred1);

            SegmentLengthRatio pred2 = new SegmentLengthRatio(ac, bc, 2);
            pred2.AddReason();
            pred2.AddCondition(midpoint);
            AddProcessor.Add(pred2);

        }
        public void RuleCL002点在线上得长度和(PointWithInPoints p)
        {
            var ab = KnowledgeGetter.GetSegment(p.Point, p.EndPoint1);
            var bc = KnowledgeGetter.GetSegment(p.Point, p.EndPoint2);
            var ac = KnowledgeGetter.GetSegment(p.EndPoint1, p.EndPoint2);
            var pred = new GeoEquation(ab.Length.Add(bc.Length), ac.Length);
            pred.AddReason();
            pred.AddCondition(p);
            AddProcessor.Add(pred);

        }
        public void RuleCL002共线得线段和(Line line)
        {
            for (var i = 1; i < line.Properties.Count; i++)
            {
                //内部
                for (var j = i + 1; j < line.Properties.Count; j++)
                {
                    var ab = KnowledgeGetter.GetSegment(line[0], line[i]);
                    var bc = KnowledgeGetter.GetSegment(line[i], line[j]);
                    var ac = KnowledgeGetter.GetSegment(line[0], line[j]);

                    var pred = new GeoEquation(ab.Length.Add(bc.Length), ac.Length);
                    pred.AddReason();
                    pred.AddCondition(line);
                    AddProcessor.Add(pred);
                }
            }
        }
        public void RuleCL003直角三角形斜边上的中线等于斜边的一半(RightTriangle rightTriangle, Midpoint midpoint)
        {
            Segment segment = KnowledgeGetter.GetSegment((Point)rightTriangle[1], (Point)rightTriangle[2]);
            Point point = (Point)midpoint[0];
            Segment segment1 =GetSegment(midpoint[1], midpoint[2]);
            if (segment != segment1)
            {
                return;
            }
            Segment segmnet2 = KnowledgeGetter.GetSegment((Point)rightTriangle[0], point);
            Segment segment3 = KnowledgeGetter.GetSegment(point, (Point)segment1[0]);
            Segment segment4 = KnowledgeGetter.GetSegment(point, (Point)segment1[1]);
            if (segmnet2 is not null && segment3 is not null)
            {
                var segmentLengthEqual = new SegmentLengthEqual(segmnet2, segment3);
                segmentLengthEqual.AddReason();
                segmentLengthEqual.AddCondition(rightTriangle, midpoint);
                AddProcessor.Add(segmentLengthEqual);
            }
            else if (segmnet2 is not null && segment4 is not null)
            {
                var segmentLengthEqual1 = new SegmentLengthEqual(segmnet2, segment4);
                segmentLengthEqual1.AddReason();
                segmentLengthEqual1.AddCondition(rightTriangle, midpoint);
                AddProcessor.Add(segmentLengthEqual1);
            }
        }
    }
}
