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

        // 【核心新增】注入代数方程库，停机判断器必须查这里才知道等式有没有证出来
        [ZDI]
        public FormularBase formularBase { get; set; }
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
            // ================= 2. 【核心修复：检查等式证明目标】 =================
            if (formularBase != null)
            {
                foreach (var unProvedEq in targetBase.EquationTargetInfos.Where(i => !i.IsSuccess).ToList())
                {
                    // 方法 A：优先使用 HashCode 精确匹配 (左边右边结构完全一致)
                    if (formularBase.AllGeoEquationInfos.ContainsKey(unProvedEq.Target.HashCode))
                    {
                        unProvedEq.IsSuccess = true;
                        unProvedEq.Conclusion = formularBase.AllGeoEquationInfos[unProvedEq.Target.HashCode].GeoEquation;
                        AppInfo.IsActivedStop = true;
                    }
                    else
                    {
                        // 方法 B：退源匹配防漏网。
                        // 如果输入的证明是 1 = X/Y，推导出的是 X/Y = 1，HashCode 会不一样，这里做交叉防漏判定
                        foreach (var sysEqInfo in formularBase.DistanceMultiplicationGeoEquationInfos.Values)
                        {
                            bool isMatch =
                                sysEqInfo.GeoEquation.ToString() == unProvedEq.Target.ToString() ||
                                (sysEqInfo.GeoEquation.LeftPart.ToString() == unProvedEq.Target.RightPart.ToString() &&
                                 sysEqInfo.GeoEquation.RightPart.ToString() == unProvedEq.Target.LeftPart.ToString());

                            if (isMatch)
                            {
                                unProvedEq.IsSuccess = true;
                                unProvedEq.Conclusion = sysEqInfo.GeoEquation;
                                AppInfo.IsActivedStop = true;
                                break;
                            }
                        }
                    }
                }
            }
            // =================================================================
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
