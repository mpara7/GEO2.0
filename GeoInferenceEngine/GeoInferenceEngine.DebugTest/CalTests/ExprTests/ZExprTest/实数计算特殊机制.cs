
using GeoInferenceEngine.Knowledges;
using System.Diagnostics;

namespace GeoInferenceEngine.DebugTest.新表达式;
internal class 实数计算特殊机制
{
    public bool 还行吧 { get; set; } = true;
    static Expr expr = null, expr2 = null, expr3 = null, expr4 = null;
    static Expr result = null, mid1 = null, mid2 = null, five = Expr.Let(5), ten = Expr.Let(10), seventeen = Expr.Let(17);
    public static void Run()
    {
        分数化简();
        缓存机制();
    }
    static void 分数化简()
    {
        RationalNode fractionNode = new RationalNode();
        //分子分母相同
        fractionNode.Numerator = Expr.Let(10);
        fractionNode.Denominator = Expr.Let(10);
        result = fractionNode.Simplify();
        Debug.Assert(result.ToString() == "1");
        //分子为0
        fractionNode.Numerator = Expr.Let(0);
        fractionNode.Denominator = Expr.Let(177);
        result = fractionNode.Simplify();
        Debug.Assert(result.ToString() == "0");
        //分母为1
        fractionNode.Numerator = Expr.Let(10);
        fractionNode.Denominator = Expr.Let(1);
        result = fractionNode.Simplify();
        Debug.Assert(result.ToString() == "10");
        //负数化简
        fractionNode.Numerator = Expr.Let(-10);
        fractionNode.Denominator = Expr.Let(-90);
        result = fractionNode.Simplify();
        Debug.Assert(result.ToString() == "1/9");
    }
    
    static void 缓存机制()
    {
        //整数会缓存
        result = five.Add(five);
        Debug.Assert(result == ten);
        //分数不会缓存
        mid1 = five.Div(seventeen);
        mid2 = five.Div(seventeen);
        Debug.Assert(mid1 != mid2);
    }
    
}
