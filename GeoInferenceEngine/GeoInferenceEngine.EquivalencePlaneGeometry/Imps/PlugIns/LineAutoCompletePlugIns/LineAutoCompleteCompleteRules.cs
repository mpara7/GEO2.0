using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using System.Net;

namespace GeoInferenceEngine.PlaneGeometry.Rules
{
    [Description("连线信息设定")]
    public class LineAutoCompleteConfig : AInferenceSetting
    {
        public bool IsAutoConnect { get; set; } = true;
    }
    [PlaneRuleType(PlaneRuleType.CoLine)]
    public class LineAutoCompleteCompleteRules : PlaneRuleClass
    {
        [ZDI]
        LineAutoCompleteConfig config;
        public void RuleCL000线段得连线(Segment segment)
        {
            var pred = new Line((Point)segment[0], (Point)segment[1]);
            pred.AddReason();
            pred.AddCondition(segment);
            AddProcessor.Add(pred);
        }
        public void RuleCL001三角形得连线关系(Triangle triangle)
        {
            List<Knowledge> knowledge = new List<Knowledge>();
            knowledge.Add(new Segment((Point)triangle[0], (Point)triangle[1]));
            knowledge.Add(new Segment((Point)triangle[1], (Point)triangle[2]));
            knowledge.Add(new Segment((Point)triangle[0], (Point)triangle[2]));
            knowledge.Add(new Line((Point)triangle[0], (Point)triangle[1]));
            knowledge.Add(new Line((Point)triangle[1], (Point)triangle[2]));
            knowledge.Add(new Line((Point)triangle[0], (Point)triangle[2]));
            foreach (var k in knowledge)
            {
                k.AddReason();
                k.AddCondition(triangle);
                AddProcessor.Add(k);
            }
        }
        public void RuleCL002四边形得连线关系(Quadriliateral quad)
        {
            List<Knowledge> knowledge = new List<Knowledge>();
            knowledge.Add(new Segment((Point)quad[0], (Point)quad[1]));
            knowledge.Add(new Segment((Point)quad[1], (Point)quad[2]));
            knowledge.Add(new Segment((Point)quad[2], (Point)quad[3]));
            knowledge.Add(new Segment((Point)quad[3], (Point)quad[0]));
            knowledge.Add(new Line((Point)quad[0], (Point)quad[1]));
            knowledge.Add(new Line((Point)quad[1], (Point)quad[2]));
            knowledge.Add(new Line((Point)quad[2], (Point)quad[3]));
            knowledge.Add(new Line((Point)quad[3], (Point)quad[0]));
            foreach (var k in knowledge)
            {
                k.AddReason();
                k.AddCondition(quad);
                AddProcessor.Add(k);
            }
        }
        public void RuleCL003点在线上得点共线(PointWithInPoints pb)
        {
            Line pred = new Line((Point)pb[0], (Point)pb[1], (Point)pb[2]);
            pred.AddReason();
            pred.AddCondition(pb);
            AddProcessor.Add(pred);
        }
        public void RuleCL004得到线段交点得到共线(LineIntersectionPoint lp)
        {
            List<Point> list1 = new List<Point>(lp[1].Properties.Select(p => (Point)p));
            list1.Add((Point)lp[0]);
            Line pred = new Line(list1.Distinct().ToArray());

            pred.AddReason();
            pred.AddCondition(lp);
            AddProcessor.Add(pred);

            List<Point> list2 = new List<Point>(lp[2].Properties.Select(p => (Point)p));
            list2.Add((Point)lp[0]);
            Line pred2 = new Line(list2.Distinct().ToArray());

            pred2.AddReason();
            pred2.AddCondition(lp);
            AddProcessor.Add(pred2);
        }
        public void RuleCL005任意两点成线段与共线(Point p1, Point p2)
        {
            //是全连接
            if (config.IsAutoConnect)
            {
                Segment segment = new Segment(p1, p2);
                segment.AddReason();
                //segment.AddCondition(p1, p2);
                AddProcessor.Add(segment);

                Line line = new Line(p1, p2);
                line.AddReason();
                //line.AddCondition(p1, p2);
                AddProcessor.Add(line);
            }
            else
            {
                if (KnowledgeGetter.HasSegment(p1, p2))
                {
                    KnowledgeGetter.Connections.Add((p1, p2));

                    Segment segment = new Segment(p1, p2);
                    segment.AddReason();
                    //segment.AddCondition(p1, p2);
                    AddProcessor.Add(segment);
                    var hadline = KnowledgeGetter.GetLine(p1, p2);
                    if (hadline is not null) return;
                    Line line = new Line(p1, p2);
                    line.AddReason();
                    //line.AddCondition(p1, p2);
                    AddProcessor.Add(line);
                }
            }
        }
    }
}
