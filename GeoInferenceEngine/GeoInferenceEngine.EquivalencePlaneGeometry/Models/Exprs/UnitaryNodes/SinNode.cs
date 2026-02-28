using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class SinNode : TrigonometricNode
    {
        public override string ToString()
        {
            return $"sin({Expr})";
        }
        public override Expr Clone()
        {
            SinNode node = new SinNode();
            node.Expr = Expr.Clone();
            return node;
        }
    }
}
