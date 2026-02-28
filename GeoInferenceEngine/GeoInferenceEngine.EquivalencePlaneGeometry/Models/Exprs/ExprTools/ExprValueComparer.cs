using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class ExprValueComparer : IEqualityComparer<Expr>
    {
        public bool Equals(Expr? x, Expr? y)
        {
            if (ReferenceEquals(x, y)) return true;
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(Expr obj)
        {
            if (obj == null) return 0;
            var code = obj.ToString().GetHashCode();
            return code;
        }
    }
}
