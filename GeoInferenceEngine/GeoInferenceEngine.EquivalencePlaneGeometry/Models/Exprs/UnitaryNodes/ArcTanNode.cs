using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class ArcTanNode : TrigonometricNode
    {
        public override string ToString()
        {
            return $"arctan({Expr})";
        }
        public override ArcTanNode Clone()
        {
            ArcTanNode node = new ArcTanNode();
            node.Expr = Expr.Clone();
            return node;
        }
    }
}
