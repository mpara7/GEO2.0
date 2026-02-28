using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.DebugTest.ProblemTests.Problems
{
    internal class 高斯测试
    {
        public static string Any = """
            Points:A(2.5,3) B(0,0) C(3,0) D(2.5,1.2) E(1.8,2) F(1.5,0) G(2,1.5) H(2.8,1)
            AE-BE=BE-CE
            DE-CE=AE-DE
            BE-CE=CE-FE

            Prove:线段长度相等 BE,DE
            """;
    }

    internal class 连加测试
    {
        public static string Any = """
            Points:A(2.5,3) B(0,0) C(3,0) D(2.5,1.2) E(1.8,2) F(1.5,0) G(2,1.5) H(2.8,1)
            AB+CD+EF=90
            AB+EF=45
            Prove:线段长度 CD，45
            """;
    }

    internal class 连乘测试
    {
        public static string Any = """
            Points:A(2.5,3) B(0,0) C(3,0) D(2.5,1.2) E(1.8,2) F(1.5,0) G(2,1.5) H(2.8,1) A2(2.5,1) B2(1,0) C2(1.5,0) P(1.1,1.2)
            (AB/BD)*(B2D/A2B2)*(A2P/PA)=1
            (AC/CD)*(B2D/C2A2)*(A2P/PA)=1
            Prove:(AB/AC)*(CD/BD)*(C2A2/A2B2)=1

            """;
    }

    internal class 风筝测试
    {
        public static string Any = """
            Points:A(2.5,3) B(0,0) C(3,0) D(2.5,1.2) E(1.8,2) F(1.5,0) G(2,1.5) H(2.8,1) A1(2.5,1) B2(1,0) C2(1.5,0) P(1.1,1.2)
            角BEA1=角EAA1+角EA1A
            角A1FC=角FAA1+角FA1A
            角BEA1+角A1FC=110
            角EAA1=角EA1A
            角FAA1=角FA1A
            角BAC=角EAA1+角FAA1

            Prove:角EAF=55

            """;
    }
}