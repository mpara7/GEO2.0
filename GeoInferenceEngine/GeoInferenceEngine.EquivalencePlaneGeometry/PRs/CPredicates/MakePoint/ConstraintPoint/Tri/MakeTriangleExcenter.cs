using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakePoint.ConstraintPoint.Tri
{
    /// <summary>
    /// 三个旁切圆的圆心
    /// </summary>
    [Description("作三角形旁心")]
    public class MakeTriangleExcenter : ConstructiveKnowledge
    {
        public MakeTriangleExcenter(Triangle triangle, Point rPoint1, Point rPoint2, Point rPoint3)
        {
            Add(triangle, rPoint1, rPoint2, rPoint3);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的垂心{Properties[1]}、{Properties[2]}、{Properties[3]}";
        public override void Normalize()
        {
        }
    }
}
