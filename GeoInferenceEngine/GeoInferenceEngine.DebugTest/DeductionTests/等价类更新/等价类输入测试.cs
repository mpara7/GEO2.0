using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.EquivalencePlaneGeometry;
using GeoInferenceEngine.EquivalencePlaneGeometry.Configs;

using System.Diagnostics;

namespace GeoInferenceEngine.XUnitTest
{
    public class 等价类输入测试
    {
        public static void Run()
        {

        }
        public void 角()
        {
            var zscript = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
角 ABC
角 DE,B,CF
""";
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public void 垂直平分线()
        {
            var zscript = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
垂直平分线 AC,DEF
""";
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public void 角平分线()
        {
            var zscript = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
角平分线 ACB,EF
""";
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public void 圆()
        {
            var zscript = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
圆 ABCD
共圆 BCDE
""";
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
    }
}