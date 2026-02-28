using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.Knowledges.Models;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

using System.Diagnostics;

using static GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests.RatioInfoTestContext;
namespace GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
internal class RatioInfo生成新知识机制
{
    public static void Run()
    {
        已有值连续添加();
        连续添加后加值();
        根据比值合并();
        根据比值合并2();
        根据具体值合并();
        根据具体值合并2();
    }
    public static void 已有值连续添加()
    {
        ResetContext(); 

        var f = fbase;
        adder.UpdateRatioInfo(AB.Length, 2, new SegmentLength(AB, 2));
        adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.One.Div(2), new SegmentLengthRatio(AB,CD,Expr.AHalf));
        adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.One.Div ( 2), new SegmentLengthRatio(CD, EF, Expr.AHalf));
        var k= kbase;
        var knowledges = k.Knowledges.ToList();
        Debug.Assert(knowledges.Exists(k=>k.Value.StrContains("CD的长度是4") ));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF的长度是8")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/4")));
        Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(2=1*AB的长度=1/2*CD的长度=1/4*EF的长度)"));
    }
    public static void 连续添加后加值()
    {
        ResetContext();

        var f = fbase;
        
        adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.One.Div(2), new SegmentLengthEqual(AB, CD));
        adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.One.Div(2), new SegmentLengthEqual(CD, EF));
        adder.UpdateRatioInfo(AB.Length, 2, new SegmentLength(AB, 2));
        var k = kbase;
        var knowledges = k.Knowledges.ToList();
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD的长度是4")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF的长度是8")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/4")));
        Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(2=1*AB的长度=1/2*CD的长度=1/4*EF的长度)"));
    }

    public static void 根据比值合并()
    {
        ResetContext();
        var f = fbase;
        adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
        adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.AHalf, new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

        adder.UpdateRatioInfo(GH.Length, IJ.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));

        adder.UpdateRatioInfo(CD.Length, GH.Length, Expr.AHalf, new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));
        Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(1*AB的长度=1/2*CD的长度=1/4*EF的长度=1/4*GH的长度=1/8*IJ的长度)"));
        var k = kbase;
        var knowledges = k.Knowledges.ToList();
        //注释为不会自动生成
        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与CD的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/4")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与GH的比值是1/4")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与IJ的比值是1/8")));

        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与EF的比值是1/2")));
        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与GH的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与IJ的比值是1/4")));

        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与GH的长度相等")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与IJ的比值是1/2")));

        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("GH与IJ的比值是2")));
    }
    public static void 根据比值合并2()
    {
        ResetContext();
        var f = fbase;
        adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
        adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.AHalf, new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

        adder.UpdateRatioInfo(GH.Length, IJ.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));

        adder.UpdateRatioInfo(GH.Length, CD.Length,  Expr.Two, new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));
        Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(1*AB的长度=1/2*CD的长度=1/4*EF的长度=1/4*GH的长度=1/8*IJ的长度)"));
        var k = kbase;
        var knowledges = k.Knowledges.ToList();
        //注释为不会自动生成
        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与CD的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/4")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与GH的比值是1/4")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与IJ的比值是1/8")));

        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与EF的比值是1/2")));
        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与GH的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与IJ的比值是1/4")));

        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与GH的长度相等")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与IJ的比值是1/2")));

        //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("GH与IJ的比值是2")));
    }

    public static void 根据具体值合并()
    {
        ResetContext();
        var f = fbase;
        adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length));
        adder.UpdateRatioInfo(AB.Length, 90, new GeoEquation(AB.Length, 90));
        adder.UpdateRatioInfo(EF.Length, 180, new GeoEquation(EF.Length, 180));
        //adder.UpdateRatioInfo(AB.Length, EF.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length));
        Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(90=1*AB的长度=1/2*CD的长度=1/2*EF的长度)"));
        var k = kbase;
        var knowledges = k.Knowledges.ToList();
        //注释为不会自动生成
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD的长度是180")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与EF的长度相等")));
    }
    public static void 根据具体值合并2()
    {
        ResetContext();
        var f = fbase;
        adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length));
        adder.UpdateRatioInfo(AB.Length, 90, new GeoEquation(AB.Length, 90));
        adder.UpdateRatioInfo(EF.Length, GH.Length, Expr.AHalf, new GeoEquation(EF.Length, 180));
        adder.UpdateRatioInfo(EF.Length, 180, new GeoEquation(EF.Length, 180));
        Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(90=1*AB的长度=1/2*CD的长度=1/2*EF的长度=1/4*GH的长度)"));

        var k = kbase;
        var knowledges = k.Knowledges.ToList();
        //注释为不会自动生成
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD的长度是180")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("GH的长度是360")));
        
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与EF的长度相等")));

        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF的长度是180")));

        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与GH的比值是1/4")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD与GH的比值是1/2")));
        Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与GH的比值是1/2")));
    }
}
