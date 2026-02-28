namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments
{
    [Description("抽象计算处理")]
    public abstract class ACalExecutor : IInferenceComponent
    {
        public abstract void Init();
        public abstract void Do();
    }
    [Description("啥都不算的计算处理")]
    public class EmptyCalExecutor : ACalExecutor
    {
        public override void Init() {; }
        public override void Do() {; }
    }
}
