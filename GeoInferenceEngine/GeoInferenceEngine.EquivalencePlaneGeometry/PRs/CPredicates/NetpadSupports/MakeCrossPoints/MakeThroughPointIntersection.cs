using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.PRs.CKnowledges.MakeSegment
{
    [Description("过点交于线的交点")]
    public class MakeThroughPointIntersection : Knowledge
    {
        /// <summary>
        /// N A B
        /// </summary>
        /// <param name="points"></param>
        public MakeThroughPointIntersection(Point from, Point through, Line intersect, Point rPoint, Line rSegment)
        {
            Add(from, through, intersect, rPoint, rSegment);
            Normalize();
            SetHashCode();
        }
        public override string ToString() => $"作{Properties[0]}过{Properties[1]}相交于{Properties[2]}的线{Properties[4]}，交点是{Properties[3]}";

        public override void Normalize()
        {
        }

    }
}
