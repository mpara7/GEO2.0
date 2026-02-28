using GeoInferenceEngine.Knowledges;


namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.Strategy;
public class QuadrilateralStrategy : Strategy
{

    List<Knowledge> quadrilateralModel1(Quadriliateral quadriliateral, Midpoint midpoint, Midpoint midpoint1, Midpoint midpoint2, Midpoint midpoint3)
    {
        List<Knowledge> list = new List<Knowledge>();
        HashSet<Segment> segSets = new HashSet<Segment>();


        Segment segment1 = (Segment)midpoint[1];
        Segment segment2 = (Segment)midpoint1[1];
        Segment segment3 = (Segment)midpoint2[1];
        Segment segment4 = (Segment)midpoint3[1];

        segSets.Add(segment1);
        segSets.Add(segment2);
        segSets.Add(segment3);
        segSets.Add(segment4);

        Boolean flag = false;

        Segment segment5 = KnowledgeGetter.GetSegment((Point)quadriliateral[0], (Point)quadriliateral[1]);
        Segment segment6 = KnowledgeGetter.GetSegment((Point)quadriliateral[1], (Point)quadriliateral[2]);
        Segment segment7 = KnowledgeGetter.GetSegment((Point)quadriliateral[2], (Point)quadriliateral[3]);
        Segment segment8 = KnowledgeGetter.GetSegment((Point)quadriliateral[0], (Point)quadriliateral[3]);

        if (!segSets.Contains(segment5))
        {
            flag = true;
        }
        if (!segSets.Contains(segment6))
        {
            flag = true;
        }
        if (!segSets.Contains(segment7))
        {
            flag = true;
        }
        if (!segSets.Contains(segment8))
        {
            flag = true;
        }
        if (flag)
        {
            return list;
        }
        Point point1 = (Point)midpoint[0];
        Point point2 = (Point)midpoint1[0];
        Point point3 = (Point)midpoint2[0];
        Point point4 = (Point)midpoint3[0];
        list.Add(new Parallelogram(point1, point2, point3, point4));
        list.Add(new Segment(point1, point2));
        list.Add(new Segment(point2, point3));
        list.Add(new Segment(point3, point4));
        list.Add(new Segment(point1, point4));
        return list;
    }

    List<Knowledge> quadrilateralModel2(Rectangle rect, Midpoint midpoint, Midpoint midpoint1, Midpoint midpoint2, Midpoint midpoint3)
    {
        List<Knowledge> list = new List<Knowledge>();
        HashSet<Segment> segSets = new HashSet<Segment>();


        Segment segment1 = (Segment)midpoint[1];
        Segment segment2 = (Segment)midpoint1[1];
        Segment segment3 = (Segment)midpoint2[1];
        Segment segment4 = (Segment)midpoint3[1];

        segSets.Add(segment1);
        segSets.Add(segment2);
        segSets.Add(segment3);
        segSets.Add(segment4);

        Boolean flag = false;

        Segment segment5 = KnowledgeGetter.GetSegment((Point)rect[0], (Point)rect[1]);
        Segment segment6 = KnowledgeGetter.GetSegment((Point)rect[1], (Point)rect[2]);
        Segment segment7 = KnowledgeGetter.GetSegment((Point)rect[2], (Point)rect[3]);
        Segment segment8 = KnowledgeGetter.GetSegment((Point)rect[0], (Point)rect[3]);

        if (!segSets.Contains(segment5))
        {
            flag = true;
        }
        if (!segSets.Contains(segment6))
        {
            flag = true;
        }
        if (!segSets.Contains(segment7))
        {
            flag = true;
        }
        if (!segSets.Contains(segment8))
        {
            flag = true;
        }
        if (flag)
        {
            return list;
        }
        Point point1 = (Point)midpoint[0];
        Point point2 = (Point)midpoint1[0];
        Point point3 = (Point)midpoint2[0];
        Point point4 = (Point)midpoint3[0];
        list.Add(new Rhombus(point1, point2, point3, point4));
        list.Add(new Segment(point1, point2));
        list.Add(new Segment(point2, point3));
        list.Add(new Segment(point3, point4));
        list.Add(new Segment(point1, point4));
        return list;
    }

    List<Knowledge> quadrilateralModel3(Rhombus rhombus, Midpoint midpoint, Midpoint midpoint1, Midpoint midpoint2, Midpoint midpoint3)
    {
        List<Knowledge> list = new List<Knowledge>();
        HashSet<Segment> segSets = new HashSet<Segment>();


        Segment segment1 = (Segment)midpoint[1];
        Segment segment2 = (Segment)midpoint1[1];
        Segment segment3 = (Segment)midpoint2[1];
        Segment segment4 = (Segment)midpoint3[1];

        segSets.Add(segment1);
        segSets.Add(segment2);
        segSets.Add(segment3);
        segSets.Add(segment4);

        Boolean flag = false;

        Segment segment5 = KnowledgeGetter.GetSegment((Point)rhombus[0], (Point)rhombus[1]);
        Segment segment6 = KnowledgeGetter.GetSegment((Point)rhombus[1], (Point)rhombus[2]);
        Segment segment7 = KnowledgeGetter.GetSegment((Point)rhombus[2], (Point)rhombus[3]);
        Segment segment8 = KnowledgeGetter.GetSegment((Point)rhombus[0], (Point)rhombus[3]);

        if (!segSets.Contains(segment5))
        {
            flag = true;
        }
        if (!segSets.Contains(segment6))
        {
            flag = true;
        }
        if (!segSets.Contains(segment7))
        {
            flag = true;
        }
        if (!segSets.Contains(segment8))
        {
            flag = true;
        }
        if (flag)
        {
            return list;
        }
        Point point1 = (Point)midpoint[0];
        Point point2 = (Point)midpoint1[0];
        Point point3 = (Point)midpoint2[0];
        Point point4 = (Point)midpoint3[0];
        list.Add(new Rectangle(point1, point2, point3, point4));
        list.Add(new Segment(point1, point2));
        list.Add(new Segment(point2, point3));
        list.Add(new Segment(point3, point4));
        list.Add(new Segment(point1, point4));
        return list;
    }

    List<Knowledge> quadrilateralModel4(Square sq, Midpoint midpoint, Midpoint midpoint1, Midpoint midpoint2, Midpoint midpoint3)
    {
        List<Knowledge> list = new List<Knowledge>();
        HashSet<Segment> segSets = new HashSet<Segment>();


        Segment segment1 = (Segment)midpoint[1];
        Segment segment2 = (Segment)midpoint1[1];
        Segment segment3 = (Segment)midpoint2[1];
        Segment segment4 = (Segment)midpoint3[1];

        segSets.Add(segment1);
        segSets.Add(segment2);
        segSets.Add(segment3);
        segSets.Add(segment4);

        Boolean flag = false;

        Segment segment5 = KnowledgeGetter.GetSegment((Point)sq[0], (Point)sq[1]);
        Segment segment6 = KnowledgeGetter.GetSegment((Point)sq[1], (Point)sq[2]);
        Segment segment7 = KnowledgeGetter.GetSegment((Point)sq[2], (Point)sq[3]);
        Segment segment8 = KnowledgeGetter.GetSegment((Point)sq[0], (Point)sq[3]);

        if (!segSets.Contains(segment5))
        {
            flag = true;
        }
        if (!segSets.Contains(segment6))
        {
            flag = true;
        }
        if (!segSets.Contains(segment7))
        {
            flag = true;
        }
        if (!segSets.Contains(segment8))
        {
            flag = true;
        }
        if (flag)
        {
            return list;
        }
        Point point1 = (Point)midpoint[0];
        Point point2 = (Point)midpoint1[0];
        Point point3 = (Point)midpoint2[0];
        Point point4 = (Point)midpoint3[0];
        list.Add(new Square(point1, point2, point3, point4));
        list.Add(new Segment(point1, point2));
        list.Add(new Segment(point2, point3));
        list.Add(new Segment(point3, point4));
        list.Add(new Segment(point1, point4));
        return list;
    }

    List<Knowledge> quadrilateralModel5()
    {
        List<Knowledge> list = new List<Knowledge>();

        return list;
    }




}