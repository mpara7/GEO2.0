// See https://aka.ms/new-console-template for more information
using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.DebugTest;
using GeoInferenceEngine.DebugTest.CalTests.ExprTests;
using GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests;
using GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
using GeoInferenceEngine.DebugTest.DebugAsserts.Deduction;
using GeoInferenceEngine.DebugTest.DebugAsserts.Exprs;
using GeoInferenceEngine.DebugTest.ProblemTests;
using GeoInferenceEngine.DebugTest.ProblemTests.Problems;
using GeoInferenceEngine.DebugTest.产生式推理;
using GeoInferenceEngine.EquivalencePlaneGeometry;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.Knowledges.Imps.Componments;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using GeoInferenceEngine.XUnitTest;
using GeoProblemDatabaseDLL;
using System.Net.WebSockets;

if (true)
{
    if (false)
    {
        Epxr实数计算.Run();
        Expr代数计算.Run();
        Expr比较.Run();
    }
    if (true)
    {
        RatioInfo更新机制.Run();
        RatioInfo生成新知识机制.Run();
        RatioInfo模拟输入.Run();
    }
    if (false)
    {
        Equation求解策略生成.Run();
        Equation计算.Run();
        Equation模拟输入.Run();
    }
}
if (false)
{
    点构造性示例.Run();
    等价类输入测试.Run();
    if (false)
    {
        几何关系判定.Run();
        几何关系性质.Run();
        特殊图形判定.Run();
        特殊图形性质.Run();
    }
}
if (false)
{
    Problem几何宝典.Run();
}
//<<<<<<< Updated upstream
//if (false)
//{
//    var c= GeoProblemDatabaseQApp.QApp["几何模型"]["1.1.1"][0];
//    Problem几何宝典.Run();
//}
//GlobalDatabaseSetting.DatabaseSource = DatabaseSource.Remote;
//var q = GeoProblemDatabaseQApp.QApp["GeoDict"]["103"][0];
//EPGApp.RunByZScriptWithResult(q, GlobalEngineConfigs.Load("默认解题"));
//=======

EPGApp.RunByZScriptWithResult(风筝测试.Any, GlobalEngineConfigs.Load("默认解题"));