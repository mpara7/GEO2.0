using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoInferenceEngine.Knowledges.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Predicates.Relations.GeoPropRelations
{
    using System.Linq;
    using System.Collections.Generic;
    public static class SREECombiner
    {
        public static SREE CombineAndCancel(SREE eq1, SREE eq2, bool isDivision)
        {
            var factors = new List<SLR>();

            foreach (var prop in eq1.Properties.OfType<SLR>())
                factors.Add(prop);

            foreach (var prop in eq2.Properties.OfType<SLR>())
                factors.Add(isDivision ? prop.GetInverse() : prop);

            // ================= 阶段 1：完全倒数抵消 =================
            bool cancelled = true;
            while (cancelled)
            {
                cancelled = false;
                for (int i = 0; i < factors.Count; i++)
                {
                    for (int j = i + 1; j < factors.Count; j++)
                    {
                        if (factors[i].IsInverseRatio(factors[j]))
                        {
                            factors.RemoveAt(j);
                            factors.RemoveAt(i);
                            cancelled = true;
                            break;
                        }
                    }
                    if (cancelled) break;
                }
            }

            // ================= 阶段 2：局部线段融合 (纯几何约分) =================
            bool merged = true;
            while (merged)
            {
                merged = false;
                for (int i = 0; i < factors.Count; i++)
                {
                    for (int j = i + 1; j < factors.Count; j++)
                    {
                        if (TryMergeSLR(factors[i], factors[j], out SLR newSlr))
                        {
                            // 发现可以首尾相接的线段，删掉旧的，放入融合后的新项！
                            factors.RemoveAt(j);
                            factors.RemoveAt(i);
                            factors.Add(newSlr);
                            merged = true;
                            break;
                        }
                    }
                    if (merged) break;
                }
            }

            // 3. 计算代数表达式 Expr 的乘除
            var product = new ProductNode();
            if (eq1.Expr is not null)
                product.Multipliers.Add(eq1.Expr.Clone());

            if (eq2.Expr is not null)
            {
                if (isDivision)
                    product.Divisors.Add(eq2.Expr.Clone());
                else
                    product.Multipliers.Add(eq2.Expr.Clone());
            }

            Expr newExpr = product.Simplify();

            var newSREE = new SREE(newExpr, factors.ToArray());
            newSREE.AddCondition(eq1, eq2);
            newSREE.AddReason();
            return newSREE;
        }

        // =========================================================
        // 以下为局部融合算法的底层支持方法
        // =========================================================

        /// <summary>
        /// 尝试将两个含有公共首尾线段的 SLR 融合成一个。
        /// 例如：(DC / CB) 和 (CB / BD) -> 返回 (CD / DB)
        /// </summary>
        private static bool TryMergeSLR(SLR a, SLR b, out SLR merged)
        {
            merged = null;

            // a 的分母 (a.point2, a.point3) == b 的分子 (b.point1, b.point2)
            if (IsSameSeg(a.point2, a.point3, b.point1, b.point2))
            {
                // 剩下的就是 a 的分子 和 b 的分母，尝试寻找它们的公共点拼接
                merged = CreateSLRIfShared(a.point1, a.point2, b.point2, b.point3, a.Expr);
                if (merged is not null) return true;
            }
            // a 的分子 (a.point1, a.point2) == b 的分母 (b.point2, b.point3)
            else if (IsSameSeg(a.point1, a.point2, b.point2, b.point3))
            {
                // 剩下的就是 b 的分子 和 a 的分母，尝试寻找它们的公共点拼接
                merged = CreateSLRIfShared(b.point1, b.point2, a.point2, a.point3, a.Expr);
                if (merged is not null) return true;
            }

            return false;
        }

        /// <summary>
        /// 判断两条线段是否是同一条（无视方向）
        /// </summary>
        private static bool IsSameSeg(Point p1, Point p2, Point q1, Point q2)
        {
            return (p1.Equals(q1) && p2.Equals(q2)) ||
                   (p1.Equals(q2) && p2.Equals(q1));
        }

        /// <summary>
        /// 如果剩下的分子线段和分母线段共享一个顶点，就组装成一个标准 SLR 比例
        /// </summary>
        private static SLR CreateSLRIfShared(Point n1, Point n2, Point d1, Point d2, Expr templateExpr)
        {
            Point shared = null;
            if (n1.Equals(d1)) shared = n1;
            else if (n1.Equals(d2)) shared = n1;
            else if (n2.Equals(d1)) shared = n2;
            else if (n2.Equals(d2)) shared = n2;

            // 如果它们连一个公共点都没有，在几何上无法形成标准的 SLR(P1, P2, P3) 连线比例，拒绝融合
            if (shared == null) return null;

            // 提取不共享的另外两个端点
            Point p1 = n1.Equals(shared) ? n2 : n1;
            Point p3 = d1.Equals(shared) ? d2 : d1;

            // 组装成新的 SLR (分子端点 -> 共享点 -> 分母端点)
            return new SLR(p1, shared, p3, templateExpr != null ? templateExpr.Clone() : null);
        }
    }

    //public static class SREECombiner
    //{
    //    /// <summary>
    //    /// 将两个 SREE 相乘或相除，并自动约分掉共同的 SLR
    //    /// isDivision 为 true 表示 eq1 / eq2
    //    /// </summary>
    //    public static SREE CombineAndCancel(SREE eq1, SREE eq2, bool isDivision)
    //    {
    //        var factors = new List<SLR>();

    //        // 1. 放入 eq1 的所有因子
    //        foreach (var prop in eq1.Properties)
    //            factors.Add((SLR)prop);

    //        // 2. 放入 eq2 的所有因子 (如果是除法，则放入倒数)
    //        foreach (var prop in eq2.Properties)
    //        {
    //            var slr2 = (SLR)prop;
    //            factors.Add(isDivision ? slr2.GetInverse() : slr2);
    //        }

    //        // 3. 核心：贪心约分 (互相抵消互为倒数的 SLR)
    //        bool cancelled = true;
    //        while (cancelled)
    //        {
    //            cancelled = false;
    //            for (int i = 0; i < factors.Count; i++)
    //            {
    //                for (int j = i + 1; j < factors.Count; j++)
    //                {
    //                    // 如果存在互为倒数的项，比如 A-B-C 和 C-B-A，就互相抵消
    //                    if (factors[i].IsInverseRatio(factors[j]))
    //                    {
    //                        factors.RemoveAt(j);
    //                        factors.RemoveAt(i);
    //                        cancelled = true;
    //                        break;
    //                    }
    //                }
    //                if (cancelled) break;
    //            }
    //        }

    //        // 4. 代数表达式的乘除化简
    //        Expr newExpr;
    //        if (isDivision)
    //        {
    //            var product = new ProductNode();
    //            product.Multipliers.Add(eq1.Expr);
    //            product.Divisors.Add(eq2.Expr);
    //            newExpr = product.Simplify();
    //        }
    //        else
    //        {
    //            var product = new ProductNode();
    //            product.Multipliers.Add(eq1.Expr);
    //            product.Multipliers.Add(eq2.Expr);
    //            newExpr = product.Simplify();
    //        }

    //        // 5. 生成新的 SREE
    //        var newSREE = new SREE(newExpr, factors.ToArray());
    //        newSREE.AddCondition("贪心消元合并", eq1, eq2);

    //        return newSREE;
    //    }
    //}
    public static class SLRExtensions
    {

        

        /// <summary>
        /// 判断两个 SLR 是否实质上是同一个线段比（例如 A-B-C 和 A-B-C）
        /// </summary>
        public static bool IsSameRatio(this SLR a, SLR b)
        {
            return a.point1.Equals(b.point1) &&
                   a.point2.Equals(b.point2) &&
                   a.point3.Equals(b.point3);
        }

        /// <summary>
        /// 判断两个 SLR 是否互为倒数（例如 A-B-C 和 C-B-A）
        /// </summary>
        public static bool IsInverseRatio(this SLR a, SLR b)
        {
            return a.point1.Equals(b.point3) &&
                   a.point2.Equals(b.point2) &&
                   a.point3.Equals(b.point1);
        }

        /// <summary>
        /// 生成一个完全倒过来的 SLR (用于除法)
        /// </summary>
        public static SLR GetInverse(this SLR slr)
        {
            // 构造倒数 SLR，点的顺序反转，代数值取倒数
            return new SLR(slr.point3, slr.point2, slr.point1, slr.Expr.Invert());
        }
    }

    public static class SREEToEquationExtensions
    {
        /// <summary>
        /// 将 SREE (几何比例知识) 转换为原生的 GeoEquation (代数等式)
        /// </summary>
        public static GeoEquation ToGeoEquation(this SREE sree)
        {
            // 1. 构建代数表达式的左半边：一个大的乘除法节点
            var leftPart = new ProductNode();

            // 2. 遍历所有的 SLR 因子
            foreach (var prop in sree.Properties.OfType<SLR>())
            {
                // 利用你提供的 Segment 类构造线段 (Segment 内部自带 Normalize 排序)
                var segNum = new Segment(prop.point1, prop.point2);
                var segDen = new Segment(prop.point2, prop.point3);

                // 将线段的 Length 属性 (GeoProp) 包装为代数引擎的变量节点 (MutNode)
                var mutNum = new MutNode(segNum.Length);
                var mutDen = new MutNode(segDen.Length);

                // 分子放入乘法区，分母放入除法区
                leftPart.Multipliers.Add(mutNum);
                leftPart.Divisors.Add(mutDen);
            }

            // 3. 构建等式 (左右两边都经过系统的自带化简)
            Expr rightPart = sree.Expr.Clone();
            var equation = new GeoEquation(leftPart.Simplify(), rightPart.Simplify());
            
            return equation;
        }
    }
}
