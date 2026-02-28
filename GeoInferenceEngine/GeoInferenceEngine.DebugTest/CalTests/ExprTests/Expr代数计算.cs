
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.DebugTest.CalTests.ExprTests;

namespace GeoInferenceEngine.DebugTest.DebugAsserts.Exprs;
internal class Expr代数计算
{
    static Expr a = Expr.FromString("a"), b = Expr.FromString("b"), c = Expr.FromString("c"), d = Expr.FromString("d");
    static Expr expr = null, expr2 = null, expr3 = null, expr4 = null, result = null;
    public static void Run()
    {
        ExprPreparer.Init();
        加减乘除与次方与相反数与倒数();
        化简机制();
    }


    static void 加减乘除与次方与相反数与倒数()
    {
        //加法
        result = a.Clone().Add(b).Simplify();
        Debug.Assert(result.StrEqual( "(a+b)"));
        //加法系数合并
        result = a.Clone().Add(a).Simplify();
        Debug.Assert(result.StrEqual("2*a"));
        //减法 
        result = a.Clone().Sub(a).Simplify();
        Debug.Assert(result.StrEqual("0"));
        //减法 系数相减
        result = a.Clone().Mul(2).Sub(a).Simplify();
        result = result.Simplify();
        Debug.Assert(result.StrEqual("a"));
        //减法
        result = a.Clone().Sub(b).Simplify();
        Debug.Assert(result.StrEqual("(a-b)"));
        //乘法
        result = a.Clone().Mul(a).Simplify();
        Debug.Assert(result.StrEqual("a^2"));
        //乘法
        result = a.Clone().Mul(b).Simplify();
        Debug.Assert(result.StrEqual("(a*b)")|| result.StrEqual("(b*a)"));
        //除法
        result = a.Clone().Div(a).Simplify();
        Debug.Assert(result.StrEqual("1"));
        //除法
        result = a.Clone().Div(b).Simplify();
        Debug.Assert(result.StrEqual("a/b"));
        //指数
        result = a.Clone().Pow(3).Simplify();
        Debug.Assert(result.StrEqual("a^3"));
        //指数 开根号
        result = a.Clone().Pow(3).Sqrt();
        result = result.Simplify();
        Debug.Assert(result.StrEqual("a^(3/2)"));
        //指数 开根号
        result = a.Clone().Pow(2).Sqrt();
        result = result.Simplify();
        Debug.Assert(result.StrEqual("a"));
        //指数 开根号
        result = a.Clone().Pow(3).Sqrt().Opposite();
        result = result.Simplify();
        Debug.Assert(result.StrEqual("-a^(3/2)"));
        result = a.Clone().Pow(3).Sqrt().Invert();
        result = result.Simplify();
        Debug.Assert(result.StrEqual("1/a^(3/2)"));
    }
    static void 化简机制()
    {
        //和节点到积节点
        expr = Expr.Let(2).Add(a.Clone().Mul(2)).Add(3);
        result = expr.Sub(5).Simplify();
        Debug.Assert(result.ToString() == "2*a");
        //和节点常数合并
        result = Expr.Let(2).Add(a).Add(3).Simplify();
        Debug.Assert(result.StrEqual("(5+a)"));
        //延迟化简
        expr = Expr.Let(2).Add(a).Add(3);
        expr2 = Expr.Let(2).Add(a.Clone().Mul(2)).Add(3);
        expr3 = Expr.Let(5).Add(a.Clone().Mul(2));
        result = expr.Sub(expr2);
        result = result.Sub(expr3);
        //组合完了再化简
        result = result.Simplify();
        Debug.Assert(result.StrEqual("(-5-3*a)"));
        //积节点化简
        expr = a.Clone().Mul(b);
        expr2 = b.Clone().Mul(a);
        result = expr.Add(expr2);
        result = result.Simplify();
        Debug.Assert(result.StrEqual("2*(a*b)"));
        //(a+b+c)*(-a*c)-b
        expr = a.Clone().Add(b).Add(c).Mul(c.Clone().Opposite().Mul(a)).Add(b.Clone().Opposite());
        result = expr.Simplify();
        Debug.Assert(result.StrEqual("(-(a*c*(a+b+c))-b)"));
        //连加计算
        expr = a.Clone().Add(b).Add(c).Sub(180);
        expr.Sub(b.Clone().Add(c));
        expr.Add(55);
        result = expr.Simplify();
        Debug.Assert(result.StrEqual("(-125+a)"));
        expr = a.Clone().Div(b);
        expr2 = c.Clone().Div(b);
        result = expr.Div(expr2);
        result = result.Simplify();
        Debug.Assert(result.StrEqual("a/c"));
        //积节点化简
        expr = a.Clone().Mul(b).Mul(a).Div(b.Clone().Mul(2));
        result = expr.Div(2).Simplify();
        Debug.Assert(result.StrEqual("1/4*a^2"));

    }
    
}
