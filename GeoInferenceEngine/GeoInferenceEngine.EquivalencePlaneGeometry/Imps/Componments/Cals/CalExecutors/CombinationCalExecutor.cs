using GeoInferenceEngine.PlaneKnowledges.Engine.Comps.GeoCal;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalByCombination
{
    [Description("陈梓聪测试")]
    internal class CombinationCalExecutor : ACalExecutor
    {
        [ZDI]
        private CalHanderStrategy calHanderStrategy { get; set; }

        public override void Do()
        {
            calHanderStrategy.AddingEquations();
            calHanderStrategy.MultiplyingEquations();
            calHanderStrategy.CalGaussianElimination();
        }

        public override void Init()
        {
        }
    }
}