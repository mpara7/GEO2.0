using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeAngle
{
    [Description("作三等分角", "作角三等分")]
    public class MakeAngleTriBisector : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeAngleTriBisector(Angle angle, Point rbisector1, Point rbisector2, Angle rAngle1, Angle rAngle2, Angle rAngle3)
        {
            Add(angle);
            AddResult(rbisector1, rbisector2, rAngle1, rAngle2, rAngle3);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的三平分线{Properties[0][1]}{Properties[1]},{Properties[0][1]}{Properties[2]}得到{Properties[3]}{Properties[4]}{Properties[5]}";

        public override void Normalize()
        {
        }
    }
}
