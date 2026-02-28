using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives
{
    public class MakeThreePointArc : Knowledge
    {
        public MakeThreePointArc(Point other, Point start, Point end, Arc rArc)
        {
            Add(other, start, end, rArc);
            Normalize();
            SetHashCode();
        }
        public override void Normalize()
        {
        }
        public override string ToString()
        {
            return $"作以{Properties[0]}{Properties[1]}{Properties[2]}的圆心作{Properties[3]}";
        }
    }
}
