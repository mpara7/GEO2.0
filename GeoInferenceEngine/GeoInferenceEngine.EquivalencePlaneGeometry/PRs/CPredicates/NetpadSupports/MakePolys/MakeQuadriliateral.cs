using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeFigure.Quad
{
    [Description("作四边形")]
    public class MakeQuadriliateral : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeQuadriliateral(Point point1, Point point2, Point point3, Point point4, Quadriliateral quadriliateral)
        {
            Add(point1, point2, point3, point4, quadriliateral);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}作四边形";

        public override void Normalize()
        {
        }
    }

}
