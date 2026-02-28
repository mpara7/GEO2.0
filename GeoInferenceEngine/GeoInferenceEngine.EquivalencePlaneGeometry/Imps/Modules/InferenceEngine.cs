
using GeoInferenceEngine.Backbone.AppConfiger;
using GeoInferenceEngine.EquivalencePlaneGeometry.Configs.ConfigerPlugIns;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors;
using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Engine.Modules
{
    [Description("引擎状态信息")]
    public class EngineInfo : IInferenceDatabase
    {
        public string RunningRuleName { get; set; }
        /// <summary>
        /// 新知识
        /// </summary>
        public List<Knowledge> NewKnowledges { get; set; } = new();
        public bool HasNewKnowledge { get; set; }
        public bool HasNewEquation
        {
            get;
            set;
        }
        public bool IsOutOfPair { get; set; }
    }
    [Description("推理引擎")]
    [ConfigRequirePlugIns(typeof(RuleRequireConfigConfigerPlugIn))]
    internal class InferenceEngine : IInferenceEngine
    {
        [ZDI]
        AStopJudge StopJudge { get; set; }
        [ZDI]
        PlaneExecutor Executor { get; set; }
        public void Init()
        {
            Executor.DoConstructiveRule();
            Executor.DoInternal();
            Executor.DoAutoGen();
            Executor.Init();
            StopJudge.Init();
        }
        public void StepForward()
        {
            StopJudge.JudgeStop();
            Executor.Step();
        }
        public void Release()
        {
        }


    }
}
