using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine
{
    internal class 直线平行垂直测试题目
    {
        public static string 直线平行传递性 = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
直线平行 AB,CD
直线平行 CD,EF
Prove:直线平行 AB,EF
""";
        public static string 直线垂直传递性 = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
直线平行 AB,CD
直线垂直 CD,EF
Prove:直线垂直 AB,EF
""";
        public static string 直线平行垂直传递性 = """
Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
直线平行 AB,CD
直线垂直 CD,EF
直线平行 EF,GH
Prove:直线垂直 AB,GH
""";
    }
    
    internal class 共线更新
    {
        /// <summary>
        ///   D
        /// A C B
        /// </summary>
        public static string 平行且有公共点判定共线 = """
Points:A(0,0) B(0,2) C(0,1) D(1,1)
直线垂直 AC,CD
直线垂直 CD,CB
""";
    }

    internal class 几何图形
    {
        public static string 简单输入 = """
Points:A(0,0) B(1,0) C(1,1) D(2,1) E(2,3) F(3,3)
三角形 ABC
四边形 CDEF
四边形 ACFB
四边形 BCED
""";
        public static string 特殊三角形 = """
            Points:A(0,1) B(0,2) C(1,1) D(1,2) E(2,1) F(2,2)
            直角三角形 ABC
            等腰三角形 ABD
            等边三角形 BCF
            """;
        public static string 特殊四边形 = """
            Points:A(0,0) B(2,0) C(1.8,2) D(0.2,2) E(1.8,4) F(0.2,4) G(0.1,2.1) H(2.2,2.1) I(1.8,0)
            等腰梯形 ABCD
            正方形 DCEF
            平行四边形 ADFG
            菱形 ECBH
            直角梯形 AICD
            """;
    }
}
