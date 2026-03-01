using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalExecutors.GeoCalHandler;
using GeoInferenceEngine.PlaneKnowledges.Engine.Comps.GeoCal;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalByCombination
{
    [Description("陈梓聪测试")]
    internal class CombinationCalExecutor : ACalExecutor
    {
        [ZDI]
        private CalHanderStrategy calHanderStrategy { get; set; }
        [ZDI]
        private SREEGreedyElimination _sreeGreedyElimination { get; set; }

        public override void Do()
        {
            calHanderStrategy.AddingEquations();
            calHanderStrategy.MultiplyingEquations();
            // ================== 第 2 步：在这里执行贪心消元！ ==================
            // 为什么放在这里？
            // 因为在梅涅劳斯定理产生 SREE 知识之后，或者系统刚好做完一轮基础的 MultiplyingEquations 之后，
            // 此时调用贪心消元，能以极快的速度把冗余的比例式“吃掉”，极大减轻后续高斯消元的计算压力！
            //_sreeGreedyElimination.ExecuteElimination();
            calHanderStrategy.CalGaussianElimination();
        }

        public override void Init()
        {
        }
    }
}