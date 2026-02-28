using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.DebugTest.ProblemTests.Problems;
using GeoInferenceEngine.EquivalencePlaneGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.DebugTest.CalTests.RatioInfoTests
{
    internal class RatioInfo模拟输入
    {
        public static void Run()
        {
            连续加入();
            根据值合并();
            根据值合并2();
            根据比值合并();
        }
        public static void 连续加入()
        {
            string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AD/CD=2/3
            CD=2*BC
            2*BC=EB
            Prove:AD/EB=2/3
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void 根据值合并()
        {
            string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AD/CD=2/3
            CD=3

            6=2*BC
            3*BC=EB
            Prove:AD=2
            Prove:AD/EB=2/9
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void 根据值合并2()
        {
            string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)
            AD/CD=2/3
            
            6=2*BC
            3*BC=EB

            CD=3
            Prove:AD=2
            Prove:AD/EB=2/9
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void 根据比值合并()
        {
            string zscript = """
            Points:A(0,0) B(0,1) C(0,2) D(0,3) E(0,4) F(0,5)

            AD/CD=2/3
            CD=EF

            3=BC
            9*BC=EB

            BC=3*EF

            Prove:AD=2/3
            Prove:AD/EB=2/81
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void _139()
        {
            string zscript = """
            Point:A B C D E F O P 
            角EOA=角EFC+1/2*角DCB 
            角EOA=角EDC+1/2*角DEB 
            角EPC=角EFC+1/2*角DEB 
            角EPC=角EBC+1/2*角DCB 
            //角EFC+1/2*角DCB=角EDC+1/2*角DEB 
            //角EBC+1/2*角DCB=角EFC+1/2*角DEB
            //((1/2角BED的大小)+角CDE的大小-(角BCD的大小1/2)-角CFE的大小)=0
            //((1/2角BED的大小)+角CFE的大小-(角BCD的大小1/2)-角CBE的大小)=0
            Prove:2*角EFC=角EBC+角EDC
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void _163()
        {
            string zscript = """
            Points:A(1,0) B(1,2) C(1,3) D(1,4) E(1,5) F(1,6) G(1,7) H(1,8) M(1,9)
            
            角ABE=角CBE
            角CDE=角ADE
            角BAD=45
            角BCD=45
            //角BAD+角ABE=角BED+角ADE
            //角BCD+角CDE=角BED+角CBE
            角BAD+角ABE+角BCD+角CDE=角BED+角ADE+角BED+角CBE
            //角的大小 BAD,45
            //角的大小 BCD,45
            //Prove:角的大小 BED,45
            Prove:角BED=45
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void _4dot2()
        {
            string zscript = """
            //Points:A(1,0) B(1,2) C(1,3) D(1,4) E(1,5) F(1,6) G(1,7) H(1,8) M(1,9)
            Points:A(1,0) B(1,2) C(1,3) D(1,4) A1(1,5) B1(1,6) C1(1,7) D1(1,13) A2(1,8) B2(1,9) C2(1,10) P(1,11)
            //(AB/BD)*(B2D/A2B2)*(A2P/PA)=1
            //(AC/CD)*(C2D/C2A2)*(A2P/PA)=1
            (AB/BD)*(B2D/A2B2)*(A2P/PA)=(AC/CD)*(C2D/C2A2)*(A2P/PA)
            Prove:(AB*CD)/(AC*BD)=(A2B2*C2D)/(A2C2*B2D)
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
        public static void 在平面几何应用()
        {
            string zscript = """
            Points:A(1,0) B(1,2) C(1,3) D(1,4) E(1,5) F(1,6) 
            AE=EB
            AD/DC=2/3
            (AE*BF)/(EB*FD)=AD/DC+1
            三角形ABC的面积=40
            三角形CDF的面积=3/8*三角形BCD的面积
            三角形BCD的面积=3/5*三角形ABC的面积

            Prove:三角形CDF的面积=9/40
            """;
            EPGApp.RunByZScriptWithResult(zscript, EngineConfig.FormFile("ProblemTests/Configs/题目求解.zec"));
        }
    }
}
