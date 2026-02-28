using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Knowledges.Models;
using System.Diagnostics;

using ZTool.Infrastructures.ZDI;

using static GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests.RatioInfoTestContext;
namespace GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
internal class Equation求解策略生成
{
    public static void Run()
    {
        连加();
        连乘();
    }
    public static void 连加()
    {
        ResetContext();
        adder.Add(new(a.Add(b).Add(c), 180));
        adder.Add(new(a.Add(b), 90));
        //var ss= strategyMaker.MakeStrategies();
    }
    public static void 连乘()
    {
        ResetContext();
        GeoEquation equation = new(a.Div(b), c.Div(d));
        adder.Add(equation);
        adder.Add(new(c.Div(d), e.Div(f)));
        //var ss = strategyMaker.MakeStrategies();
        //foreach (var s in ss)
        //{
        //    s.Do();
        //    GeoEquationInfo equationInfo = new GeoEquationInfo(s.NewEquation);
        //}
    }
}
