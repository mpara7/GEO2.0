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
        [ZDI]
        public CalAddProcessor _calAddProcessor { get; set; }

        public void ExecuteElimination()
        {
            if (_knowledgeBase == null || _knowledgeAddProcessor == null || _calAddProcessor == null) return;
            // 检查全局知识库是否包含 SREE
            if (!_knowledgeBase.Categories.ContainsKey(typeof(SREE))) return;
            var sreesList = _knowledgeBase.Categories[typeof(SREE)];
            if (sreesList == null) return;

            bool systemChanged = true;

            // 只要系统还能被贪心消元化简，就继续循环
            while (systemChanged)
            {
                systemChanged = false;

                // 【核心防御】：只提取那些“还没被吸收废弃”的新鲜 SREE 来操作
                var activeSrees = sreesList.OfType<SREE>().Where(e => !e.IsSubsumed).ToList();
                if (activeSrees.Count < 2) break;

                SLR targetToEliminate = null;
                SREE pivotEq = null;
                SREE targetEq = null;
                bool isDivision = false;

                // 2. 寻找任意两个包含相同（或互为倒数）SLR 的 SREE 方程
                foreach (var eq1 in activeSrees)
                {
                    foreach (var eq2 in activeSrees)
                    {
                        if (eq1 == eq2) continue;

                        foreach (var slr1 in eq1.Properties.OfType<SLR>())
                        {
                            foreach (var slr2 in eq2.Properties.OfType<SLR>())
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
                if (targetToEliminate is not null)
                {
                    // 【软删除机制】：打上吸收标签，不从黑板物理删除，保护推理树
                    pivotEq.IsSubsumed = true;
                    targetEq.IsSubsumed = true;

                    // 生成新方程并交由系统录入
                    SREE combinedEq = SREECombiner.CombineAndCancel(pivotEq, targetEq, isDivision);
                    _knowledgeAddProcessor.Add(combinedEq);

                    // ================== 核心新增 ==================
                    // 2. 将化简后的结果转换为系统原生代数等式，并交给代数引擎！
                    GeoEquation geoEq = combinedEq.ToGeoEquation();
                    geoEq.AddCondition("消元", pivotEq, targetEq);

                    // 将新等式加入系统的代数计算池，供高斯消元等后续模块使用
                    //_calAddProcessor.Add(geoEq);
                    _knowledgeAddProcessor.Add(geoEq);
                    // ==============================================

                    systemChanged = true;
                }
            }
        }
    }
}
