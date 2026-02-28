using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeAngle
{
    [Description("作逆时针时针扩张角")]
    public class MakeCounterClockwiseDoubleAngle : ConstructiveKnowledge
    {
        /// <summary>
        /// s3与extand接近
        /// rAngle2是大的
        /// </summary>
        /// <param name="points"></param>
        public MakeCounterClockwiseDoubleAngle(Point s1, Point top, Point s3, Point extand, Angle rAngle1, Angle rAngle2)
        {
            Add(s1, top, s3, extand, rAngle1, rAngle2);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}的扩张{Properties[1]}得到{Properties[2]}{Properties[3]}";

        public override void Normalize()
        {
        }
    }
}
