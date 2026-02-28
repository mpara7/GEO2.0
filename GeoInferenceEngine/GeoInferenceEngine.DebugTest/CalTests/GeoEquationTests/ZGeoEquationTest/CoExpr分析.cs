//using GeoInferenceEngine.EquivalencePlaneGeometry.Engine.Components.Core.Cal.Models;
//using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using static GeoInferenceEngine.DebugTest.DebugAsserts.Cal.CalContext;
//namespace GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
//internal class CoExpr分析
//{
//    static ACoExprResult analysisResult = null;
//    public static void Run()
//    {
//        ValueCoExprResult();
//        EqualCoExprResult();
//        RatioCoExprResult();
//        AdditionCoExprResult();
//        MulCoExprResult();
//        ComplexCoExprResult();
//    }
//    static void ValueCoExprResult()
//    {
//        ResetContext();
        
//        expr = a.Add(1).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is ValueCoExprResult);

//        expr = a.Sub(1).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is ValueCoExprResult);
//    }
//    static void EqualCoExprResult()
//    {
//        ResetContext();

//        expr = a.Sub(b).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is EqualCoExprResult);

//        expr = a.Mul(3).Sub(b.Mul(3)).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is EqualCoExprResult);
//    }
//    static void RatioCoExprResult()
//    {
//        ResetContext();

//        expr = a.Add(b).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is RatioCoExprResult r1 && r1.Ratio.ToString() == "-1");

//        expr = a.Mul(2).Sub(b.Mul(3)).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is RatioCoExprResult r&&r.Ratio.ToString()=="3/2");

//        expr = a.Add(b.Mul(10)).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is RatioCoExprResult r3 && r3.Ratio.ToString() == "-10");
//    }
//    static void AdditionCoExprResult()
//    {
//        ResetContext();

//        expr = a.Add(b).Sub(3).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is AdditionCoExprResult);

//    }
//    static void MulCoExprResult()
//    {
//        ResetContext();

//        expr = a.Mul(b).Sub(3).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is MultiplicationCoExprResult);

//        expr = a.Mul(b).Sub(c.Mul(d)).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is MultiplicationCoExprResult);

//        expr = a.Sub(c.Mul(d)).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is MultiplicationCoExprResult);

//        expr = a.Mul(b).Sub(d).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is MultiplicationCoExprResult);

//    }
//    static void ComplexCoExprResult()
//    {
//        expr = a.Mul(b).Sub(c.Mul(d)).Sub(3).Simplify();
//        analysisResult = CoExprTool.AnalysisCoExpr(expr);
//        Debug.Assert(analysisResult is ComplexCoExprResult);
//    }
//}
