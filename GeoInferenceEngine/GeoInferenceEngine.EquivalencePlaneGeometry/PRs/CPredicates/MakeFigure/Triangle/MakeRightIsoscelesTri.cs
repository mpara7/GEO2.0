using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeFigure.Quad
{

    [Description("作等腰直角三角形", "作直角等腰三角形")]
    public class MakeRightIsoscelesTri : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeRightIsoscelesTri(Point point1, Point point2, Point point3)
        {
            Add(point1, point2, point3);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}{Properties[1]}{Properties[2]}作直角等腰三角形";

        public override void Normalize()
        {
        }
    }

}
