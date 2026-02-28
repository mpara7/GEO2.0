using GeoInferenceEngine.Backbone;
namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.StopJudges
{
    [Description("抽象停机判断器")]
    internal abstract class AStopJudge : IInferenceComponent
    {
        [ZDI]
        protected AppInfo AppInfo;
        [ZDI]
        protected EngineInfo EngineInfo;
        internal abstract void Init();
        internal abstract void JudgeStop();
    }
}
