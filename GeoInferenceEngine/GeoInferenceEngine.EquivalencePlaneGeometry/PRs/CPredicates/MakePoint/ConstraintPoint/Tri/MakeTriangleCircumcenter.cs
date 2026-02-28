using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakePoint.ConstraintPoint.Tri
{
    /// <summary>
    /// 外接圆圆心
    /// </summary>
    [Description("作三角形外心")]
    public class MakeTriangleCircumcenter : ConstructiveKnowledge
    {
        public MakeTriangleCircumcenter(Triangle triangle, Point rPoint)
        {
            Add(triangle, rPoint);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的外心{Properties[1]}";
        public override void Normalize()
        {
        }
    }
}
