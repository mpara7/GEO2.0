using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.StopJudges
{
    [Description("知识发现停机判断器")]
    internal class KnowledgeFindingStopJudge : AStopJudge
    {
        [ZDI]
        TargetBase targetBase;

        internal override void Init()
        {
            if (targetBase.ToSolves.Count != 0 || targetBase.ToProves.Count != 0)
            {
                AppInfo.WarningInfo.Warnings.Add("知识发现不支持题目求解");
            }

        }
        internal override void JudgeStop()
        {
            var finish = EngineInfo.IsOutOfPair && !EngineInfo.HasNewKnowledge && !EngineInfo.HasNewEquation;
            if (finish)
            {
                
               
                
                AppInfo.AppStatu = AppStatus.Finished;
            }

        }
    }
}
