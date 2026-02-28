using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.Knowledges.Models;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using GeoInferenceEngine.PredicateShared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests.RatioInfoTestContext;
namespace GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests
{
    internal class RatioInfo更新机制
    {
        public static void Run()
        {
            已有值连续添加();
            连续添加后加值();
            根据比值合并_头对头();
            根据比值合并_尾对尾();
            根据比值合并_头对尾();
            根据具体值合并_头对头();
            根据具体值合并_尾对尾();
            根据具体值合并_头对尾();
        }
        public static SegmentLengthRatio AB2CD = new SegmentLengthRatio(AB, CD, Expr.AHalf);
        public static SegmentLengthRatio CD2EF = new SegmentLengthRatio(CD, EF, Expr.Let(1).Div(3));
        public static SegmentLengthRatio EF2GH = new SegmentLengthRatio(EF, GH, Expr.Let(1).Div(5));

        public static SegmentLengthRatio WV2XY = new SegmentLengthRatio(WX, XY, Expr.Let(1).Div(2));
        public static SegmentLengthRatio XY2YZ = new SegmentLengthRatio(WX, XY, Expr.Let(1).Div(3));
        static FormularBase f = fbase;
        static KnowledgeBase k = kbase;
        public static void 已有值连续添加()
        {
            ResetContext();
            adder.UpdateRatioInfo(AB.Length, 2,new SegmentLength(AB, 2));
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf,AB2CD);
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3),CD2EF);
            var knowledges = k.Knowledges.ToList();
            //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD的长度是4")));
            //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF的长度是12")));
            //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/6")));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(2=1*AB的长度=1/2*CD的长度=1/6*EF的长度)"));
        }

        public static void 连续添加后加值()
        {
            ResetContext();
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.One.Div(2), new SegmentLengthEqual(AB, CD));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new SegmentLengthEqual(CD, EF));
            adder.UpdateRatioInfo(AB.Length, 2, new SegmentLength(AB, 2));
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("CD的长度是4")));
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF的长度是12")));
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("AB与EF的比值是1/6")));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(2=1*AB的长度=1/2*CD的长度=1/6*EF的长度)"));
        }

        public static void 根据比值合并_头对头()
        {
            ResetContext();
            var f = fbase;
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(WX.Length, XY.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));
            adder.UpdateRatioInfo(XY.Length, YZ.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(AB.Length, WX.Length, Expr.Let(5), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(1*AB的长度=1/2*CD的长度=1/6*EF的长度=5*WX的长度=5/2*XY的长度=5/6*YZ的长度)"));
            var k = kbase;
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与YZ的比值是5")));
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与WX的比值是30")));
        }
        public static void 根据比值合并_尾对尾()
        {
            ResetContext();
            var f = fbase;
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(WX.Length, XY.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));
            adder.UpdateRatioInfo(XY.Length, YZ.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(EF.Length, YZ.Length, Expr.Let(5), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(1*AB的长度=1/2*CD的长度=1/6*EF的长度=5*WX的长度=5/2*XY的长度=5/6*YZ的长度)"));
            var k = kbase;
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与YZ的比值是5")));
            //Debug.Assert(knowledges.Exists(k => k.Value.StrContains("GH与IJ的比值是2")));
        }
        public static void 根据比值合并_头对尾()
        {
            ResetContext();
            var f = fbase;
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(WX.Length, XY.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));
            adder.UpdateRatioInfo(XY.Length, YZ.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(AB.Length, YZ.Length, Expr.Let(5).Div(6), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(1*AB的长度=1/2*CD的长度=1/6*EF的长度=5*WX的长度=5/2*XY的长度=5/6*YZ的长度)"));
            var k = kbase;
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与YZ的比值是5")));
        }

        public static void 根据具体值合并_头对头()
        {
            ResetContext();
            var f = fbase;
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(WX.Length, XY.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));
            adder.UpdateRatioInfo(XY.Length, YZ.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(AB.Length, Expr.Let(10), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(WX.Length, Expr.Let(2), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(10=1*AB的长度=1/2*CD的长度=1/6*EF的长度=5*WX的长度=5/2*XY的长度=5/6*YZ的长度)"));
            var k = kbase;
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("YZ的长度是12")));
        }
        public static void 根据具体值合并_头对尾()
        {
            ResetContext();
            var f = fbase;
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(WX.Length, XY.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));
            adder.UpdateRatioInfo(XY.Length, YZ.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(AB.Length, Expr.Let(10), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(YZ.Length, Expr.Let(12), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(10=1*AB的长度=1/2*CD的长度=1/6*EF的长度=5*WX的长度=5/2*XY的长度=5/6*YZ的长度)"));
            var k = kbase;
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与YZ的比值是5")));
        }
        public static void 根据具体值合并_尾对尾()
        {
            ResetContext();
            var f = fbase;
            adder.UpdateRatioInfo(AB.Length, CD.Length, Expr.AHalf, new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(CD.Length, EF.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, EF.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(WX.Length, XY.Length, Expr.AHalf, new GeoEquation(GH.Length, IJ.Length*(Expr.Three.Div(2))));
            adder.UpdateRatioInfo(XY.Length, YZ.Length, Expr.Let(1).Div(3), new GeoEquation(CD.Length, GH.Length*(Expr.AHalf)));

            adder.UpdateRatioInfo(EF.Length, Expr.Let(60), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            adder.UpdateRatioInfo(YZ.Length, Expr.Let(12), new GeoEquation(AB.Length, CD.Length*(Expr.AHalf)));
            Debug.Assert(f.DistanceRatioInfos.Exists(r => r.ToString() == "(10=1*AB的长度=1/2*CD的长度=1/6*EF的长度=5*WX的长度=5/2*XY的长度=5/6*YZ的长度)"));
            var k = kbase;
            var knowledges = k.Knowledges.ToList();
            Debug.Assert(knowledges.Exists(k => k.Value.StrContains("EF与YZ的比值是5")));
        }
    }
}
