using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Predicates.Relations.GeoPropRelations
{
    using System.Linq;
    using System.Collections.Generic;

    public static class SREECombiner
    {
        /// <summary>
        /// 将两个 SREE 相乘或相除，并自动约分掉共同的 SLR
        /// isDivision 为 true 表示 eq1 / eq2
        /// </summary>
        public static SREE CombineAndCancel(SREE eq1, SREE eq2, bool isDivision, FormularBase formularBase)
        {
            var factors = new List<SLR>();

            // 1. 放入 eq1 的所有因子
            foreach (var prop in eq1.Properties)
                factors.Add((SLR)prop);

            // 2. 放入 eq2 的所有因子 (如果是除法，则放入倒数)
            foreach (var prop in eq2.Properties)
            {
                var slr2 = (SLR)prop;
                factors.Add(isDivision ? slr2.GetInverse() : slr2);
            }

            // 3. 核心：贪心约分 (互相抵消互为倒数的 SLR)
            bool cancelled = true;
            while (cancelled)
            {
                cancelled = false;
                for (int i = 0; i < factors.Count; i++)
                {
                    for (int j = i + 1; j < factors.Count; j++)
                    {
                        // 如果存在互为倒数的项，比如 A-B-C 和 C-B-A，就互相抵消
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

            // 4. 代数表达式的乘除化简
            Expr newExpr;
            if (isDivision)
            {
                var product = new ProductNode();
                product.Multipliers.Add(eq1.Expr);
                product.Divisors.Add(eq2.Expr);
                newExpr = product.Simplify();
            }
            else
            {
                var product = new ProductNode();
                product.Multipliers.Add(eq1.Expr);
                product.Multipliers.Add(eq2.Expr);
                newExpr = product.Simplify();
            }

            // 5. 生成新的 SREE
            var newSREE = new SREE(newExpr, factors.ToArray());
            newSREE.AddCondition("贪心消元合并", eq1, eq2);
            return newSREE;
        }
    }
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
}
