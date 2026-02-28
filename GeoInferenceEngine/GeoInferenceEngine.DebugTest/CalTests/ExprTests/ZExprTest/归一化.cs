using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.DebugTest.DebugAsserts.Exprs
{
    internal class 归一化
    {
        static Expr a = Expr.FromString("a"), b = Expr.FromString("b"), c = Expr.FromString("c"), d = Expr.FromString("d");
        static Segment ab = new Segment(new Point("A"), new Point("B"));
        static Segment cd = new Segment(new Point("C"), new Point("D"));
        static Expr expr = null, expr2 = null, expr3 = null, expr4 = null, result = null;
        public static void 不同节点类型比较()
        {
            //具体查看ExprOrderComparer中的typeOrder
            result = a.Mul(10).Mul(Expr.Pi).Mul(ab.Length);
            //result.Normalize();
            Debug.Assert(result.ToString() == "(10*Pi*a*AB的长度)");

            result = Expr.Pi.Mul(a).Mul(ab.Length).Mul(10);
            //result.Normalize();
            Debug.Assert(result.ToString() == "(10*Pi*a*AB的长度)");
        }
        public static void 相同类型节点比较()
        {
            //除了Mutable节点按出现先后排序
            //其他暂时按字符串HashCode排序
            result = a.Mul(b).Mul(c).Mul(d);
            //result.Normalize();
            Debug.Assert(result.ToString() == "(a*b*c*d)");

            result = d.Mul(b).Mul(a).Mul(c);
            //result.Normalize();
            Debug.Assert(result.ToString() == "(a*b*c*d)");
        }
        public static void Run()
        {
            Knowledge.InitClassIndex(typeof(Point).Assembly);
            不同节点类型比较();
            相同类型节点比较();
        }
    }
}
