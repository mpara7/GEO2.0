using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.MutNodes
{
    public class NameNode : Expr
    {
        public string Name { get; set; }
        public NameNode(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
        public override Expr Clone()
        {
            return this;
        }
        public override Expr Simplify()
        {
            return this;
        }
    }
}
