using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class TanNode : TrigonometricNode
    {
        public override string ToString()
        {
            return $"tan({Expr})";
        }
        public override Expr Clone()
        {
            TanNode node = new TanNode();
            node.Expr = Expr.Clone();
            return node;
        }
    }
}
