using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeAngle
{
    [Description("作逆时针角")]
    public class MakeCounterClockwiseAngle : ConstructiveKnowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeCounterClockwiseAngle(Point top, Point side, Expr value, Point rPoint, Angle rAngle)
        {
            Add(top, side, value, rPoint, rAngle);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"以{Properties[0]}为顶点旋转{Properties[1]}{Expr}度得到{Properties[2]}得到{Properties[3]}";

        public override void Normalize()
        {
        }
    }
}
