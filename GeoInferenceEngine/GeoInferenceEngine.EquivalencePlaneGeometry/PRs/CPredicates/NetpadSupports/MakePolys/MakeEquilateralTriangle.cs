using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.CPredicates.NetpadSupports.MakePolys
{
    [Description("作全等三角形")]
    public class MakeEquilateralTriangle : ConstructiveKnowledge
    {
        public MakeEquilateralTriangle(Point p1, Point p2, Point p3, EquilateralTriangle tri)
        {
            Add(p1, p2, p3, tri);
            Normalize();
            SetHashCode();
        }
        public override void Normalize()
        {
        }
        public override string ToString()
        {
            return $"作全等三角形{Properties[3]}";
        }
    }
}
