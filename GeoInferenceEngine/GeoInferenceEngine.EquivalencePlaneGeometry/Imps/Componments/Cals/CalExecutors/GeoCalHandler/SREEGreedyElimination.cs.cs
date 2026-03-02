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

        [ZDI]
        public TargetBase _targetBase { get; set; }

        // ========================================================
        // 【核心新增】：结对记忆池，记录已经发生过反应的两个等式 HashCode
        // ========================================================
        private HashSet<string> _processedPairs = new HashSet<string>();

        public void ExecuteElimination()
        {



            if (_knowledgeBase == null || _knowledgeAddProcessor == null || _calAddProcessor == null) return;
            if (!_knowledgeBase.Categories.ContainsKey(typeof(SREE))) return;

            // 获取所有 SREE（不再使用 IsSubsumed 过滤，所有等式都可以参与多元反应）
            var activeSrees = _knowledgeBase.Categories[typeof(SREE)].OfType<SREE>().ToList();
            if (activeSrees.Count < 2) return;

            bool systemChanged = true;

            // 只要系统还能被贪心消元化简，就继续循环
            while (systemChanged)
            {
                systemChanged = false;
                // 把新生成的 SREE 和它的两个父节点打包存起来
                var newGeneratedSrees = new List<(SREE newSree, SREE parent1, SREE parent2)>();

                // 遍历所有可能的等式对
                for (int i = 0; i < activeSrees.Count; i++)
                {
                    for (int j = i + 1; j < activeSrees.Count; j++)
                    {
                        var eq1 = activeSrees[i];
                        var eq2 = activeSrees[j];

                        // 1. 生成这对等式的唯一ID（小号在前大号在后，保证无序对唯一）
                        string pairId = eq1.HashCode < eq2.HashCode
                            ? $"{eq1.HashCode}_{eq2.HashCode}"
                            : $"{eq2.HashCode}_{eq1.HashCode}";

                        // 2. 如果这对等式已经结合过了，直接跳过，防止死循环！
                        if (_processedPairs.Contains(pairId)) continue;

                        // 标记为已处理
                        _processedPairs.Add(pairId);

                        // 3. 寻找消元项
                        SLR targetToEliminate = null;
                        bool isDivision = false;

                        foreach (var slr1 in eq1.Properties.OfType<SLR>())
                        {
                            foreach (var slr2 in eq2.Properties.OfType<SLR>())
                            {
                                if (slr1.IsSameRatio(slr2))
                                {
                                    targetToEliminate = slr1;
                                    isDivision = true; goto Found;
                                }
                                else if (slr1.IsInverseRatio(slr2))
                                {
                                    targetToEliminate = slr1;
                                    isDivision = false; goto Found;
                                }
                            }
                        }

                    Found:
                        // 4. 执行合并
                        if (targetToEliminate is not null)
                        {
                            SREE combinedEq = SREECombiner.CombineAndCancel(eq1, eq2, isDivision);

                            // ========================================================
                            // 【防爆卡口】：只保留化简后剩余 SLR 数量 <= 4 的等式！
                            // 两个梅涅劳斯合并后恰好是 4 项，符合条件。5项以上的直接抛弃。
                            // ========================================================
                            if (combinedEq.Properties.Count <= 4)
                            {
                                // 将它们打包成一个元组塞进列表
                                newGeneratedSrees.Add((combinedEq, eq1, eq2));
                            }
                        }
                    }
                }
                // 5. 将本轮新生成的“精简等式”加入系统，触发下一轮连锁反应
                if (newGeneratedSrees.Count > 0)
                {
                    foreach (var tuple in newGeneratedSrees)
                    {
                        // 从元组中解包提取出一家三口
                        var newEq = tuple.newSree;
                        var eq1 = tuple.parent1;
                        var eq2 = tuple.parent2;
                        // 放进几何知识库
                        _knowledgeAddProcessor.Add(newEq);

                        // 放进代数等式库 (代数引擎会自动把分子分母都有的 CB 约掉)
                        GeoEquation geoEq = newEq.ToGeoEquation();
                        geoEq.AddCondition("消元", eq1, eq2);
                        _knowledgeAddProcessor.Add(geoEq);

                        // ================= 提前嗅探，狙击目标 =================
                        if (_targetBase is not null)
                        {
                            // 标记是否命中的 flag
                            bool isHit = false;

                            // 1. 检查专用的方程目标池 (以防后续系统改版用到)
                            foreach (var targetInfo in _targetBase.EquationTargetInfos)
                            {
                                GeoEquation targetEq = targetInfo.Target;
                                if (geoEq.HashCode == targetEq.HashCode ||
                                    geoEq.ToString() == targetEq.ToString() ||
                                    (geoEq.LeftPart.ToString() == targetEq.RightPart.ToString() && geoEq.RightPart.ToString() == targetEq.LeftPart.ToString()))
                                {
                                    isHit = true; break;
                                }
                            }

                            // 2. 【核心修复】检查普通的几何知识目标池 (解析器通常把输入的等式放在这里！)
                            if (!isHit)
                            {
                                foreach (var unProved in _targetBase.KnowledgeTargetInfos)
                                {
                                    // 尝试把普通的 Knowledge 强转为 GeoEquation
                                    if (unProved.Target is GeoEquation targetEq)
                                    {
                                        if (geoEq.HashCode == targetEq.HashCode ||
                                            geoEq.ToString() == targetEq.ToString() ||
                                            (geoEq.LeftPart.ToString() == targetEq.RightPart.ToString() && geoEq.RightPart.ToString() == targetEq.LeftPart.ToString()))
                                        {
                                            isHit = true; break;
                                        }
                                    }
                                    // 退一步：如果类型不完全是 GeoEquation，但是底层的代数表达式 Expr 一模一样
                                    else if (unProved.Target.Expr != null && geoEq.Expr != null)
                                    {
                                        if (unProved.Target.Expr.Equals(geoEq.Expr))
                                        {
                                            isHit = true; break;
                                        }
                                    }
                                }
                            }

                            if (isHit)
                            {
                                // 完美命中目标！直接强行终止整个贪心消元过程，不再浪费时间！
                                return;
                            }
                        }
                        // ==========================================================


                        // 加入活跃列表，让它能在下一轮和别人继续反应
                        activeSrees.Add(newEq);
                    }
                    systemChanged = true;
                }
            }
        }
    }
}
            
