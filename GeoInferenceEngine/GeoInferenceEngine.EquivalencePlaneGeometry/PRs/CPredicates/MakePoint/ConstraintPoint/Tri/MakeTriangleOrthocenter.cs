using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakePoint.ConstraintPoint.Tri
{
    /// <summary>
    /// 三条高的交点
    /// </summary>
    [Description("作三角形垂心")]
    public class MakeTriangleOrthocenter : ConstructiveKnowledge
    {
        public MakeTriangleOrthocenter(Triangle triangle, Point rPoint)
        {
            Add(triangle, rPoint);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的垂心{Properties[1]}";
        public override void Normalize()
        {
        }
    }
}
