using GeoInferenceEngine.Knowledges;
using System.Collections.Generic;

namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Determine
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 三角形全等相似 : PlaneRuleClass
    {
        public void RuleT001三角形全等_SSS(SegmentLengthEqual equal1, SegmentLengthEqual equal2, SegmentLengthEqual equal3)
        {
            Segment seg11 = (Segment)equal1[0];
            Segment seg12 = (Segment)equal1[1];
            Segment seg21 = (Segment)equal2[0];
            Segment seg22 = (Segment)equal2[1];
            Segment seg31 = (Segment)equal3[0];
            Segment seg32 = (Segment)equal3[1];
            List<(List<Segment>, List<Segment>)> groups = new List<(List<Segment>, List<Segment>)>();
            groups.Add((new() { seg11, seg21, seg31 }, new() { seg12, seg22, seg32 }));
            groups.Add((new() { seg11, seg21, seg32 }, new() { seg12, seg22, seg31 }));
            groups.Add((new() { seg11, seg22, seg31 }, new() { seg12, seg21, seg32 }));
            groups.Add((new() { seg11, seg22, seg32 }, new() { seg12, seg21, seg31 }));

            foreach (var group in groups)
            {
                var a = DictionaryTool.CountItemNum(group.Item1.SelectMany(s => s.Properties));
                var b = DictionaryTool.CountItemNum(group.Item2.SelectMany(s => s.Properties));
                if (a.ToList().TrueForAll(s => s.Value == 2) && b.ToList().TrueForAll(s => s.Value == 2))
                {
                    //找对应
                    var pa1 = a.Keys.First(p => !group.Item1[0].Properties.Contains(p));
                    var pb1 = b.Keys.First(p => !group.Item2[0].Properties.Contains(p));

                    var pa2 = a.Keys.First(p => !group.Item1[1].Properties.Contains(p));
                    var pb2 = b.Keys.First(p => !group.Item2[1].Properties.Contains(p));

                    var pa3 = a.Keys.First(p => !group.Item1[2].Properties.Contains(p));
                    var pb3 = b.Keys.First(p => !group.Item2[2].Properties.Contains(p));
                    //三角形共线
                    var line1 = KnowledgeGetter.GetColine(pa1, pa2, pa3);
                    var line2 = KnowledgeGetter.GetColine(pb1, pb2, pb3);
                    if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
                        return;
                    var triangle1 = KnowledgeGetter.GetTriangle((Point)pa1, (Point)pa2, (Point)pa3);
                    var triangle2 = KnowledgeGetter.GetTriangle((Point)pb1, (Point)pb2, (Point)pb3);
                    if (triangle1 is NegatingKnowledge || triangle2 is NegatingKnowledge) continue;
                    TriangleCongruence pred =
                    new TriangleCongruence((Point)pa1, (Point)pa2, (Point)pa3,
                                        (Point)pb1, (Point)pb2, (Point)pb3);
                    if (pred.Properties.Take(3).ToList().TrueForAll(p => pred.Properties.Skip(3).Contains(p)))
                        return;
                    pred.AddReason();
                    pred.AddCondition(equal1, equal2, equal3);
                    AddProcessor.Add(pred);
                }
            }

        }
        public void RuleT002三角形全等SAS(SegmentLengthEqual equal1, SegmentLengthEqual equal2, AngleSizeEqual angleEqual)
        {
            TriangleCongruence pred = null;
            Angle angle1 = (Angle)angleEqual[0];
            var aseg11 = KnowledgeGetter.GetSegment(angle1[0], angle1[1]);
            var aseg12 = KnowledgeGetter.GetSegment(angle1[2], angle1[1]);

            Angle angle2 = (Angle)angleEqual[1];
            var aseg21 = KnowledgeGetter.GetSegment(angle2[0], angle2[1]);
            var aseg22 = KnowledgeGetter.GetSegment(angle2[2], angle2[1]);

            if (aseg11 == equal1[0] && aseg12 == equal2[1] && aseg21 == equal2[0] && aseg22 == equal1[1])
            {
                var triangle1 = KnowledgeGetter.GetTriangle((Point)angle1[0], (Point)angle1[1], (Point)angle1[2]);
                var triangle2 = KnowledgeGetter.GetTriangle((Point)angle2[2], (Point)angle2[1], (Point)angle2[0]);
                if (triangle1 is null || triangle2 is null) return;
                pred = new TriangleCongruence((Point)angle1[0], (Point)angle1[1], (Point)angle1[2],
                    (Point)angle2[2], (Point)angle2[1], (Point)angle2[0]);
            }
            else
                return;
            var line1 = KnowledgeGetter.GetColine((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var line2 = KnowledgeGetter.GetColine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
                return;
            if (pred.Properties.Take(3).ToList().TrueForAll(p => pred.Properties.Skip(3).Contains(p)))
                return;
            pred.AddReason();
            pred.AddCondition(equal1, equal2, angleEqual);
            AddProcessor.Add(pred);
        }
        public void RuleT003三角形全等AAS(AngleSizeEqual angleSizeEqual1,AngleSizeEqual angleSizeEqual2,SegmentLengthEqual segmentLengthEqual)
        {
                List<List<Point>> angle11_combine = new();
                List<List<Point>> angle12_combine = new();
                List<List<Point>> angle21_combine = new();
                List<List<Point>> angle22_combine = new();

                Angle angle11 = (Angle)angleSizeEqual1[0];
                Angle angle12 = (Angle)angleSizeEqual1[1];

                Angle angle21 = (Angle)angleSizeEqual2[0];
                Angle angle22 = (Angle)angleSizeEqual2[1];

                foreach (Point item in angle11.Edge1)
                {
                    foreach (Point item1 in angle11.Edge2)
                    {
                        List<Point> tmp = new List<Point> { item, angle11.Vertex, item1 };
                        angle11_combine.Add(tmp);
                    }
                }

                foreach (Point item in angle12.Edge1)
                {
                    foreach (Point item1 in angle12.Edge2)
                    {
                        List<Point> tmp = new List<Point> { item, angle12.Vertex, item1 };
                        angle12_combine.Add(tmp);
                    }
                }

                foreach (Point item in angle21.Edge1)
                {
                    foreach (Point item1 in angle21.Edge2)
                    {
                        List<Point> tmp = new List<Point> { item, angle21.Vertex, item1 };
                        angle21_combine.Add(tmp);
                    }
                }

                foreach (Point item in angle22.Edge1)
                {
                    foreach (Point item1 in angle22.Edge2)
                    {
                        List<Point> tmp = new List<Point> { item, angle22.Vertex, item1 };
                        angle22_combine.Add(tmp);
                    }
                }
                var result1 = GetIntersectionWithThreeElements(angle11_combine, angle21_combine);
                var result2 = GetIntersectionWithThreeElements(angle11_combine, angle22_combine);
                var result3 = GetIntersectionWithThreeElements(angle12_combine, angle21_combine);
                var result4 = GetIntersectionWithThreeElements(angle12_combine, angle22_combine);
                if (result1.exists && result4.exists)
                {
                    List<Point> triangle1_points = result1.intersection;
                    List<Point> triangle2_points = result4.intersection;
                    Triangle triangle1 = KnowledgeGetter.GetTriangle(triangle1_points[0], triangle1_points[1], triangle1_points[2]);
                    Triangle triangle2 = KnowledgeGetter.GetTriangle(triangle2_points[0], triangle2_points[1], triangle2_points[2]);
                    if (triangle1 is null || triangle2 is null)
                    {
                        return;
                    }
                    List<Point> seg1_points = RemoveElementByCondition(triangle1_points, x => x == angle11.Vertex);
                    List<Point> seg2_points = RemoveElementByCondition(triangle1_points, x => x == angle21.Vertex);

                    List<Point> seg3_points = RemoveElementByCondition(triangle2_points, x => x == angle12.Vertex);
                    List<Point> seg4_points = RemoveElementByCondition(triangle2_points, x => x == angle22.Vertex);

                    //获取角的对边
                    Segment seg1 = KnowledgeGetter.GetSegment(seg1_points[0], seg1_points[1]);
                    Segment seg2 = KnowledgeGetter.GetSegment(seg2_points[0], seg2_points[1]);
                    Segment seg3 = KnowledgeGetter.GetSegment(seg3_points[0], seg3_points[1]);
                    Segment seg4 = KnowledgeGetter.GetSegment(seg4_points[0], seg4_points[1]);
                    if (seg1 == segmentLengthEqual[0] && seg3 == segmentLengthEqual[1] || seg1 == segmentLengthEqual[1] && seg3 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle21.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle22.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle21.Vertex, triangle1_points[0], angle12.Vertex, angle22.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);

                    }
                    else if (seg1 == segmentLengthEqual[0] && seg4 == segmentLengthEqual[1] || seg1 == segmentLengthEqual[1] && seg4 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle21.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle22.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle21.Vertex, triangle1_points[0], angle12.Vertex, angle22.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);

                    }
                    else if (seg2 == segmentLengthEqual[0] && seg3 == segmentLengthEqual[1] || seg2 == segmentLengthEqual[1] && seg3 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle21.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle22.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle21.Vertex, triangle1_points[0], angle12.Vertex, angle22.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);
                    }
                    else if (seg2 == segmentLengthEqual[0] && seg4 == segmentLengthEqual[1] || seg2 == segmentLengthEqual[1] && seg4 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle21.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle22.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle21.Vertex, triangle1_points[0], angle12.Vertex, angle22.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);
                    }

                }
                else if (result2.exists && result3.exists)
                {
                    List<Point> triangle1_points = result2.intersection;
                    List<Point> triangle2_points = result3.intersection;
                    Triangle triangle1 = KnowledgeGetter.GetTriangle(triangle1_points[0], triangle1_points[1], triangle1_points[2]);
                    Triangle triangle2 = KnowledgeGetter.GetTriangle(triangle2_points[0], triangle2_points[1], triangle2_points[2]);
                    if (triangle1 is null || triangle2 is null)
                    {
                        return;
                    }
                    List<Point> seg1_points = RemoveElementByCondition(triangle1_points, x => x == angle11.Vertex);
                    List<Point> seg2_points = RemoveElementByCondition(triangle1_points, x => x == angle22.Vertex);
                    List<Point> seg3_points = RemoveElementByCondition(triangle2_points, x => x == angle12.Vertex);
                    List<Point> seg4_points = RemoveElementByCondition(triangle2_points, x => x == angle21.Vertex);
                    //获取角的对边
                    Segment seg1 = KnowledgeGetter.GetSegment(seg1_points[0], seg1_points[1]);
                    Segment seg2 = KnowledgeGetter.GetSegment(seg2_points[0], seg2_points[1]);
                    Segment seg3 = KnowledgeGetter.GetSegment(seg3_points[0], seg3_points[1]);
                    Segment seg4 = KnowledgeGetter.GetSegment(seg4_points[0], seg4_points[1]);
                    if (seg1 == segmentLengthEqual[0] && seg3 == segmentLengthEqual[1] || seg1 == segmentLengthEqual[1] && seg3 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle22.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle21.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle22.Vertex, triangle1_points[0], angle12.Vertex, angle21.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);

                    }
                    else if (seg1 == segmentLengthEqual[0] && seg4 == segmentLengthEqual[1] || seg1 == segmentLengthEqual[1] && seg4 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle22.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle21.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle22.Vertex, triangle1_points[0], angle12.Vertex, angle21.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);

                    }
                    else if (seg2 == segmentLengthEqual[0] && seg3 == segmentLengthEqual[1] || seg2 == segmentLengthEqual[1] && seg3 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle22.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle21.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle22.Vertex, triangle1_points[0], angle12.Vertex, angle21.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);
                    }
                    else if (seg2 == segmentLengthEqual[0] && seg4 == segmentLengthEqual[1] || seg2 == segmentLengthEqual[1] && seg4 == segmentLengthEqual[0])
                    {
                        triangle1_points.Remove(angle11.Vertex);
                        triangle1_points.Remove(angle22.Vertex);
                        triangle2_points.Remove(angle12.Vertex);
                        triangle2_points.Remove(angle21.Vertex);
                        TriangleCongruence triangleCongruence = new TriangleCongruence(angle11.Vertex, angle22.Vertex, triangle1_points[0], angle12.Vertex, angle21.Vertex, triangle2_points[0]);
                        triangleCongruence.AddReason();
                        triangleCongruence.AddCondition(angleSizeEqual1, angleSizeEqual2, segmentLengthEqual);
                        AddProcessor.Add(triangleCongruence);
                    }
                }
                else
                {
                    return;
                }
            

        }
    
        public void RuleT004三角形全等HL(RightTriangle rTriangle1, RightTriangle rTriangle2, SegmentLengthEqual equal1, SegmentLengthEqual equal2)
        {
            var seg11 = KnowledgeGetter.GetSegment(rTriangle1[0], rTriangle1[1]);
            var seg12 = KnowledgeGetter.GetSegment(rTriangle1[0], rTriangle1[2]);
            //斜边
            var seg13 = KnowledgeGetter.GetSegment(rTriangle1[1], rTriangle1[2]);

            var seg21 = KnowledgeGetter.GetSegment(rTriangle2[0], rTriangle2[1]);
            var seg22 = KnowledgeGetter.GetSegment(rTriangle2[0], rTriangle2[2]);
            //斜边 
            var seg23 = KnowledgeGetter.GetSegment(rTriangle2[1], rTriangle2[2]);

            if (equal1[0] == seg13 && equal1[1] == seg23 || equal1[1] == seg13 && equal1[0] == seg23)
            {
            }
            else
                return;
            TriangleCongruence pred;
            if (equal2[0] == seg11 && equal2[1] == seg21 || equal2[1] == seg11 && equal2[0] == seg21)
            {
                pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
                    (Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2]);
            }
            else if (equal2[0] == seg12 && equal2[1] == seg21 || equal2[1] == seg12 && equal2[0] == seg21)
            {
                pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
                    (Point)rTriangle1[2], (Point)rTriangle1[1], (Point)rTriangle1[0]);
            }
            else if (equal2[0] == seg11 && equal2[1] == seg22 || equal2[1] == seg11 && equal2[0] == seg22)
            {
                pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
                    (Point)rTriangle1[2], (Point)rTriangle1[1], (Point)rTriangle1[0]);
            }
            else if (equal2[0] == seg12 && equal2[1] == seg22 || equal2[1] == seg12 && equal2[0] == seg22)
            {
                pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
                    (Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2]);
            }
            else
                return;
            var line1 = KnowledgeGetter.GetColine((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var line2 = KnowledgeGetter.GetColine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
                return;
            if (pred.Properties.Take(3).ToList().TrueForAll(p => pred.Properties.Skip(3).Contains(p)))
                return;
            var triangle1 = KnowledgeGetter.GetTriangle((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var triangle2 = KnowledgeGetter.GetTriangle((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (triangle1 is null || triangle2 is null) return;
            pred.AddReason();
            pred.AddCondition(rTriangle1, rTriangle1, equal1, equal2);
            AddProcessor.Add(pred);
        }

        public void RuleT005三角形相似_SSS(SegmentLengthRatio ratio1, SegmentLengthRatio ratio2, SegmentLengthRatio ratio3)
        {
            Segment seg11 = (Segment)ratio1[0];
            Segment seg12 = (Segment)ratio1[1];
            Segment seg21 = null;
            Segment seg22 = null;
            if (ratio2.Expr == ratio1.Expr)
            {
                seg21 = (Segment)ratio2[0];
                seg22 = (Segment)ratio2[1];
            }
            else if (ratio2.Expr == ratio1.Expr.Invert())
            {
                seg21 = (Segment)ratio2[1];
                seg22 = (Segment)ratio2[0];
            }
            else
                return;
            Segment seg31 = null;
            Segment seg32 = null;
            if (ratio3.Expr == ratio1.Expr)
            {
                seg31 = (Segment)ratio3[0];
                seg32 = (Segment)ratio3[1];
            }
            else if (ratio3.Expr == ratio1.Expr.Invert())
            {
                seg31 = (Segment)ratio2[1];
                seg32 = (Segment)ratio2[0];
            }
            else
                return;


            List<(List<Segment>, List<Segment>)> groups = new List<(List<Segment>, List<Segment>)>();
            groups.Add((new() { seg11, seg21, seg31 }, new() { seg12, seg22, seg32 }));
            groups.Add((new() { seg11, seg21, seg32 }, new() { seg12, seg22, seg31 }));
            groups.Add((new() { seg11, seg22, seg31 }, new() { seg12, seg21, seg32 }));
            groups.Add((new() { seg11, seg22, seg32 }, new() { seg12, seg21, seg31 }));

            foreach (var group in groups)
            {
                var a = DictionaryTool.CountItemNum(group.Item1.SelectMany(s => s.Properties));
                var b = DictionaryTool.CountItemNum(group.Item2.SelectMany(s => s.Properties));
                if (a.ToList().TrueForAll(s => s.Value == 2) && b.ToList().TrueForAll(s => s.Value == 2))
                {
                    //找对应
                    var pa1 = a.Keys.First(p => !group.Item1[0].Properties.Contains(p));
                    var pb1 = b.Keys.First(p => !group.Item2[0].Properties.Contains(p));

                    var pa2 = a.Keys.First(p => !group.Item1[1].Properties.Contains(p));
                    var pb2 = b.Keys.First(p => !group.Item2[1].Properties.Contains(p));

                    var pa3 = a.Keys.First(p => !group.Item1[2].Properties.Contains(p));
                    var pb3 = b.Keys.First(p => !group.Item2[2].Properties.Contains(p));
                    //三角形共线
                    var line1 = KnowledgeGetter.GetColine((Point)pa1, (Point)pa2, (Point)pa3);
                    var line2 = KnowledgeGetter.GetColine((Point)pb1, (Point)pb2, (Point)pb3);
                    if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
                        return;
                    var triangle1 = KnowledgeGetter.GetTriangle((Point)pa1, (Point)pa2, (Point)pa3);
                    var triangle2 = KnowledgeGetter.GetTriangle((Point)pb1, (Point)pb2, (Point)pb3);
                    if (triangle1 is null || triangle2 is null) continue;
                    TriangleSimilarity pred =
                    new TriangleSimilarity((Point)pa1, (Point)pa2, (Point)pa3,
                                        (Point)pb1, (Point)pb2, (Point)pb3);
                    if (pred.Properties.Take(3).ToList().TrueForAll(p => pred.Properties.Skip(3).Contains(p)))
                        return;
                    pred.AddReason();
                    pred.AddCondition(ratio1, ratio2, ratio3);
                    AddProcessor.Add(pred);
                }
            }

        }
        public void RuleT006三角形相似SAS(SegmentLengthRatio ratio1, SegmentLengthRatio ratio2, AngleSizeEqual angleEqual)
        {
            Segment seg11 = (Segment)ratio1[0];
            Segment seg12 = (Segment)ratio1[1];
            Segment seg21 = null;
            Segment seg22 = null;
            if (ratio1.Expr == 0) return;
            if (ratio2.Expr == 0) return;
            if (ratio2.Expr == ratio1.Expr)
            {
                seg21 = (Segment)ratio2[0];
                seg22 = (Segment)ratio2[1];
            }
            else if (ratio2.Expr == ratio1.Expr.Invert())
            {
                seg21 = (Segment)ratio2[1];
                seg22 = (Segment)ratio2[0];
            }
            else
                return;

            TriangleCongruence pred = null;
            Angle angle1 = (Angle)angleEqual[0];
            var aseg11 = KnowledgeGetter.GetSegment(angle1[0], angle1[1]);
            var aseg12 = KnowledgeGetter.GetSegment(angle1[2], angle1[1]);

            Angle angle2 = (Angle)angleEqual[1];
            var aseg21 = KnowledgeGetter.GetSegment(angle2[0], angle2[1]);
            var aseg22 = KnowledgeGetter.GetSegment(angle2[2], angle2[1]);

            if (aseg11 == seg11 && aseg12 == seg22 && aseg21 == seg21 && aseg22 == seg11)
            {
                pred = new TriangleCongruence((Point)angle1[0], (Point)angle1[1], (Point)angle1[2],
                    (Point)angle2[2], (Point)angle2[1], (Point)angle2[0]);
            }
            else
                return;
            var line1 = KnowledgeGetter.GetColine((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var line2 = KnowledgeGetter.GetColine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
                return;
            if (pred.Properties.Take(3).ToList().TrueForAll(p => pred.Properties.Skip(3).Contains(p)))
                return;

            var triangle1 = KnowledgeGetter.GetTriangle((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var triangle2 = KnowledgeGetter.GetTriangle((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (triangle1 is null || triangle2 is null) return;

            pred.AddReason();
            pred.AddCondition(ratio1, ratio2, angleEqual);
            AddProcessor.Add(pred);
        }
        public void RuleT007三角形相似AAS(AngleSizeEqual angleEqual1, AngleSizeEqual angleEqual2, SegmentLengthRatio equal)
        {

            Angle angle11 = (Angle)angleEqual1[0];
            Angle angle12 = (Angle)angleEqual1[1];
            Angle angle21 = (Angle)angleEqual2[0];
            Angle angle22 = (Angle)angleEqual2[1];
            List<Point> points1 = new List<Point>();
            List<Point> points2 = new List<Point>();
            //确定存在两个三角形中两个角对应相等
            if (angle11.Properties.TrueForAll(p => angle21.Properties.Contains(p)) &&
                angle12.Properties.TrueForAll(p => angle22.Properties.Contains(p)))
            {
                points1.Add((Point)angle11[1]);
                points1.Add((Point)angle21[1]);
                points1.Add((Point)angle11.Properties.First(p => p != angle11[1] && p != angle21[1]));

                points2.Add((Point)angle12[1]);
                points2.Add((Point)angle22[1]);
                points2.Add((Point)angle12.Properties.First(p => p != angle12[1] && p != angle22[1]));
            }
            else if (angle11.Properties.TrueForAll(p => angle22.Properties.Contains(p)) &&
                angle12.Properties.TrueForAll(p => angle21.Properties.Contains(p)))
            {
                points1.Add((Point)angle11[1]);
                points1.Add((Point)angle22[1]);
                points1.Add((Point)angle11.Properties.First(p => p != angle11[1] && p != angle22[1]));

                points2.Add((Point)angle12[1]);
                points2.Add((Point)angle21[1]);
                points2.Add((Point)angle12.Properties.First(p => p != angle12[1] && p != angle21[1]));
            }
            else
                return;
            var seg11 = KnowledgeGetter.GetSegment(points1[0], points1[2]);
            var seg12 = KnowledgeGetter.GetSegment(points1[1], points1[2]);
            var seg13 = KnowledgeGetter.GetSegment(points1[0], points1[1]);

            var seg21 = KnowledgeGetter.GetSegment(points2[0], points2[2]);
            var seg22 = KnowledgeGetter.GetSegment(points2[1], points2[2]);
            var seg23 = KnowledgeGetter.GetSegment(points2[0], points2[1]);
            if (equal[0] == seg11 && equal[1] == seg21 || equal[1] == seg11 && equal[0] == seg21)
            {

            }
            else if (equal[0] == seg12 && equal[1] == seg22 || equal[1] == seg12 && equal[0] == seg22)
            {

            }
            else if (equal[0] == seg13 && equal[1] == seg23 || equal[1] == seg13 && equal[0] == seg23)
            {

            }
            else
                return;


            TriangleSimilarity pred = new TriangleSimilarity((Point)points1[0], (Point)points1[1], (Point)points1[2],
                    (Point)points2[0], (Point)points2[1], (Point)points2[2]);

            var triangle1 = KnowledgeGetter.GetTriangle((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var triangle2 = KnowledgeGetter.GetTriangle((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (triangle1 is null || triangle2 is null) return;


            var line1 = KnowledgeGetter.GetColine((Point)pred[0], (Point)pred[1], (Point)pred[2]);
            var line2 = KnowledgeGetter.GetColine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
                return;
            if (points1.TrueForAll(p => points2.Contains(p)))
                return;

            pred.AddReason();
            pred.AddCondition(angleEqual1, angleEqual2, equal);
            AddProcessor.Add(pred);
        }
        //public void RuleT008三角形相似HL(RightTriangle rTriangle1, RightTriangle rTriangle2, SegmentLengthRatio ratio1, SegmentLengthRatio ratio2)
        //{
        //    Segment rseg11 = (Segment)ratio1[0];
        //    Segment rseg12 = (Segment)ratio1[1];
        //    Segment rseg21 = null;
        //    Segment rseg22 = null;
        //    if (ratio2.Expr == ratio1.Expr)
        //    {
        //        rseg21 = (Segment)ratio2[0];
        //        rseg22 = (Segment)ratio2[1];
        //    }
        //    else if (ratio2.Expr == ratio1.Expr.Invert())
        //    {
        //        rseg21 = (Segment)ratio2[1];
        //        rseg22 = (Segment)ratio2[0];
        //    }
        //    else
        //        return;

        //    var seg11 = KnowledgeGetter.GetSegment(rTriangle1[0], rTriangle1[1]);
        //    var seg12 = KnowledgeGetter.GetSegment(rTriangle1[0], rTriangle1[2]);
        //    //斜边
        //    var seg13 = KnowledgeGetter.GetSegment(rTriangle1[1], rTriangle1[1]);

        //    var seg21 = KnowledgeGetter.GetSegment(rTriangle2[0], rTriangle2[1]);
        //    var seg22 = KnowledgeGetter.GetSegment(rTriangle2[0], rTriangle2[2]);
        //    //斜边 
        //    var seg23 = KnowledgeGetter.GetSegment(rTriangle2[1], rTriangle2[1]);

        //    if (rseg11 == seg13 && rseg12 == seg23 || rseg12 == seg13 && rseg11 == seg23)
        //    {
        //    }
        //    else
        //        return;
        //    TriangleCongruence pred;
        //    if (ratio2[0] == seg11 && ratio2[1] == seg21 || ratio2[1] == seg11 && ratio2[0] == seg21)
        //    {
        //        pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
        //            (Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2]);
        //    }
        //    else if (ratio2[0] == seg12 && ratio2[1] == seg21 || ratio2[1] == seg12 && ratio2[0] == seg21)
        //    {
        //        pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
        //            (Point)rTriangle1[2], (Point)rTriangle1[1], (Point)rTriangle1[0]);
        //    }
        //    else if (ratio2[0] == seg11 && ratio2[1] == seg22 || ratio2[1] == seg11 && ratio2[0] == seg22)
        //    {
        //        pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
        //            (Point)rTriangle1[2], (Point)rTriangle1[1], (Point)rTriangle1[0]);
        //    }
        //    else if (ratio2[0] == seg12 && ratio2[1] == seg22 || ratio2[1] == seg12 && ratio2[0] == seg22)
        //    {
        //        pred = new TriangleCongruence((Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2],
        //            (Point)rTriangle1[0], (Point)rTriangle1[1], (Point)rTriangle1[2]);
        //    }
        //    else
        //        return;
        //    var line1 = KnowledgeGetter.GetColine((Point)pred[0], (Point)pred[1], (Point)pred[2]);
        //    var line2 = KnowledgeGetter.GetColine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //    if (line1 is not NegatingKnowledge || line2 is not NegatingKnowledge)
        //        return;
        //    if (pred.Properties.Take(3).ToList().TrueForAll(p => pred.Properties.Skip(3).Contains(p)))
        //        return;
        //    var triangle1 = KnowledgeGetter.GetTriangle((Point)pred[0], (Point)pred[1], (Point)pred[2]);
        //    var triangle2 = KnowledgeGetter.GetTriangle((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //    if (triangle1 is null || triangle2 is null) return;

        //    pred.AddReason();
        //    pred.AddCondition(rTriangle1, rTriangle1, ratio1, ratio2);
        //    AddProcessor.Add(pred);
        //}

        public void RuleT009三角形全等性质(TriangleCongruence cong)
        {
            
            var ab = KnowledgeGetter.GetSegment(cong[0], cong[1]);
            var bc = KnowledgeGetter.GetSegment(cong[0], cong[2]);
            var ca = KnowledgeGetter.GetSegment(cong[1], cong[2]);
            var de = KnowledgeGetter.GetSegment(cong[3], cong[4]);
            var ef = KnowledgeGetter.GetSegment(cong[3], cong[5]);
            var df = KnowledgeGetter.GetSegment(cong[4], cong[5]);

            var angleBAC = KnowledgeGetter.GetAngle(cong[1], cong[0], cong[2]);
            var angleABC = KnowledgeGetter.GetAngle(cong[0], cong[1], cong[2]);
            var angleACB = KnowledgeGetter.GetAngle(cong[0], cong[2], cong[1]);
            var angleEDF = KnowledgeGetter.GetAngle(cong[4], cong[3], cong[5]);
            var angleDEF = KnowledgeGetter.GetAngle(cong[3], cong[4], cong[5]);
            var angleDFE = KnowledgeGetter.GetAngle(cong[3], cong[5], cong[4]);

            List<Knowledge> knowledges = new List<Knowledge>();
            knowledges.Add(new SegmentLengthEqual(ab, de));
            knowledges.Add(new SegmentLengthEqual(bc, ef));
            knowledges.Add(new SegmentLengthEqual(ca, df));

            knowledges.Add(new AngleSizeEqual(angleBAC, angleEDF));
            knowledges.Add(new AngleSizeEqual(angleABC, angleDEF));
            knowledges.Add(new AngleSizeEqual(angleACB, angleDFE));
           
            foreach (var pred in knowledges)
            {
                pred.AddReason();
                pred.AddCondition(cong);
                AddProcessor.Add(pred);
            }
        }
        public void RuleT010三角形相似性质(TrisectionArc cong)
        {
            var triangle1 = KnowledgeGetter.GetTriangle((Point)cong[0], (Point)cong[1], (Point)cong[2]);
            var triangle2 = KnowledgeGetter.GetTriangle((Point)cong[3], (Point)cong[4], (Point)cong[5]);
            var ab = KnowledgeGetter.GetSegment(triangle1[0], triangle1[1]);
            var bc = KnowledgeGetter.GetSegment(triangle1[1], triangle1[2]);
            var ca = KnowledgeGetter.GetSegment(triangle1[2], triangle1[0]);
            var de = KnowledgeGetter.GetSegment(triangle2[0], triangle2[1]);
            var ef = KnowledgeGetter.GetSegment(triangle2[1], triangle2[2]);
            var df = KnowledgeGetter.GetSegment(triangle2[2], triangle2[0]);

            var angleBAC = KnowledgeGetter.GetAngle(triangle1[1], triangle1[0], triangle1[2]);
            var angleABC = KnowledgeGetter.GetAngle(triangle1[0], triangle1[1], triangle1[2]);
            var angleACB = KnowledgeGetter.GetAngle(triangle1[0], triangle1[2], triangle1[1]);
            var angleEDF = KnowledgeGetter.GetAngle(triangle2[1], triangle2[0], triangle2[2]);
            var angleDEF = KnowledgeGetter.GetAngle(triangle2[0], triangle2[1], triangle2[2]);
            var angleDFE = KnowledgeGetter.GetAngle(triangle2[0], triangle2[2], triangle2[1]);
            //var nv = Expr.NewVar;
            //List<Knowledge> knowledges = new List<Knowledge>();
            //knowledges.Add(new GeoEquation(ab.Length.Div(de.Length), nv));
            //knowledges.Add(new GeoEquation(bc.Length.Div(ef.Length), nv));
            //knowledges.Add(new GeoEquation(ca.Length.Div(df.Length), nv));

            //knowledges.Add(new GeoEquation(angleBAC.Size, angleEDF.Size));
            //knowledges.Add(new GeoEquation(angleABC.Size, angleDEF.Size));
            //knowledges.Add(new GeoEquation(angleACB.Size, angleDFE.Size));

            //knowledges.Add(new GeoEquation(triangle1.Area.Div(triangle2.Area), nv));
            //knowledges.Add(new GeoEquation(triangle1.Perimeter.Div(triangle2.Perimeter), nv));
            //foreach (var pred in knowledges)
            //{

            //    pred.AddReason();
            //    pred.AddCondition(cong);
            //    AddProcessor.Add(pred);
            //}
        }

        private (bool exists, List<Point> intersection) GetIntersectionWithThreeElements(List<List<Point>> list1, List<List<Point>> list2)
        {
            // 遍历 list1 的每个元素
            foreach (var subList1 in list1)
            {
                // 遍历 list2 的每个元素
                foreach (var subList2 in list2)
                {
                    // 计算交集
                    var intersection = subList1.Intersect(subList2).ToList();

                    // 如果交集的元素个数等于 3，返回 true 和交集的元素
                    if (intersection.Count == 3)
                    {
                        return (true, intersection);
                    }
                }
            }

            // 如果没有找到符合条件的交集，返回 false 和一个空列表
            return (false, new List<Point>());
        }

        private List<T> RemoveElementByCondition<T>(List<T> list, Func<T, bool> condition)
        {
            // 创建一个新列表，复制原始列表的内容
            List<T> newList = new List<T>(list);

            // 找到满足条件的元素
            T elementToRemove = newList.FirstOrDefault(condition);

            // 如果找到满足条件的元素，则去掉它
            if (elementToRemove != null)
            {
                newList.Remove(elementToRemove);
            }

            // 返回新列表
            return newList;
        }
    }


}
