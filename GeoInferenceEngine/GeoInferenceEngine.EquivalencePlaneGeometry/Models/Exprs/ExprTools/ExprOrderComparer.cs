using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.MutNodes;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class ExprOrderComparer : IComparer<Expr>
    {
        static Dictionary<Type, int> typeOrder = new Dictionary<Type, int>() {
            { typeof(IntNode), 0 },
            { typeof(PiNode), 1 },
            { typeof(FractionNode), 2 },
            { typeof(RealSumNode), 3 },
            { typeof(RealProductNode), 4 },
            { typeof(RealPowerNode), 5 },

            { typeof(MutNode), 10 } ,
            { typeof(NameNode), 11 } ,

            //{ typeof(ArcSinNode), 22 } ,
            //{ typeof(ArcCosNode), 23 },
            //{ typeof(ArcTanNode), 24 } ,
            { typeof(SinNode), 25 } ,
            { typeof(CosNode), 26 },
            { typeof(TanNode), 27 } ,

            { typeof(SumNode), 31 },
            { typeof(ProductNode), 32 },
            { typeof(PowerNode), 32 },
        };
        ExprValueComparer ExprValue = new ExprValueComparer();
        public int Compare(Expr? x, Expr? y)
        {
            //if (x is MutableExpr atomx && y is MutableExpr atomy)
            //    return Expr.MutableExprIndexDict[atomx].CompareTo(Expr.MutableExprIndexDict[atomy]);
            var a = typeOrder[x.GetType()];
            var b = typeOrder[y.GetType()];
            if (a != b)
                return a.CompareTo(b);
            else
                // 如果长度相同，按字母顺序排序  
                return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
            return x.ToString().GetHashCode().CompareTo(y.ToString().GetHashCode());
        }
    }
}
