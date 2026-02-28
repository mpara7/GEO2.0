using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Configs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

using System.Diagnostics;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.EquivalencePlaneGeometry;

namespace GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
internal class Equation模拟输入
{
    public static void Run()
    {
        //等式输入();
        //复杂等式输入();
        //复杂等式输入2();
        一道例题_在平面几何的应用();
    }
    public static void 等式输入()
    {
        string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AB=2*CD
            BC=2*CD
            Prove:AB=BC 
            """;
        EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        
    }
    public static void 复杂等式输入()
    {
        var watch= Stopwatch.StartNew();
        string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AB=2*CD
            BC=2*CD
            线段长度相等 EF,AB
            DE+AB+EF=3
            Prove:EF=BC 
            """;
        EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
    }
    public static void 复杂等式输入2()
    {
        var watch = Stopwatch.StartNew();
        string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AB=2*DE
            AB=2*BC
            BC=2*EF
            DE+AB+EF=3
            Prove:EF=3/7 
            """;
        EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
    }
    public static void 一道例题_在平面几何的应用()
    {
        var watch = Stopwatch.StartNew();
        string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AD/CD=2/3
            AE=EB
            //解析有问题(AE*BF)与(EB*FD) AD/DC
            //((AE*BF)/(EB*FD))=AD/DC+1
            (EB*FD)/(AE*BF)=DC/AD+1
            Prove:BF/FD=5/3
            """;
        EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
    }
}
