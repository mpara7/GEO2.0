using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors
{
    [Description("什么都不做的执行器(用于测试组件功能)")]
    internal class DoNothingExecutor : PlaneExecutor
    {
        [ZDI]
        KnowledgeBase KnowledgeBase { get; set; }
        
        internal override void Init()
        {
            base.Init();
        }
        internal override void Step()
        {
            EngineInfo.IsOutOfPair = true;
            EngineInfo.HasNewEquation = false;
            EngineInfo.HasNewKnowledge = false;
            
        }
    }

    public class TimeConsumingExecutorConfig : AInferenceSetting
    {
        public int WaitTime { get; set; } = 15;
    }
    [Description("耗时执行器(用于测试组件功能)")]
    internal class TimeConsumingExecutor : PlaneExecutor
    {
        [ZDI]
        TimeConsumingExecutorConfig config;
        [ZDI]
        KnowledgeAddProcessor AddProcessor { get; set; }
        [ZDI]
        KnowledgeBase KnowledgeBase { get; set; }
        internal override void Init()
        {
            base.Init();
        }
        int i = 0;
        internal override void Step()
        {
            if (i >= config.WaitTime)
            {
                EngineInfo.IsOutOfPair = true;
                EngineInfo.HasNewEquation = false;
                EngineInfo.HasNewKnowledge = false;
            }
            else
            {
                i++;
                Point pred = new Point($"A{i}");
                AddProcessor.Add(pred);
                Thread.Sleep(1000);
            }

        }
    }

    [Description("制造错误的执行器(用于测试组件功能)")]
    internal class MakeErrorExecutor : PlaneExecutor
    {
        [ZDI]
        KnowledgeAddProcessor AddProcessor { get; set; }
        [ZDI]
        KnowledgeBase KnowledgeBase { get; set; }
        internal override void Init()
        {
            base.Init();
        }
        int i = 0;
        internal override void Step()
        {
            if (i >= 3)
            {
                EngineInfo.IsOutOfPair = true;
                EngineInfo.HasNewEquation = false;
                EngineInfo.HasNewKnowledge = false;
                throw new Exception();
            }
            else
            {
                i++;
                Point pred = new Point($"A{i}");
                AddProcessor.Add(pred);
                Thread.Sleep(1000);
            }

        }
    }
}
