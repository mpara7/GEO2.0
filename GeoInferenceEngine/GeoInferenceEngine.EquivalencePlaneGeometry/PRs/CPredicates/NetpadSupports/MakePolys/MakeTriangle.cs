using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeFigure.Quad
{
    [Description("作三角形")]
    public class MakeTriangle : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeTriangle(Point point1, Point point2, Point point3, Triangle triangle)
        {
            Add(point1, point2, point3, triangle);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}{Properties[1]}{Properties[2]}作三角形";

        public override void Normalize()
        {
        }
    }

}
