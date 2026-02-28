using GeoInferenceEngine.Backbone;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns
{
    [InferenceStandPlugIn]
    [Description("知识正确性检查插件")]
    internal class KnowledgeCorrectnessCheckingPlugIn : IInferencePlugIn
    {
        [ZDI]
        AppInfo AppInfo { get; set; }   
        [ZDI]
        KnowledgeAddProcessor AddProcessor { get; set; }
        public void Init()
        {
            AddProcessor.OnAddNew = (knowledge) =>
            {
                if (knowledge is LineParallel parallel)
                {
                    if (parallel.Line1.Properties.Exists(parallel.Line2.Properties.Contains))
                    {
                        AppInfo.IsActivedStop = true;
                        AppInfo.ActivedPauseReasons.Add($"{parallel}平行线不应该出现相交点");
                    }
                }
            };
        }
        
    }
}
