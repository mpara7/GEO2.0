using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.StopJudges
{
    [Description("题目求解停机判断器")]
    internal class SolvingStopJudge : AStopJudge
    {
        public Action OnJudgeStop;
        [ZDI]
        KnowledgeBase knowledgeBase;
        [ZDI]
        TargetBase targetBase;
        internal override void Init()
        {
            if (targetBase.ToSolves.Count == 0 && targetBase.ToProves.Count == 0)
            {
                AppInfo.IsActivedStop = true;
                AppInfo.ActivedStopReasons.Add("求解判断器没有目标\n");
            }
        }

        internal override void JudgeStop()
        {
            //检查待证明
            
            var newKnowledges = EngineInfo.NewKnowledges;

            if (newKnowledges.Count > 0)
            {
                foreach (var unProved in targetBase.KnowledgeTargetInfos.Where(i => !i.IsSuccess).ToList())
                {
                    foreach (var newKnowledge in newKnowledges)
                    {
                        var a = UlongTool.UlongToStr(newKnowledge.HashCode);
                        var b = UlongTool.UlongToStr(unProved.Target.HashCode);
                        if (newKnowledge.HashCode == unProved.Target.HashCode)
                        {
                            if (unProved.Target.Expr is null)
                            {
                                unProved.IsSuccess = true;
                                unProved.Conclusion = newKnowledge;
                                //GlobalTimer.Stop();
                            }
                            else
                            {
                                if (newKnowledge.Expr.Equals(unProved.Target.Expr))
                                {
                                    unProved.IsSuccess = true;
                                    unProved.Conclusion = newKnowledge;
                                    AppInfo.IsActivedStop = true;
                                    //GlobalTimer.Stop();
                                }
                                else
                                {
                                    AppInfo.IsActivedStop = true;
                                    AppInfo.ActivedStopReasons.Add($"已证明结果不成立{newKnowledge}\n");
                                    //GlobalTimer.Stop();
                                }
                            }

                        }
                    }
                }

                foreach (var unSolve in targetBase.RelationTargets.Where(i => !i.IsSuccess).ToList())
                {
                    foreach (var newKnowledge in newKnowledges)
                    {
                        var a = UlongTool.UlongToStr(newKnowledge.HashCode);
                        var b = UlongTool.UlongToStr(unSolve.Target.HashCode);
                        if (newKnowledge.HashCode == unSolve.Target.HashCode)
                        {
                            unSolve.IsSuccess = true;
                            unSolve.Conclusion = knowledgeBase.Knowledges[unSolve.Target.HashCode];
                            //GlobalTimer.Stop();
                        }
                    }
                }
            }

            var allSolved = targetBase.ToProves.All(p => p.IsSuccess) && targetBase.ToSolves.All(p => p.IsSuccess);
            var finish = allSolved || EngineInfo.IsOutOfPair && !EngineInfo.HasNewKnowledge && !EngineInfo.HasNewEquation;
            if (finish)
            {
                AppInfo.AppStatu = AppStatus.Finished;
                OnJudgeStop?.Invoke();
            }
        }
    }
}
