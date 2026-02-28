using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakePoint.ConstraintPoint.Tri
{
    /// <summary>
    /// 内切圆圆心
    /// </summary>
    [Description("作三角形内心")]
    public class MakeTriangleIncenter : ConstructiveKnowledge
    {
        public MakeTriangleIncenter(Triangle triangle, Point rPoint)
        {
            Add(triangle, rPoint);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的内心{Properties[1]}";
        public override void Normalize()
        {
        }
    }
}
