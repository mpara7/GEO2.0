using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.Strategy;
public class AngleBisectorStrategy : Strategy
{
    [ZDI]
    KnowledgeBase KnowledgeBase { get; set; }
    List<Knowledge> bisectorModel1(PointOnAngularBisector angularBisector, SegmentLengthEqual segmentLengthEqual)
    {

        List<Knowledge> list = new List<Knowledge>();
        //获取角
        Angle angle = (Angle)angularBisector[0];
        List<Point> points = angularBisector.PointOnBisector;
        //获取角的两边上的点
        List<Point> edge1 = angle.Edge1;
        List<Point> edge2 = angle.Edge2;

        Segment seg1 = (Segment)segmentLengthEqual[0];
        Segment seg2 = (Segment)segmentLengthEqual[1];

        var common = this.FindCommon(seg1, seg2);
        //如果两条相等的线段相交点是角的顶点，并且角的两边的点分别包含了seg1和seg2另外一端的端点
        if ((common.common == angle[1] && edge1.Contains((Point)common.notcommon1) && edge2.Contains((Point)common.notcommon2)) || (common.common == angle[1] && edge1.Contains((Point)common.notcommon2) && edge2.Contains((Point)common.notcommon1)))
        {
            Segment segment = new Segment((Point)common.notcommon1, points[points.Count - 1]);
            Segment segment1 = new Segment((Point)common.notcommon2, points[points.Count - 1]);
            TriangleCongruence triangleCongruence = new TriangleCongruence((Point)angle[1], (Point)common.notcommon1, points[points.Count - 1], (Point)angle[1], (Point)common.notcommon2, points[points.Count - 1]);
            list.Add(segment);
            list.Add(segment1);
            list.Add(triangleCongruence);
            return list;
        }

        return list;
    }


    //已知角的一边有垂直关系，作另外一边的垂直
   public  List<Knowledge> bisectorModel2(PointOnAngularBisector angularBisector, Pedal pedal)
    {

        List<Knowledge> list = new List<Knowledge>();
        Line segment1 = (Line)pedal[1];
        Line segment2 = (Line)pedal[2];
        Angle angle = (Angle)angularBisector[0];
        List<Point> edge1 = new List<Point>(angle.Edge1);
        edge1.Insert(0, (Point)angle[1]);
        List<Point> edge2 = new List<Point>(angle.Edge2);
        edge2.Insert(0, (Point)angle[1]);
        //获取角的两边的共线关系
        Line segment3 = KnowledgeGetter.GetLine(edge1.ToArray());
        Line segment4 = KnowledgeGetter.GetLine(edge2.ToArray());

        //角平分线上的点
        List<Point> points = angularBisector.PointOnBisector;

        if (segment1 == segment3)
        {
            Point matchedItem = points.FirstOrDefault(item => segment2.Points.Contains(item));
            if (matchedItem is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSeg = new Line(matchedItem, point);
                //用伪坐标去做
                segment4 = new Line((Point)segment4[0], point, (Point)segment4[1]);
                Pedal newPed = new Pedal(point, newSeg, segment4);
                list.Add(point);
                list.Add(segment4);
                list.Add(newSeg);
                list.Add(newPed);
            }

        }
        else if (segment1 == segment4)
        {
            Point matchedItem = points.FirstOrDefault(item => segment2.Points.Contains(item));
            if (matchedItem is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSeg = new Line(matchedItem,point);
                //用伪坐标去做
                segment3 = new Line((Point)segment3[0], point, (Point)segment3[1]);
                Pedal newPed = new Pedal(point, newSeg, segment3);
                list.Add(point);
                list.Add(segment3);
                list.Add(newSeg);
                list.Add(newPed);
            }
        }
        else if (segment2 == segment3)
        {
            Point matchedItem = points.FirstOrDefault(item => segment1.Points.Contains(item));
            if (matchedItem is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSeg = new Line(matchedItem, point);
                //用伪坐标去做
                segment4 = new Line((Point)segment4[0], point,(Point)segment4[1]);
                Pedal newPed = new Pedal(point, newSeg, segment4);
                list.Add(point);
                list.Add(segment4);
                list.Add(newSeg);
                list.Add(newPed);
               
            }
        }
        else if (segment2 == segment4)
        {
            Point matchedItem = points.FirstOrDefault(item => segment1.Points.Contains(item));
            if (matchedItem is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSeg = new Line(matchedItem, point);
                //用伪坐标去做
                segment3 = new Line((Point)segment3[0], point, (Point)segment3[1]);
                Pedal newPed = new Pedal(point, newSeg, segment3);
                list.Add(point);
                list.Add(segment3);
                list.Add(newSeg);
                list.Add(newPed);
               
            }
        }
        return list;
    }

    //角平分线加垂直,构造等腰三角形
    public List<Knowledge> bisectorModel3(PointOnAngularBisector angularBisector, Pedal pedal)
    {
        List<Knowledge> list = new List<Knowledge>();

        Line segment1 = (Line)pedal[1];
        Line segment2 = (Line)pedal[2];
        List<Point> points1 = segment1.Points;
        List<Point> points2 = segment2.Points;
        Angle angle = (Angle)angularBisector[0];
        List<Point> edge1 = angle.Edge1;
        List<Point> edge2 = angle.Edge2;
        //角平分线上的点
        List<Point> points = angularBisector.PointOnBisector;

        bool isSubset1 = points1.All(item => points.Contains(item));
        bool isSubset2 = points2.All(item => points.Contains(item));
        //seg1是角平分线上的线段
        if (isSubset1)
        {

            Point matchedItem1 = points2.FirstOrDefault(item => edge1.Contains(item));
            Point matchedItem2 = points2.FirstOrDefault(item => edge2.Contains(item));
            if (matchedItem1 is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSegment = new Line(point, (Point)pedal[0]);
                Pedal pedal1 = new Pedal((Point)pedal[0], newSegment, segment1);
                PointWithInPoints pointOnSegment = new PointWithInPoints(point, (Point)angle[1], edge2[edge2.Count - 1]);
                IsoscelesTriangle isoscelesTriangle = new IsoscelesTriangle((Point)angle[1], point,matchedItem1);
                list.Add(point);
                list.Add(newSegment);
                list.Add(pedal1);
                list.Add(pointOnSegment);
                list.Add(isoscelesTriangle);
             
            }
            else if (matchedItem2 is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSegment = new Line(point, (Point)pedal[0]);
                Pedal pedal1 = new Pedal((Point)pedal[0], newSegment, segment1);
                PointWithInPoints pointOnSegment = new PointWithInPoints(point, (Point)angle[1], edge1[edge1.Count - 1]);
                IsoscelesTriangle isoscelesTriangle = new IsoscelesTriangle((Point)angle[1], point, matchedItem2);
                list.Add(point);
                list.Add(newSegment);
                list.Add(pedal1);
                list.Add(pointOnSegment);
                list.Add(isoscelesTriangle);
            }

        }
        //seg2是角平分线上的线段
        else if (isSubset2)
        {
            Point matchedItem1 = points1.FirstOrDefault(item => edge1.Contains(item));
            Point matchedItem2 = points1.FirstOrDefault(item => edge2.Contains(item));
            if (matchedItem1 is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSegment = new Line(point, (Point)pedal[0]);
                Pedal pedal1 = new Pedal((Point)pedal[0], newSegment, segment1);
                PointWithInPoints pointOnSegment = new PointWithInPoints(point, (Point)angle[1], edge2[edge2.Count - 1]);
                IsoscelesTriangle isoscelesTriangle = new IsoscelesTriangle((Point)angle[1],point,matchedItem1);
                list.Add(point);
                list.Add(newSegment);
                list.Add(pedal1);
                list.Add(pointOnSegment);
                list.Add(isoscelesTriangle);
            }
            else if (matchedItem2 is not null)
            {
                Point point = new Point(GenerateRandomString());
                Line newSegment = new(point, (Point)pedal[0]);
                Pedal pedal1 = new Pedal((Point)pedal[0], newSegment, segment1);
                PointWithInPoints pointOnSegment = new PointWithInPoints(point, (Point)angle[1], edge1[edge1.Count - 1]);
                IsoscelesTriangle isoscelesTriangle = new IsoscelesTriangle((Point)angle[1], point, matchedItem2);
                list.Add(point);
                list.Add(newSegment);
                list.Add(pedal1);
                list.Add(pointOnSegment);
                list.Add(isoscelesTriangle);

            }

        }
        return list;
    }


    //遇见角平分线尝试做平行线,作角的两边的平行线

    public List<Knowledge> bisectorModel4(PointOnAngularBisector angularBisector)
    { 
        List<Knowledge> list = new List<Knowledge>();
        Angle angle = (Angle)angularBisector[0];
        List<Point> points = angularBisector.PointOnBisector;
        List<Point> edge1 = angle.Edge1;
        List<Point> edge2 = angle.Edge2;
        Point point = new Point(GenerateRandomString());
        Point point1 = new Point(GenerateRandomString());
        Line angleEdge1 = KnowledgeGetter.GetLine((Point)angle[1], edge1[edge1.Count - 1]);
        Line angleEdge2 = KnowledgeGetter.GetLine((Point)angle[1], edge2[edge2.Count - 1]);
        PointWithInPoints pointOnSegment = new PointWithInPoints(point, (Point)angle[1], edge1[edge1.Count - 1]);
        Line new_collinear = new Line((Point)angle[1], point,edge1[edge1.Count - 1]);
        PointWithInPoints pointOnSegment1 = new PointWithInPoints(point1, (Point)angle[1], edge2[edge2.Count - 1]);
        Line new_collinear1 = new Line((Point)angle[1], point1, edge2[edge2.Count - 1]);
        Line line_parallel = new (points[points.Count - 1], point);
        Line line1_parallel = new (points[points.Count - 1], point1);
        LineParallel segmentParallel = new LineParallel(angleEdge2, line_parallel);
        LineParallel segmentParallel1 = new LineParallel(angleEdge1, line1_parallel);
        list.Add(point);
        list.Add(point1);
        list.Add(pointOnSegment);
        list.Add(pointOnSegment1);
        list.Add(new_collinear);
        list.Add(new_collinear1);
        list.Add(line_parallel);
        list.Add(line1_parallel);
        list.Add(segmentParallel);
        list.Add(segmentParallel1);
        return list;
    }



    //遇见角平分线尝试做平行线,作角平分线的平行线
    public List<Knowledge> bisectorModel5(PointOnAngularBisector angularBisector)
    {
        List<Knowledge> list = new List<Knowledge>();
        Angle angle = (Angle)angularBisector[0];
        List<Point> points = angularBisector.PointOnBisector;
        List<Point> edge1 = angle.Edge1;
        List<Point> edge2 = angle.Edge2;
        Line angleEdge1 = KnowledgeGetter.GetLine((Point)angle[1], edge1[edge1.Count - 1]);
        Line angleEdge2 = KnowledgeGetter.GetLine((Point)angle[1], edge2[edge2.Count - 1]);
        Point point = new Point(GenerateRandomString());
        Point point1 = new Point(GenerateRandomString());
        PointWithInPoints pointOnSegment = new PointWithInPoints(point1, (Point)angle[1], edge1[edge1.Count - 1]);
        Line collinear = new Line(point, (Point)angleEdge2[0], (Point)angleEdge2[1]);
        Line seg = new Line(point, point1);
        LineParallel segmentParallel = new LineParallel(seg, KnowledgeGetter.GetLine((Point)angle[1], points[points.Count - 1]));
        list.Add(point);
        list.Add(point1);
        list.Add(collinear);
        list.Add(seg);
        list.Add(segmentParallel);
        list.Add(pointOnSegment);
        return list;
    }
}

