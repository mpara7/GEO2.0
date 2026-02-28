using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CPredicates.NetpadSupports.MakePolys
{
    [Description("作正方形")]
    public class MakeSquare : ConstructiveKnowledge
    {
        public MakeSquare(Point p1, Point p2, Point p3, Point p4, Square square)
        {
            Add(p1, p2, p3, square);
            Normalize();
            SetHashCode();
        }
        public override void Normalize()
        {
        }
        public override string ToString()
        {
            return $"作{Properties[3]}";
        }
    }
}
