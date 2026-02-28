using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakePoint.ConstraintPoint.Tri
{
    /// <summary>
    /// 中线中心
    /// </summary>
    [Description("作三角形重心")]
    public class MakeTriangleCentroid : ConstructiveKnowledge
    {
        public MakeTriangleCentroid(Triangle triangle, Point rPoint)
        {
            Add(triangle, rPoint);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的重心{Properties[1]}";
        public override void Normalize()
        {
        }
    }
}
