using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Modules;
using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns
{
    public class HookNewKnowledgePlugInInfo
    {
        public string Target { get; set; } = "";
        public bool IsUse { get; set; } = true;
    }
    [Description("新知识拦截插件配置")]
    public class HookNewKnowledgePlugInConfig : AInferenceSetting
    {
        public bool IsActivePause { get; set; } = true;
        public List<HookNewKnowledgePlugInInfo> Targets { get; set; } = new();
    }
    [InferenceStandPlugIn]
    [Description("新知识拦截插件")]
    public class HookNewKnowledgePlugIn : IInferencePlugIn
    {
        [ZDI]
        AppInfo AppInfo { get; set; }
        [ZDI]
        ZScriptInputEnginePreparer parser { get; set; }
        [ZDI]
        HookNewKnowledgePlugInConfig config;
        bool isInited = false;
        /// <summary>
        /// 或列表
        /// </summary>
        List<Knowledge> targetKnowledges = new List<Knowledge>();
        [ZDI]
        PlaneExecutor Executor { get; set; }
        public void Init()
        {
            Executor.Inited += () =>
            {
                foreach (var arg in config.Targets.Where(info => info.IsUse))
                {
                    var kInfos = parser.parseKnowledge(arg.Target);
                    targetKnowledges.Add(kInfos);
                }
            };
        }
        public void AddNew(Knowledge newKnowledge)
        {
            if (!isInited)
            {
                Init();
                isInited = true;
            }

            if (targetKnowledges.Exists(k => newKnowledge.HashCode == k.HashCode))
            {
                if (config.IsActivePause)
                {
                    AppInfo.IsActivedPause = true;
                    AppInfo.ActivedPauseReasons.Add($"得到指定知识停机");
                }
                //在此打断点
                return;
            }


        }
    }
}
