using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeFigure.Quad
{
    [Description("作长方形")]
    public class MakeRect : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeRect(Point point1, Point point2, Point point3, Point point4)
        {
            Add(point1, point2, point3, point4);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}长方形";

        public override void Normalize()
        {
        }
    }

}
