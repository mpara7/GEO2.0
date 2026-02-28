using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakePoint.ConstraintPoint
{
    //[Description("作平移点")]
    public class MakeTransferPoint : ConstructiveKnowledge
    {
        /// <summary>
        /// 多边形
        /// </summary>
        /// <param name="points"></param>
        public MakeTransferPoint(Point from, Point start, Point end, Point rPoint)
        {
            Add(from, start, end, rPoint);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}向x:{Properties[0]}y:{Properties[1]}移动的点{Properties[1]}";

        public override void Normalize()
        {
        }
    }

}
