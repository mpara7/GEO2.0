
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.Strategy;
public class MidPointStrategy : Strategy
{
    //构建三角形的中位线
    List<Knowledge> midPointStrategy(Triangle triangle,Midpoint midpoint)
    {
        List<Knowledge> list = new List<Knowledge>();
        Segment segment1 = KnowledgeGetter.GetSegment((Point)triangle[0], (Point)triangle[1]);
        Segment segment2 = KnowledgeGetter.GetSegment((Point)triangle[0], (Point)triangle[2]);
        Segment segment3 = KnowledgeGetter.GetSegment((Point)triangle[1], (Point)triangle[2]);

        Point point = (Point)midpoint[0];
        Segment segment =GetSegment(midpoint[1], midpoint[2]);
        if(segment1 == segment)
        {
            Midpoint midpoint1 = KnowledgeGetter.GetMidPoint((Point)triangle[0], (Point)triangle[2]);
            if (midpoint1 is null)
            {
                Point point1 = new Point(GenerateRandomString());
                midpoint1 = new Midpoint(point1, (Point)triangle[0], (Point)triangle[2]);
                Line line = new Line(point1, (Point)triangle[0], (Point)triangle[2]);
                Segment midsSegment = new Segment(point, point1);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment3, triangle);
                list.Add(point1);
                list.Add(line);
                list.Add(midpoint1);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            else
            {
                Segment midsSegment = new Segment(point, (Point)midpoint1[0]);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment3, triangle);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            Midpoint midpoint2 = KnowledgeGetter.GetMidPoint((Point)triangle[1], (Point)triangle[2]);
            if (midpoint2 is null)
            {
                Point point2 = new Point(GenerateRandomString());
                midpoint2 = new Midpoint(point2, (Point)triangle[1], (Point)triangle[2]);
                Line line = new Line(point2, (Point)triangle[1], (Point)triangle[2]);
                Segment midsSegment = new Segment(point, point2);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment2, triangle);
                list.Add(point2);
                list.Add(line);
                list.Add(midpoint2);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            else
            {
                Segment midsSegment = new Segment(point, (Point)midpoint2[0]);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment2, triangle);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
        }
        else if(segment2 == segment)
        {
            Midpoint midpoint1 = KnowledgeGetter.GetMidPoint((Point)triangle[0], (Point)triangle[1]);
            if (midpoint1 is null)
            {
                Point point1 = new Point(GenerateRandomString());
                midpoint1 = new Midpoint(point1, (Point)triangle[0], (Point)triangle[1]);
                Line line = new Line(point1, (Point)triangle[0], (Point)triangle[1]);
                Segment midsSegment = new Segment(point, point1);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment3, triangle);
                list.Add(point1);
                list.Add(line);
                list.Add(midpoint1);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            else
            {
                Segment midsSegment = new Segment(point, (Point)midpoint1[0]);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment3, triangle);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            Midpoint midpoint2 = KnowledgeGetter.GetMidPoint((Point)triangle[1], (Point)triangle[2]);
            if (midpoint2 is null)
            {
                Point point2 = new Point(GenerateRandomString());
                midpoint2 = new Midpoint(point2, (Point)triangle[1], (Point)triangle[2]);
                Line line = new Line(point2, (Point)triangle[1], (Point)triangle[2]);
                Segment midsSegment = new Segment(point, point2);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment1, triangle);
                list.Add(point2);
                list.Add(line);
                list.Add(midpoint2);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            else
            {
                Segment midsSegment = new Segment(point, (Point)midpoint2[0]);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment1, triangle);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }

        }
        else if(segment3 == segment)
        {
            Midpoint midpoint1 = KnowledgeGetter.GetMidPoint((Point)triangle[0], (Point)triangle[1]);
            if (midpoint1 is null)
            {
                Point point1 = new Point(GenerateRandomString());
                midpoint1 = new Midpoint(point1, (Point)triangle[0], (Point)triangle[1]);
                Line line = new Line(point1, (Point)triangle[0], (Point)triangle[1]);
                Segment midsSegment = new Segment(point, point1);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment2, triangle);
                list.Add(point1);
                list.Add(line);
                list.Add(midpoint1);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            else
            {
                Segment midsSegment = new Segment(point, (Point)midpoint1[0]);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment2, triangle);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            Midpoint midpoint2 = KnowledgeGetter.GetMidPoint((Point)triangle[0], (Point)triangle[2]);
            if (midpoint2 is null)
            {
                Point point2 = new Point(GenerateRandomString());
                midpoint2 = new Midpoint(point2, (Point)triangle[0], (Point)triangle[2]);
                Line line = new Line(point2, (Point)triangle[0], (Point)triangle[2]);
                Segment midsSegment = new Segment(point, point2);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment1, triangle);
                list.Add(point2);
                list.Add(line);
                list.Add(midpoint2);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }
            else
            {
                Segment midsSegment = new Segment(point, (Point)midpoint2[0]);
                MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midsSegment, segment1, triangle);
                list.Add(midsSegment);
                list.Add(midsegmentOfTriangle);
            }

        }
        return list;
    }

    //倍长三角形的一边，构造三角形中位线
    List<Knowledge> midPointStrategy1(Triangle triangle, Midpoint midpoint)
    {
        List<Knowledge> list = new List<Knowledge>();
        Segment segment1 = KnowledgeGetter.GetSegment((Point)triangle[0], (Point)triangle[1]);
        Segment segment2 = KnowledgeGetter.GetSegment((Point)triangle[0], (Point)triangle[2]);
        Segment segment3 = KnowledgeGetter.GetSegment((Point)triangle[1], (Point)triangle[2]);
        Point point = (Point)midpoint[0];
        Segment segment = GetSegment(midpoint[1], midpoint[2]);
        if (segment1 == segment)
        {
            Point point1 = new Point(GenerateRandomString());
            var common = this.FindCommon(segment1, segment2);
            Segment extensionSegment = new Segment(point1,(Point)common.notcommon2);
            Segment midSegment = new Segment(point,(Point)common.notcommon2);
            Segment hemSeg = new Segment((Point)common.notcommon1,point1);
            Line collinear = new Line((Point)segment2[0], (Point)segment2[1],point1);
            Midpoint midpoint1 = new Midpoint((Point)common.notcommon2 ,(Point)common.common, point1);
            Triangle triangle1 = new Triangle((Point)segment1[0], (Point)segment1[1],point1);
            MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midSegment,hemSeg,triangle1);
            list.Add(point1);
            list.Add(extensionSegment);
            list.Add(midSegment);
            list.Add(hemSeg);
            list.Add(collinear);
            list.Add(midpoint1);
            list.Add(triangle1);
            list.Add(midsegmentOfTriangle);
        }
        else if (segment2 == segment)
        {
            Point point1 = new Point(GenerateRandomString());
            var common = this.FindCommon(segment1, segment2);
            Segment extensionSegment = new Segment(point1, (Point)common.notcommon1);
            Segment midSegment = new Segment(point, (Point)common.notcommon1);
            Segment hemSeg = new Segment((Point)common.notcommon2, point1);
            Line collinear = new Line((Point)segment1[0], (Point)segment1[1], point1);
            Midpoint midpoint1 = new Midpoint((Point)common.notcommon1, (Point)common.common, point1);
            Triangle triangle1 = new Triangle((Point)segment2[0], (Point)segment2[1], point1);
            MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midSegment, hemSeg, triangle1);
            list.Add(point1);
            list.Add(extensionSegment);
            list.Add(midSegment);
            list.Add(hemSeg);
            list.Add(collinear);
            list.Add(midpoint1);
            list.Add(triangle1);
            list.Add(midsegmentOfTriangle);

        }
        else if(segment3 == segment)
        {
            Point point1 = new Point(GenerateRandomString());
            var common = this.FindCommon(segment1, segment3);
            Segment extensionSegment = new Segment(point1, (Point)common.notcommon1);
            Segment midSegment = new Segment(point, (Point)common.notcommon1);
            Segment hemSeg = new Segment((Point)common.notcommon2, point1);
            Line collinear = new Line((Point)segment1[0], (Point)segment1[1], point1);
            Midpoint midpoint1 = new Midpoint((Point)common.notcommon1, (Point)common.common, point1);
            Triangle triangle1 = new Triangle((Point)segment3[0], (Point)segment[1], point1);
            MidsegmentOfTriangle midsegmentOfTriangle = new MidsegmentOfTriangle(midSegment, hemSeg, triangle1);
            list.Add(point1);
            list.Add(extensionSegment);
            list.Add(midSegment);
            list.Add(hemSeg);
            list.Add(collinear);
            list.Add(midpoint1);
            list.Add(triangle1);
            list.Add(midsegmentOfTriangle);
        }
        return list;
    }
}
