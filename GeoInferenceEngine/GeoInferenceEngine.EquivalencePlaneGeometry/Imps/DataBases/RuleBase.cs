

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases
{
    [Description("规则库")]
    public class RuleBase : IInferenceDatabase
    {
        public List<RuleInfo> RuleInfos { get; set; } = new();
    }
}
