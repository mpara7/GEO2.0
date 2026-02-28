using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeFigure.Quad
{
    [Description("作直角梯形")]
    public class MakeRightTrapezoid : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeRightTrapezoid(Point point1, Point point2, Point point3, Point point4)
        {
            Add(point1, point2, point3, point4);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}直角梯形";

        public override void Normalize()
        {
        }
    }

}
