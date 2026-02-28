using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.PairMakers;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.RuleProviders;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors
{
    [Description("平面几何执行器")]
    public class PlaneExecutor : IInferenceComponent
    {
        [ZDI]
        protected RuleBase RuleBase;
        [ZDI]
        protected KnowledgeBase KBase;
        [ZDI]
        protected FormularBase FBase;
        [ZDI]
        protected PairMaker PairMaker;
        [ZDI]
        protected RuleProvider RuleProvider;
        [ZDI]
        protected EngineInfo EngineInfo;
        [ZDI]
        protected AppInfo AppInfo;
        [ZDI]
        protected Logger Logger;
        [ZDI]
        protected ACalExecutor CalExecutor;
        public Action Initing { get; set; }
        public Action DoingAutoGen { get; set; }
        public Action Inited { get; set; }
        public Action Steping { get; set; }
        public Action Steped { get; set; }
        public Action<MethodInfo, Knowledge[]> OnNewInvoke { get; set; }
        public Action OnOutofPair { get; set; }

        public void DoConstructiveRule()
        {
            RuleProvider.BuildRules(PlaneRuleType.Constructive);
            PairMaker.Reload();
            Logger.Info("准备转换构造线语句");
            EngineInfo.IsOutOfPair = false;

            while (!EngineInfo.IsOutOfPair)
            {
                if (PairMaker.HasNextPair())
                {
                    var pair = PairMaker.GetNextPair();
                    if (pair.args.ToList().TrueForAll(p => p.IsAvailable))
                    {
                        pair.rule.Invoke(pair.@class, pair.args);
                    }
                }
                else
                {
                    EngineInfo.IsOutOfPair = true;
                }
            }
        }
        public void DoInternal()
        {
            RuleProvider.BuildRules(PlaneRuleType.Internal);
            PairMaker.Reload();
            Logger.Info("准备知识继承关系推理");
            EngineInfo.IsOutOfPair = false;

            while (!EngineInfo.IsOutOfPair)
            {
                if (PairMaker.HasNextPair())
                {
                    var pair = PairMaker.GetNextPair();
                    if (pair.args.ToList().TrueForAll(p => p.IsAvailable))
                    {
                        pair.rule.Invoke(pair.@class, pair.args);
                    }
                }
                else
                {
                    EngineInfo.IsOutOfPair = true;
                }
            }
        }
        public void DoAutoGen()
        {
            DoingAutoGen?.Invoke();
            RuleProvider.BuildRules(PlaneRuleType.AutoGenerate);
            PairMaker.Reload();
            Logger.Info("准备自动生成几何对象");
            EngineInfo.IsOutOfPair = false;
            
            while (!EngineInfo.IsOutOfPair)
            {
                if (PairMaker.HasNextPair())
                {
                    var pair = PairMaker.GetNextPair();
                    if (pair.args.ToList().TrueForAll(p => p.IsAvailable))
                    {
                        pair.rule.Invoke(pair.@class, pair.args);
                    }
                }
                else
                {
                    EngineInfo.IsOutOfPair = true;
                }
            }
        }

        internal virtual void Init()
        {
            Initing?.Invoke();
            RuleProvider.BuildRules(PlaneRuleType.Tradition, PlaneRuleType.Internal);
            PairMaker.Reload();
            Logger.Info("准备使用DD+AR推理");
            EngineInfo.IsOutOfPair = false;
            CalExecutor.Init();
            Inited?.Invoke();
        }
        int curRound = -1;
        internal virtual void Step()
        {
            Steping?.Invoke();

            EngineInfo.NewKnowledges = new();
            EngineInfo.HasNewEquation = false;
            EngineInfo.HasNewKnowledge = false;

            if (PairMaker.HasNextPair())
            {
                var pair = PairMaker.GetNextPair();
                if (pair.args.ToList().TrueForAll(p => p.IsAvailable))
                {
                    OnNewInvoke?.Invoke(pair.rule, pair.args);
                    pair.rule.Invoke(pair.@class, pair.args);


                }
            }
            else
            {
                EngineInfo.IsOutOfPair = true;
                OnOutofPair?.Invoke();
            }
            if (curRound != PairMaker.Round)
            {
                Logger.Info($"开启第{PairMaker.Round}轮的AR推理");
                CalExecutor.Do();
                curRound = PairMaker.Round;
            }
            Steped?.Invoke();
        }
    }
}
