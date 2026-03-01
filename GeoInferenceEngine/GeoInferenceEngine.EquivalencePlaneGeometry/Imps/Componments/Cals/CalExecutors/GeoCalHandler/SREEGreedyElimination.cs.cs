using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoInferenceEngine.Knowledges.Imps.Componments;
using GeoInferenceEngine.PredicateShared.Models;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Predicates.Relations.GeoPropRelations;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalExecutors.GeoCalHandler
{
    public class SREEGreedyElimination : IInferenceComponent
    {
        [ZDI]
        private KnowledgeBase _knowledgeBase { get; set; }

        [ZDI]
        private KnowledgeAddProcessor _knowledgeAddProcessor { get; set; }

        public void ExecuteElimination()
        {
            // 获取当前系统所有由梅涅劳斯等定理生成的 SREE 等式
            var srees = _knowledgeBase.GetKnowledgesByType<SREE>().ToList();
            if (srees.Count < 2) return;

            bool systemChanged = true;

            // 只要系统还能被贪心消元化简，就继续循环
            while (systemChanged)
            {
                systemChanged = false;

                // 1. 统计所有的 SLR 签名，寻找可以作为“消元目标”的项
                var slrPool = srees.SelectMany(s => s.Properties.Cast<SLR>()).ToList();

                SLR targetToEliminate = null;
                SREE pivotEq = null;
                SREE targetEq = null;
                bool isDivision = false;

                // 2. 寻找任意两个包含相同（或互为倒数）SLR 的 SREE 方程
                foreach (var eq1 in srees)
                {
                    foreach (var eq2 in srees)
                    {
                        if (eq1 == eq2) continue;

                        foreach (var slr1 in eq1.Properties.Cast<SLR>())
                        {
                            foreach (var slr2 in eq2.Properties.Cast<SLR>())
                            {
                                // 发现两个方程包含完全相同的项 -> 除法消元
                                if (slr1.IsSameRatio(slr2))
                                {
                                    pivotEq = eq1; targetEq = eq2;
                                    targetToEliminate = slr1;
                                    isDivision = true; goto Found;
                                }
                                // 发现两个方程包含互为倒数的项 -> 乘法消元
                                else if (slr1.IsInverseRatio(slr2))
                                {
                                    pivotEq = eq1; targetEq = eq2;
                                    targetToEliminate = slr1;
                                    isDivision = false; goto Found;
                                }
                            }
                        }
                    }
                }

            Found:
                // 3. 执行消元
                if (targetToEliminate != null)
                {
                    // 把 eq1 和 eq2 组合，约掉 targetToEliminate
                    SREE combinedEq = SREECombiner.CombineAndCancel(pivotEq, targetEq, isDivision, null);

                    // 【核心】从工作区中移除老的，防止产生 10000 条废方程的组合爆炸！
                    srees.Remove(pivotEq);
                    srees.Remove(targetEq);
                    srees.Add(combinedEq);

                    // 将成功化简出的、更短的等式加入知识库
                    _knowledgeAddProcessor.Add(combinedEq);

                    systemChanged = true;
                }
            }
        }
    }
}
