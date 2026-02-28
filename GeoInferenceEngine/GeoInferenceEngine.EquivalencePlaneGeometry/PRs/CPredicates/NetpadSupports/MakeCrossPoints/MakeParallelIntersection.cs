using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeSegment
{
    [Description("平行相交的交点")]
    public class MakeParallelIntersection : Knowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeParallelIntersection(Point point1, Line parallel, Line intersect, Point rPoint, Line rSegment)
        {
            Add(point1, parallel, intersect, rPoint, rSegment);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作过{Properties[0]}与{Properties[1]}平行且相交于{Properties[2]}的平行线{Properties[4]}，交点是{Properties[3]}";

        public override void Normalize()
        {
        }

    }
}
