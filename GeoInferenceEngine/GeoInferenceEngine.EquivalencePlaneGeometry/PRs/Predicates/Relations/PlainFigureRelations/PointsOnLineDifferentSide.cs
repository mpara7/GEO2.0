
namespace GeoInferenceEngine.PlaneKnowledges.Knowledges
{
    [Description("点在异侧")]
    public class PointsOnLineDifferentSide : PlainRelation
    {
        public PointsOnLineDifferentSide(Point point1, Point point2, Point point3, Point point4)
        {
            Add(point1, point2, point3, point4);
            Normalize();
            SetHashCode();
        }

        public override string ToString() => $"{Properties[2]}、{Properties[3]}在{Properties[0]}{Properties[1]}不同侧";

        public override void Normalize()
        {

        }
    }
}
