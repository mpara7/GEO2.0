using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.DebugTest.ProblemTests.Problems
{
    internal class 几何宝典
    {
        public static string _103 = """
            Points:A(2.5,3) B(0,0) C(3,0) D(2.5,1.2) E(1.8,2) F(1.5,0) G(2,1.5) H(2.8,1)
            三角形 ABC
            垂足 D,BD,AC
            垂足 E,CE,AB
            中点 F,BC
            垂足 G,FG,ED
            线段 EF
            线段 FD
            Prove:线段长度相等 DG,GE
            """;
        public static string _122 = """
            Points:A(2,3) B(0,0) C(4,0) D(1,1) E(5,-1) F(3,0) G(1.5,0)
            三角形 ABC
            线段长度相等 AB,AC
            点在线上 DAB
            点在线上 CAE
            点在线上 FGC
            点在线上 GBC
            点在线上 FDE
            点在线上 GBF
            线段长度相等 BD,CE
            共线 BGFC
            共线 ACE
            线段平行 DG,AC
            直线的交点 F,DE,BC
            Prove:线段长度相等 DF,EF
            """;
    }
}
