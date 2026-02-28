using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Modules;
using GeoInferenceEngine.Knowledges;
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns
{
    public class PairRequireDebugInfo
    {
        public bool IsUse { get; set; } = true;
        public string SelectedRuleName { get; set; } = "";
        /// <summary>
        /// 与列表 为了界面方便编写做的妥协使用StringValue而不是string
        /// </summary>
        public List<StringValue> WithArgs { get; set; } = new();
    }
    [Description("组队拦截插件配置")]
    public class HookPairKnowledgePlugInConfig : AInferenceSetting
    {
        public bool IsActivePause { get; set; } = true;
        public List<PairRequireDebugInfo> PairRequireDebugInfos { get; set; } = new List<PairRequireDebugInfo>();
    }

    /// <summary>
    /// 插件
    /// </summary>
    [InferenceStandPlugIn]
    [Description("组队拦截插件")]
    public class HookPairKnowledgePlugIn : IInferencePlugIn
    {
        [ZDI]
        PlaneExecutor executor { get; set; }
        [ZDI]
        AppInfo ERInfoBase { get; set; }
        [ZDI]
        ZScriptInputEnginePreparer parser { get; set; }
        class PairDebugInfo
        {
            public string SelectedRuleName { get; set; }
            public List<Knowledge> WithArgs { get; set; } = new();
        }
        [ZDI]
        HookPairKnowledgePlugInConfig config;
        bool isInited = false;
        /// <summary>
        /// 或列表
        /// </summary>
        List<PairDebugInfo> pairDebugInfos = new List<PairDebugInfo>();
        public void Init()
        {
            executor.OnNewInvoke += NewStepInfo;
            executor.Inited += () =>
            {
                foreach (var item in config.PairRequireDebugInfos.Where(info => info.IsUse))
                {
                    PairDebugInfo pairDebugInfo = new PairDebugInfo();
                    pairDebugInfo.SelectedRuleName = item.SelectedRuleName;

                    foreach (var arg in item.WithArgs)
                    {
                        var kInfos = parser.parseKnowledge(arg.Value);
                        pairDebugInfo.WithArgs.Add(kInfos);
                    }
                    pairDebugInfos.Add(pairDebugInfo);
                }
            };
        }
        public void NewStepInfo(MethodInfo ruleInfo, Knowledge[] knowledges)
        {
            if (!isInited)
            {
                Init();
                isInited = true;
            }

            foreach (var pairDebugInfo in pairDebugInfos)
            {
                if (pairDebugInfo.SelectedRuleName == ruleInfo.Name)
                {
                    var isTarget = pairDebugInfo.WithArgs.TrueForAll(tk => knowledges.ToList().Exists(k => k.HashCode == tk.HashCode)); ;
                    if (isTarget)
                    {
                        if (config.IsActivePause)
                        {
                            ERInfoBase.IsActivedPause = true;
                            ERInfoBase.ActivedPauseReasons.Add($"进入指定组队停机");
                        }
                        //在此打断点
                        return;
                    }

                }
            }
        }


    }
}
