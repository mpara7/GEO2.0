namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.Strategy;
public class TriangleStrategy : Strategy
{

    //直角三角形，斜边上有中点，连接直角顶点和该中点
    List<Knowledge> TriangleModel1(RightTriangle rightTriangle,Midpoint midpoint)
    {
        List<Knowledge> list = new List<Knowledge>();
        Segment seg = KnowledgeGetter.GetSegment((Point)midpoint[1], (Point)midpoint[2]);
        //获取斜边
        Segment seg1 = KnowledgeGetter.GetSegment((Point)rightTriangle[1], (Point)rightTriangle[2]);
        if(seg1 != seg)
        {
            return list;
        }
        Segment segment = new Segment((Point)midpoint[0], (Point)rightTriangle[0]);
        Triangle triangle = new Triangle((Point)rightTriangle[0], (Point)midpoint[0], (Point)rightTriangle[1]);
        Triangle triangle1 = new Triangle((Point)rightTriangle[0], (Point)midpoint[0], (Point)rightTriangle[2]);
        list.Add(segment);
        list.Add(triangle);
        list.Add(triangle1);
        return list;
    }

    //等腰直角三角形，直角顶点和中点连接
    List<Knowledge> TriangleModel2(IsoscelesRightTriangle rightTriangle, Midpoint midpoint)
    {
        List<Knowledge> list = new List<Knowledge>();
        Segment seg = KnowledgeGetter.GetSegment((Point)midpoint[1], (Point)midpoint[2]);
        //获取斜边
        Segment seg1 = KnowledgeGetter.GetSegment((Point)rightTriangle[1], (Point)rightTriangle[2]);
        if (seg1 != seg)
        {
            return list;
        }
        Segment segment = new Segment((Point)midpoint[0], (Point)rightTriangle[0]);
        list.Add(segment);
        return list;
    }

    //倍长三角形的中线,构造三角形全等
    List<Knowledge> TriangleModel3(Triangle triangle, Midpoint midpoint)
    {

        List<Knowledge> list = new List<Knowledge>();
        Segment seg = KnowledgeGetter.GetSegment((Point)midpoint[1], (Point)midpoint[2]);
        Segment seg1 = KnowledgeGetter.GetSegment((Point)triangle[0], (Point)triangle[1]);
        Segment seg2 = KnowledgeGetter.GetSegment((Point)triangle[0], (Point)triangle[2]);
        Segment seg3 = KnowledgeGetter.GetSegment((Point)triangle[1], (Point)triangle[2]);
        if (seg == seg1)
        {
            Segment segment = new Segment((Point)triangle[2], (Point)midpoint[0]);
            TriangleMedian triangleMedian = new TriangleMedian(segment,triangle);
            Point point = new Point(GenerateRandomString());
            //延长中线
            Segment segment1 = new Segment(point, (Point)midpoint[0]);
            //构造三角形的线段
            Segment segment2 = new Segment(point, (Point)midpoint[1]);
            Triangle triangle1 = new Triangle(point, (Point)midpoint[0],(Point) midpoint[1]);
            TriangleCongruence triangleCongruence = new TriangleCongruence(point, (Point)midpoint[0], (Point)midpoint[1], (Point)triangle[2], (Point)midpoint[0], (Point)midpoint[2]);
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment, segment1);
            list.Add(point);
            list.Add(segment1);
            list.Add(segment2);
            list.Add(triangle1);
            list.Add(segmentLengthEqual);
            list.Add(triangleCongruence);
        }
        else if (seg == seg2)
        {
            Segment segment = new Segment((Point)triangle[1], (Point)midpoint[0]);
            TriangleMedian triangleMedian = new TriangleMedian(segment, triangle);
            Point point = new Point(GenerateRandomString());
            //延长中线
            Segment segment1 = new Segment(point, (Point)midpoint[0]);
            //构造三角形的线段
            Segment segment2 = new Segment(point,(Point) midpoint[1]);
            Triangle triangle1 = new Triangle(point, (Point)midpoint[0], (Point)midpoint[1]);
            TriangleCongruence triangleCongruence = new TriangleCongruence(point, (Point)midpoint[0], (Point)midpoint[1], (Point)triangle[1], (Point)midpoint[0], (Point)midpoint[2]);
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment, segment1);
            list.Add(point);
            list.Add(segment1);
            list.Add(segment2);
            list.Add(triangle1);
            list.Add(segmentLengthEqual);
            list.Add(triangleCongruence);
        }
        else if (seg == seg3) {
            Segment segment = new Segment((Point)triangle[0], (Point)midpoint[0]);
            TriangleMedian triangleMedian = new TriangleMedian(segment, triangle);
            Point point = new Point(GenerateRandomString());
            //延长中线
            Segment segment1 = new Segment(point, (Point)midpoint[0]);
            Segment segment2 = new Segment(point, (Point)midpoint[1]);
            Triangle triangle1 = new Triangle(point, (Point)midpoint[0], (Point)midpoint[1]);
            TriangleCongruence triangleCongruence = new TriangleCongruence(point, (Point)midpoint[0], (Point)midpoint[1], (Point)triangle[0], (Point)midpoint[0], (Point)midpoint[2]);
            SegmentLengthEqual segmentLengthEqual = new SegmentLengthEqual(segment, segment1);
            list.Add(point);
            list.Add(segment1);
            list.Add(segment2);
            list.Add(triangle1);
            list.Add(segmentLengthEqual);
            list.Add(triangleCongruence);
        }

        return list;
    }

   


}
