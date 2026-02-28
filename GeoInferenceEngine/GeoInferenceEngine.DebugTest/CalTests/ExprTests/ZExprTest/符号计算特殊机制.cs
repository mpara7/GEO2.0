
using GeoInferenceEngine.Knowledges;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.DebugTest.CalTests.ExprTests.ZExprTest
{
    internal class 符号计算特殊机制
    {
        static Expr a = Expr.FromString("a"), b = Expr.FromString("b"), c = Expr.FromString("c"), d = Expr.FromString("d");
        static Expr expr, expr1, expr2, expr3, expr4, result;
        public static void Run()
        {
            ExprPreparer.Init();
            复合运算();
            分配律();
            通分();
            提取公因式();
        }
        static void 复合运算()
        {
            //n*a+m*a=(n+m)*a
            expr = Expr.FromString("a");
            expr2 = Expr.FromString("a");
            result = expr.Sub(expr2.Mul(10)).Simplify();
            Debug.Assert(result.ToString() == "-9*a");
            //a-a/c=(a*c-a)/c
            expr = Expr.FromString("a");
            expr2 = Expr.FromString("a");
            expr3 = Expr.FromString("c");
            expr4 = expr2.Div(expr3);
            result = expr.Sub(expr4);
            result = result.Simplify();
            Debug.Assert(result == "a*(c-1)/c");
            if (result is SumNode sum1)
            {
                var aa = sum1.通分();
                if (aa.Item1) result = aa.Item2;
            }
            Debug.Assert(result == "a*(c-1)/c");
            //a/d-a/c=(a*c-a*d)/(d*c)
            expr = a.Div(d);
            expr2 = a.Div(c);
            result = expr.Sub(expr2);
            result = result.Simplify();
            if (result is SumNode sum2)
            {
                var aa = sum2.通分();
                if (aa.Item1) result = aa.Item2;
            }
            Debug.Assert(result.ToString() == "((a*c)-(a*d))/(d*c)");
            //a^10+a^10=2*(a^10)
            expr = Expr.FromString("a");
            result = expr.Pow(10).Add(expr.Pow(10));
            result = result.Simplify();
            Debug.Assert(result.ToString() == "2*a^10");
            //a^10+a^9=2*(a^10)
            expr = Expr.FromString("a");
            result = expr.Pow(10).Add(expr.Pow(9));
            result = result.Simplify();
            Debug.Assert(result.ToString() == "(a^10+a^9)");

            expr = a.Add(b).Add(c);
            expr = expr.Mul(d);
            result = expr.Simplify();
            Debug.Assert(result.ToString() == "((a+b+c)*d)" || result.ToString() == "((c+b+a)*d)");
            //a / b + a / b =
            result = a.Div(b).Mul(c).Div(d).Simplify();
            Debug.Assert(result.ToString() == "(a*c)/(b*d)");

            //a/b+a/b=
            expr = a.Div(b).Mul(c);
            expr2 = a.Mul(b).Div(d.Add(a));
            expr2 = expr2.Simplify();
            result = expr.Add(expr2);
            result = result.Simplify();
            Debug.Assert(result.ToString() == "((a*c)/b+(a*b)/(a+d))");
            if (result is SumNode sum3)
            {
                var aa = sum3.通分();
                if (aa.Item1) result = aa.Item2;
            }
            Debug.Assert(result.ToString() == "((a*b^2)+(c*a^2)+(a*c*d))/(b*(a+d))");

            //正负号测试
            expr = a.Mul(b).Mul(-1).Mul(a).Div(b.Mul(-1));
            result = expr.Simplify();
            Debug.Assert(result.ToString() == "a^2");
        }
        static void 分配律()
        {
            expr = a.Mul(b).Mul(c.Add(2));
            expr = expr.Simplify();
            if (expr is ProductNode product)
            {
                var (b, r) = product.分配律();
                if (b)
                {
                    Debug.Assert(r.ToString() == "(2*(a*b)+(a*b*c))");
                }
            }
        }
        private static void 通分()
        {
            expr = a.Mul(b).Add(c.Div(2));
            expr = expr.Simplify();
            if (expr is SumNode sum)
            {
                var (b, r) = sum.通分();
                if (b)
                {
                    Debug.Assert(r.ToString() == "(2*(a*b)+c)/2");
                }
            }
        }
        private static void 提取公因式()
        {
            expr = a.Mul(b).Add(a.Div(2));
            expr = expr.Simplify();
            if (expr is SumNode sum)
            {
                var (b, r) = sum.提取公因式();
                if (b)
                {
                    Debug.Assert(r.ToString() == "(a*(b+1/2))");
                }
            }
        }
    }
}
