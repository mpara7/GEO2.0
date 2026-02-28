using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class ArcSinNode : TrigonometricNode
    {
        public override string ToString()
        {
            return $"arcsin({Expr})";
        }
        public override Expr Clone()
        {
            ArcSinNode node = new ArcSinNode();
            node.Expr = Expr.Clone();
            return node;
        }
    }
}
