
namespace GeoInferenceEngine.PlaneKnowledges.Knowledges
{
    [Description("点在同侧")]
    public class PointsOnLineSameSide : PlainRelation
    {
        public PointsOnLineSameSide(Point point1, Point point2, Point point3, Point point4)
        {
            Add(point1, point2, point3, point4);
            Normalize();
            SetHashCode();
        }

        public override string ToString() => $"{Properties[2]}、{Properties[3]}在{Properties[0]}{Properties[1]}同侧";

        public override void Normalize()
        {

        }
    }
}
