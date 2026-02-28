using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.Knowledges.Models;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

using static GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests.RatioInfoTestContext;
namespace GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
internal class Equation计算
{
    internal static void Run()
    {
        连加();
        传递性();
        一道例题_在平面几何的应用();
    }
    public static void 连加()
    {
        ResetContext();
        //传递性
        equation1 = new(AB.Length+(CD.Length)+(EF.Length), 4);
        equation2 = new(AB.Length+(CD.Length), 2);
        adder.Add((equation1));
        adder.Add((equation2));
        executor.Do();
        var f = kbase;
        Console.WriteLine(fbase);
    }
    public static void 传递性()
    {
        ResetContext();
        //传递性
        equation1 = new(AB.Length, CD.Length);
        equation2 = new(CD.Length, EF.Length);
        adder.Add((equation1));
        adder.Add((equation2));
        executor.Do();
        var f = kbase;
        Console.WriteLine(fbase);
    }

    static void 连加尝试()
    {
        ResetContext();
        adder.Add(new GeoEquation(a, e));
        adder.Add(new GeoEquation(a+(b), d));
        adder.Add(new GeoEquation(e+(c), d));
        executor.Do();
        Console.WriteLine(fbase);
    }
    static void 连乘尝试()
    {
        ResetContext();
        adder.Add(new GeoEquation(a, e));
        adder.Add(new GeoEquation(a.Mul(b), d));
        adder.Add(new GeoEquation(e.Mul(c), d));
        executor.Do();
        Console.WriteLine(fbase);
    }
    static void 等比值()
    {
        ResetContext();
        adder.Add(new GeoEquation(a.Div(b), a.Div(c)));
        adder.Add(new GeoEquation(d.Div(b), a.Div(c)));
        executor.Do();
        Console.WriteLine(fbase);
    }
    static void 一道例题_在平面几何的应用()
    {
        ResetContext(); 
        
        Segment AE = new Segment(A, E); ka.Add(AE);
        Segment EB = new Segment(E, B); ka.Add(EB);
        Segment AD = new Segment(A, D); ka.Add(AD);
        Segment DC = new Segment(D, C); ka.Add(DC);
        Segment BF = new Segment(B, F); ka.Add(BF);
        Segment FD = new Segment(F, D); ka.Add(FD);
        GeoEquation longEquation = new(AE.Length*(BF.Length)/(EB.Length)/(FD.Length), AD.Length/(DC.Length)+(1));
        GeoEquation ratioEquation = new GeoEquation(AD.Length/(DC.Length), Expr.Let(2).Div(3));
        var k = kbase;
        var f = fbase;
       
        adder.Add(ratioEquation);
        adder.Add(new GeoEquation(AE.Length, EB.Length));
        adder.Add(longEquation);



        executor.Do();
        Console.WriteLine(fbase);
    }
}
