using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.Strategy;
using System.Reflection;
using DescriptionAttribute = ZTool.UsefulTypes.DescriptionAttribute;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns
{
    public class Fake : PlaneRuleClass
    {
        [ZDI]
        KnowledgeBase KnowledgeBase { get; set; }
        public List<Knowledge> Strage(Segment seg1, Segment seg2)
        {
            return new List<Knowledge>() { new Point("ddqq") };
        }
    }
    public class AuxAddPlugInConfig : AInferenceSetting
    {
        public int MaxRetryTimes { get; set; } = 3;
    }
    [Description("辅助线添加插件")]
    [InferenceStandPlugIn]
    public class AuxAddPlugIn : IInferencePlugIn
    {
        [ZDI]
        ZDIContainer Container;
        ZDIContainer container;
        [ZDI]
        IEnginePreparer _enginePreparer;
        [ZDI]
        KnowledgeBase KnowledgeBase { get; set; }
        [ZDI]
        Logger Logger;
        [ZDI]
        AppInfo AppInfo;
        [ZDI]
        AuxAddPlugInConfig config;
        [ZDI]
        AStopJudge stopJudge;
        [ZDI]
        PlaneExecutor executor;

        [ZDI]
        protected KnowledgeAddProcessor AddProcessor { get; set; }

        SolvingStopJudge solvingStopJudge;
        public int retryTimes = 0;

        public int state = 0; // 用于判断是否第一次进入该模块

        public int index = 0; //标记此时运行到第几个策略


        public void Init()
        {
            container = new ZDIContainer(Container);

            solvingStopJudge = stopJudge as SolvingStopJudge;
            if (solvingStopJudge is null)
            {
                AppInfo.WarningInfo.Warnings
                    .Add($"辅助线添加插件:{typeof(AStopJudge)}必须是{typeof(SolvingStopJudge)}\n插件将不会启用");
            }
            else
            {
                FindAllStragerMaker();
                solvingStopJudge.OnJudgeStop += () =>
                {
                    if(state == 0)
                    {
                        
                        //生成策略
                        MakePairs();
                        MakeStrager();
                        //对返回count为0的list的策略进行删除
                        strages = strages.Where(innerList => innerList.Any()).ToList();
                        //对策略进行排序
                        //选择一个策略运行,这里暂时选择第一个
                        Logger.Info($"尝试第{index}组辅助线策略");
                        executeStrategy(index);
                        state++;
                    }
                    else
                    {
                        cleanStrategy(index);
                        Logger.Info($"尝试第{index+1}组辅助线策略");
                        executeStrategy(++index);
                       
                    }
                        
                 };
            }
        }

        

        List<RuleInfo> ruleInfos = new List<RuleInfo>();
        public void FindAllStragerMaker()
        {
            List<Type> ruleTypes = new List<Type>()
            {
                typeof(AngleBisectorStrategy),
                typeof(MidPointStrategy),
                typeof(QuadrilateralStrategy),
                typeof(TriangleStrategy)};
            foreach (var ruleType in ruleTypes)
            {
                container.SetSingleton(ruleType);
                var ruleClass = container.Get(ruleType);

                var methods = ruleType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var methodInfo in methods)
                {
                    if (methodInfo.DeclaringType == ruleType)
                    {
                        RuleInfo runningRule = new RuleInfo(methodInfo);
                        runningRule.RuleClass = (PlaneRuleClass)ruleClass;
                        ruleInfos.Add(runningRule);
                    }
                }
            }
        }

        List<RuleInfo> usingRuleInfos = new List<RuleInfo>();
        List<(RuleInfo, Knowledge[])> pairs = new();
        public void MakePairs()
        {
            foreach (var ruleInfo in ruleInfos)
            {
                if (ruleInfo.LeftTypes.TrueForAll(type => KnowledgeBase.Categories.ContainsKey(type)))
                {
                    usingRuleInfos.Add(ruleInfo);
                }
            }
            foreach (var ruleInfo in usingRuleInfos)
            {
                if (ruleInfo.LeftTypes.Count == 1)
                {
                    var ks = makePair1(ruleInfo.LeftTypes[0]);
                    foreach (var pair in ks)
                    {
                        pairs.Add((ruleInfo, pair));
                    }
                }
                else if (ruleInfo.LeftTypes.Count == 2)
                {
                    var ks = makePair2(ruleInfo.LeftTypes[0], ruleInfo.LeftTypes[1]);
                    foreach (var pair in ks)
                    {
                        pairs.Add((ruleInfo, pair));
                    }
                }
            }
        }

        List<Knowledge[]> makePair1(Type type)
        {
            List<Knowledge> knowledges = KnowledgeBase.Categories[type];
            return knowledges.Select(k => new Knowledge[] { k }).ToList();
        }
        List<Knowledge[]> makePair2(Type type1, Type type2)
        {
            List<Knowledge> knowledge1s = KnowledgeBase.Categories[type1];
            List<Knowledge> knowledge2s = KnowledgeBase.Categories[type2];
            List<Knowledge[]> result = new List<Knowledge[]>();
            foreach (var item in knowledge1s)
            {
                foreach (var item2 in knowledge2s)
                {
                    result.Add(new Knowledge[] { item, item2 });
                }
            }
            return result;
        }
       
        List<List<Knowledge>> strages = new List<List<Knowledge>>();
        public void MakeStrager()
        {
            foreach (var item in pairs)
            {
                if (item.Item2.ToList().TrueForAll(p => p.IsAvailable))
                {
                    var result = item.Item1.RuleMethod.Invoke(item.Item1.RuleClass, item.Item2);
                    strages.Add((List<Knowledge>)result);
                }
              
            }
        }

        public void executeStrategy(int i)
        {
            if(i <strages.Count)
            {
                List<Knowledge> strage = strages[i]; // 选择一个策略执行
                for (int j = 0; j < strage.Count; j++)
                {
                    Knowledge knowledge = strage[j];  // 获取当前的 Knowledge 对象
                    if(knowledge is Line)
                    {
                        AddProcessor.Add((Line)knowledge);
                    }
                    else if (knowledge is Angle)
                    {
                        AddProcessor.Add((Angle)knowledge);
                    }
                    else
                    {
                        AddProcessor.Add(knowledge);
                    }
                    // 处理每个 Knowledge 对象

                }
                executor.DoInternal();
                executor.DoAutoGen();
                executor.Init();
                //进入再次推理逻辑
                AppInfo.AppStatu = AppStatus.Running;

            }
           

        }

        public void cleanStrategy(int i)
        {
            List<Knowledge> strage = strages[i];  // 选择一个策略清理
            for (int j = 0; j < strage.Count; j++)
            {
                Knowledge knowledge = strage[j];  // 获取当前的 Knowledge 对象
                AddProcessor.removeKnowledge(knowledge); // 处理每个 Knowledge 对象

            }

        }

    }
}
