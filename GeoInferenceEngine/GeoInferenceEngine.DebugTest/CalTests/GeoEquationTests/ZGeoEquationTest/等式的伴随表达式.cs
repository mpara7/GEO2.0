

using GeoInferenceEngine.Knowledges.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests.RatioInfoTestContext;
namespace GeoInferenceEngine.DebugTest.DebugAsserts.Cal;
internal class 等式的伴随表达式
{
    public static void Run()
    {
        连加();
        等比();
        //其余待补充
    }
    public static void 连加()
    {
        ResetContext();
        GeoEquation equation = new(a.Add(b).Add(c), 180);
        //GeoEquationInfo info = new( equation);
       // Debug.Assert(info.CoExpr.StrEqual("(-180+a+b+c)"));
    }
    public static void 等比()
    {
        ResetContext();
        //GeoEquation equation = new(a.Div(b), c.Div(d));
        //GeoEquationInfo info = new( equation);
        //Debug.Assert(info.CoExpr.StrEqual("((a*d)-(b*c))")) ;
    }
}
