using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.Knowledges;
using System.Diagnostics;
namespace GeoInferenceEngine.DebugTest.DebugAsserts.Exprs
{
    internal class Expr比较
    {
        static Expr a = Expr.FromString("a"), b = Expr.FromString("b"), c = Expr.FromString("c"), d = Expr.FromString("d");
        static Expr expr = null, expr2 = null, expr3 = null, expr4 = null;
        static ExprCompareResult result = ExprCompareResult.Unknow;
        public static void Run()
        {
            //需要继续在Expr.CompareTo函数中完善比较逻辑
            有理数直接比较();
            和节点常数比较();
            积节点系数比较();
            指数比较();
        }
        public static void 有理数直接比较()
        {
            //实数比较 直接算近似值 使用浮点数比较
            result = Expr.Let(3).Div(2).CompareTo(Expr.Let(2).Div(3));
            Debug.Assert(result == ExprCompareResult.Greater);

            result = Expr.Let(1).Div(2).CompareTo(Expr.Let(8).Div(17));
            Debug.Assert(result == ExprCompareResult.Greater);

            result = Expr.Let(9).Pow(3).CompareTo(Expr.Let(8).Pow(2));
            Debug.Assert(result == ExprCompareResult.Greater);
        }
        public static void 和节点常数比较()
        {
            ////如果表达式的加数和减数都相同 则比较常数作为结果、其余情况返回未知
            //expr2 = a.Clone().Add(b).Add(2);
            //expr3 = a.Clone().Add(b).Add(2);
            //result = expr2.CompareTo(expr3);
            //Debug.Assert(result == ExprCompareResult.Equal);

            expr2 = b.Clone().Add(a).Add(2).Simplify();
            expr3 = b.Clone().Add(a).Add(3).Simplify();
            result = expr2.CompareTo(expr3);
            Debug.Assert(result == ExprCompareResult.Unknow);
        }
        public static void 积节点系数比较()
        {
            ////如果表达式的乘数与除数都相同 则比较常数作为结果、其余情况返回未知
            //expr2 = a.Clone().Mul(b).Mul(2);
            //expr3 = a.Clone().Mul(b).Mul(2);
            //result = expr2.CompareTo(expr3);
            //Debug.Assert(result == ExprCompareResult.Equal);

            expr2 = b.Clone().Mul(a).Mul(2).Simplify();
            expr3 = b.Clone().Mul(a).Mul(3).Simplify();
            result = expr2.CompareTo(expr3);
            Debug.Assert(result == ExprCompareResult.Unknow);
        }
        public static void 指数比较()
        {
            ////如果表达式的底数相同 则比较指数作为结果、其余情况返回未知
            //expr2 = a.Clone().Pow(2);
            //expr3 = a.Clone().Pow(2);
            //result = expr2.CompareTo(expr3);
            //Debug.Assert(result == ExprCompareResult.Equal);
            expr2 = a.Clone().Pow(2);
            expr3 = a.Clone().Pow(3);
            result = expr2.CompareTo(expr3);
            Debug.Assert(result == ExprCompareResult.Unknow);
        }
        
    }
}
