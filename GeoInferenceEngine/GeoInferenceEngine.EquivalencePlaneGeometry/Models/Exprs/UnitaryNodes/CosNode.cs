using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class CosNode : TrigonometricNode
    {
        public override string ToString()
        {
            return $"cos({Expr})";
        }
        public override Expr Clone()
        {
            CosNode node = new CosNode();
            node.Expr = Expr.Clone();
            return node;
        }
    }
}
