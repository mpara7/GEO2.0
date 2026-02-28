using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models
{
    [Description("不成立知识")]
    public class NegatingKnowledge : Knowledge
    {
        Knowledge Nega { get; set; }
        public NegatingKnowledge(Knowledge knowledge)
        {
            Nega = knowledge;
            SetHashCode();
        }
        public override void SetHashCode()
        {
            HashCode = ~Nega.HashCode;
        }
        public override void Normalize()
        {

        }
        public override string ToString()
        {
            return $"{Nega}不成立";
        }
    }
}
