using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeAngle
{
    [Description("作角平分线", "作角平分")]
    public class MakeAngleBisector : ConstructiveKnowledge
    {
        /// <summary>
        /// 作角平分
        /// </summary>
        /// <param name="angle">原角</param>
        /// <param name="rBisectorPoint">平分点</param>
        /// <param name="rAngle1">分角1</param>
        /// <param name="rAngle2">分角2</param>
        public MakeAngleBisector(Angle angle, Point rBisectorPoint)
        {
            Add(angle);
            Add(rBisectorPoint);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的角平分线{Properties[1]}";

        public override void Normalize()
        {
        }
    }
}
