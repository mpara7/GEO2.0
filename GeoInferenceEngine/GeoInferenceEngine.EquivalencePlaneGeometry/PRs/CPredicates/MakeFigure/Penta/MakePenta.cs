using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeFigure.Quad
{
    [Description("作五边形")]
    public class MakePenta : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakePenta(Point point1, Point point2, Point point3, Point point4, Point point5)
        {
            Add(point1, point2, point3, point4, point5);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}作五边形";

        public override void Normalize()
        {
        }
    }

}
