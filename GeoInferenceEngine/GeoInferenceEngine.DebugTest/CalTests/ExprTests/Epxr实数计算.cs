global using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.Knowledges;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.Expr;
namespace GeoInferenceEngine.DebugTest.CalTests.ExprTests
{
    internal class Epxr实数计算
    {
        static Expr expr1, expr2, expr3, result;
        public static void Run()
        {
            ExprPreparer.Init();
            整形计算();
            有理数计算();
            无理数加减混合计算();
            无理数乘除混合计算();
            无理数次方混合计算();
            分数化简();
            通分();
            复合计算();
        }
        public static void 整形计算()
        {
            //加减乘除
            result = One.Add(Two);
            Debug.Assert(result.StrEqual("3"));
            result = One.Sub(Two);
            Debug.Assert(result.StrEqual("-1"));
            result = Two.Mul(Two);
            Debug.Assert(result.StrEqual("4"));
            result = Two.Div(Two).Simplify();
            Debug.Assert(result.StrEqual("1"));
            //次方
            expr1 = 10;
            Debug.Assert(expr1.Clone().Pow(0).StrEqual("1"));
            Debug.Assert(expr1.Clone().Pow(-2).StrEqual("1/100"));
            Debug.Assert(expr1.Clone().Pow(3).StrEqual("1000"));
            //常用
            expr1 = 10;
            Debug.Assert(expr1.Clone().Opposite().StrEqual("-10"));
            Debug.Assert(expr1.Clone().Invert().StrEqual("1/10"));
        }

        public static void 有理数计算()
        {
            //与整数的加减乘除
            expr1 = Let(1).Div(5).Simplify();
            Debug.Assert(expr1.Add(Two).StrEqual("11/5"));

            expr1 = Let(1).Div(5).Simplify();
            Debug.Assert(expr1.Sub(Two).StrEqual("-9/5"));

            expr1 = Let(1).Div(5).Simplify();
            Debug.Assert(expr1.Mul(Two).StrEqual("2/5"));

            expr1 = Let(1).Div(5).Simplify();
            Debug.Assert(expr1.Div(Two).StrEqual("1/10"));

            //与其他有理数的加减乘除
            expr1 = Let(1).Div(3).Simplify();
            expr2 = Let(1).Div(2).Simplify();
            result = expr1.Clone().Add(expr2);
            Debug.Assert(result.StrEqual("5/6"));
            result = expr1.Clone().Sub(expr2);
            Debug.Assert(result.StrEqual("-1/6"));
            result = expr1.Clone().Mul(expr2);
            Debug.Assert(result.StrEqual("1/6"));
            result = expr1.Clone().Div(expr2);
            Debug.Assert(result.StrEqual("2/3"));
            //次方
            result = expr1.Clone().Opposite().Pow(2);
            Debug.Assert(result.StrEqual("1/9"));
            result = expr1.Clone().Opposite().Pow(-3).Simplify();
            Debug.Assert(result.StrEqual("-27"));
            //常用
            result = Let(1).Div(5).Opposite().Simplify();
            Debug.Assert(result.StrEqual("-1/5"));
            result = Let(1).Div(5).Invert().Simplify();
            Debug.Assert(result.StrEqual("5"));
        }

        public static void 无理数加减混合计算()
        {
            result = Pi.Add(Pi).Add(Two);
            result = result.Simplify();
            Debug.Assert(result.StrEqual("(2+2*Pi)"));

            result = Two.Sqrt().Add(Two.Sqrt()).Add(2);
            result = result.Simplify();
            Debug.Assert(result.StrEqual("(2+2*2^(1/2))"));
            result = Two.Sqrt().Add(Three.Sqrt()).Add(Two);
            result = result.Simplify();
            Debug.Assert(result.StrEqual("(2+2^(1/2)+3^(1/2))"));

        }
        public static void 无理数乘除混合计算()
        {
            result = Two.Sqrt().Mul(Two.Sqrt()).Add(Two);
            result = result.Simplify();
            Debug.Assert(result.StrEqual("4"));
            result = Two.Sqrt().Mul(Three.Sqrt());
            result = result.Simplify();
            Debug.Assert(result.StrEqual("(2^(1/2)*3^(1/2))"));

            result = Two.Sqrt().Div(Three.Sqrt());
            result = result.Simplify();
            Debug.Assert(result.StrEqual("1/3*(2^(1/2)*3^(1/2))"));

            result = Three.Mul(Two.Sqrt()).Div(Three.Sqrt());
            result = result.Simplify();
            Debug.Assert(result.StrEqual("(2^(1/2)*3^(1/2))"));
        }

        public static void 无理数次方混合计算()
        {
            result = Two.Sqrt().Div(Three.Sqrt());
            result = result.Pow(Two);
            result = result.Simplify();
            Debug.Assert(result.StrEqual("2/3"));

            result = Two.Sqrt().Mul(Pi);
            result = result.Pow(Two);
            result = result.Simplify();
            Debug.Assert(result.StrEqual("2*Pi^(2)"));

            result = Let(10).Sqrt().Mul(Let(10).Sqrt()).Div(Let(5).Sqrt());
            result = result.Simplify();
            Debug.Assert(result.StrEqual("2*5^(1/2)"));

            //5的三次方
            result = Let(5).Pow(Expr.Let(3)).Simplify();
            Debug.Assert(result.ToString() == "125");
            //分数次方
            result = Let(5).Div(17);
            result = result.Pow(Expr.Let(3));
            result = result.Simplify();
            Debug.Assert(result.ToString() == "125/4913");
        }
        static void 分数化简()
        {
            expr1 = One.Div(10);
            expr2 = Two.Pow(One.Div(Two));
            expr1 = Let(10) / Let(10);
            result = expr1.Simplify();
            Debug.Assert(result.ToString() == "1");
            //分子为0
            expr1 = Let(0);
            expr2 = Let(177);
            result = expr1/ expr2;
            Debug.Assert(result.ToString() == "0");
            //分母为1
            expr1 = Let(10);
            expr2 = Let(1);
            result = expr1/ expr2;
            Debug.Assert(result.ToString() == "10");
            //负数化简
            expr1 = Let(-10);
            expr2 = Let(-90);
            result = expr1 / expr2;
            Debug.Assert(result.ToString() == "1/9");
        }
        static void 通分()
        {
            Expr result = null, mid1 = null, mid2 = null, five = Expr.Let(5), ten = Expr.Let(10), seventeen = Expr.Let(17);
            //5/17+5/17=10/17
            mid1 = five.Clone().Div(seventeen);
            mid2 = five.Clone().Div(seventeen);
            result = mid1.Add(mid2);
            Debug.Assert(result.ToString() == "10/17");
            //1/2+5/17=(17+10)/34=27/34
            mid1 = five.Clone().Div(ten);
            mid2 = five.Clone().Div(seventeen);
            result = mid1.Add(mid2);
            Debug.Assert(result.ToString() == "27/34");
            //5/17-1/2+=(10-17)/34=-7/34
            mid1 = five.Clone().Div(seventeen);
            mid2 = five.Clone().Div(ten);
            result = mid1.Sub(mid2);
            Debug.Assert(result.ToString() == "-7/34");
        }
        static void 复合计算()
        {
            Expr expr=null, result = null, mid1 = null, mid2 = null, five = Expr.Let(5), ten = Expr.Let(10), seventeen = Expr.Let(17);
            //通分 3/10+1=13/10
            expr = 3;
            result = expr.Div(10).Add(Expr.One);
            result = result.Simplify();
            Debug.Assert(result.ToString() == "13/10");
            //通分+约分 2/3-1/6=1/2
            expr = Expr.Let(2).Div(3);
            expr2 = Expr.Let(1).Div(6);
            result = expr.Sub(expr2);
            result = result.Simplify();
            Debug.Assert(result.ToString() == "1/2");
            //次方的次方
            expr = Expr.Let(2);
            result = expr.Pow(2).Pow(2).Simplify();
            Debug.Assert(result.ToString() == "16");
            //分式的次方
            expr = Expr.Let(2).Div(3);
            result = expr.Pow(3).Simplify();
            Debug.Assert(result.ToString() == "8/27");
            //分式的次方 再开根号
            //整数开方
            expr = Expr.Let(216);
            result = expr.Sqrt();
            result = result.Simplify();
            Debug.Assert(result.StrEqual( "6*(2^(1/2)*3^(1/2))"));
            result = result.Mul(Expr.Let(2).Sqrt());
            result = result.Div(Expr.Let(4));
            result = result.Simplify();

            result = result.Simplify();
            Debug.Assert(result.StrEqual("3*3^(1/2)"));
        }
    }
}
