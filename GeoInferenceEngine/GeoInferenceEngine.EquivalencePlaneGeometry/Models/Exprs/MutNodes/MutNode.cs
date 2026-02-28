using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class MutNode : Expr
    {
        public static Dictionary<Mut, MutNode> CacheMutNodes { get; set; } = new Dictionary<Mut, MutNode>();

        // <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="i"></param>
        public static implicit operator MutNode(Mut mut)
        {
            return FromMut(mut);
        }

        public Mut Mut { get; set; }
        public MutNode(Mut mut)
        {
            Mut= mut;
        }
        public override Expr Clone()
        {
            return this;
        }
        public override Expr Simplify()
        {
            return this;
        }

        public override string ToString()
        {
            return Mut.ToString();
        }
    }
}
