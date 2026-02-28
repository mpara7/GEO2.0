using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.DebugTest;
public static class 几何宝典
{
    public static string _100 = """
            Points:A(2,2) B(0,0) C(3,0) D(-1,3) E(1.5,-2) F(4,1.8)
            三角形 ABC
            正三角形 ABD
            正三角形 AFC
            正三角形 EBC
            Prove:线段长度相等 CD,BF
            Prove:线段长度相等 BF,AE
            """;
    public static string _103 = """
            Points:A(2.5,3) B(0,0) C(3,0) D(2.5,1.2) E(1.8,2) F(1.5,0) G(2,1.5) H(2.8,1)
            三角形 ABC
            垂足 D,BD,AC
            垂足 E,CE,AB
            中点 F,BC
            垂足 G,FG,ED
            线段 EF
            线段 FD
            //直角三角形 DBC
            Prove:线段长度相等 DG,GE
            """;
    public static string _121 = """
            Points:A(2,1) B(1,0) C(3,0) D(2.2,3) E(0,2) F(1.5,3.3) G(4,1.8) H(3,2.8) M(2,0) 
            三角形 ABC
            四边形 ABDC
            正方形 ABEF
            正方形 ACGH
            中点 M,AD
            中点 M,BC
            //?
            角FAH=角DCA
            Prove:线段长度相等 AD,FH
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
    public static string _124 = """
            Points:A(3,3) B(0,0) C(6,0) D(3,0) P(4,0) E(2,2) F(5,1)
            等腰直角三角形 ABC
            中点 D,BC
            点在两点之间 PBC
            垂足 E,PE,AB
            垂足 F,PF,AC
            Prove:线段长度相等 ED,DF
            Prove:线段垂直 ED,DF
            """;
    public static string _127 = """
            Points:A(0,0) C(4,0) B(2,0) M(1,0) N(3,0)
            点在线上 B,AC
            中点 M,AB
            中点 N,BC
            Prove:MN=1/2*AC
            
            """;
    public static string _128 = """
            Points:A(0,0) B(0,2) C(2,0) M(11,0) D(2,2) 
            三角形 ABC
            角BAC=90
            角平分线 BAC,AD
            中点 M,BC
            线段垂直 MD,BC
            Prove:线段长度相等 MA,MD
            """;
    public static string _133 = """
            Points:A(0,0) C(4,0) B(2,0) M(3,0) N(1,0)
            点在线上 BAC
            中点 NAC
            中点 MBC
            Prove:MN=1/2*AB
            """;
    public static string _139 = """
            Points:A(0,0) B(-1,-1) C(1,-1) D(-1,2) E(1,1) F(-2,0)
            三角形 ABC
            三角形 ADE
            角的值相等 BAC,DAE
            角平分线 DEA,EF
            角平分线 ABC,FC
            Prove:EFC=1/2*(EBC+CDE)
            """;
    public static string _140 = """
            Points:A(0,2) B(-2,0) C(0,0) D(2,0) H(1,1) E(-1,1)
            三角形 ABC
            角平分线 ACB,E
            共线 BCD
            CD=AC
            直线垂直 CH,AD
            点在线上 HAD
            Prove:直线垂直 CE,CH
            """;
    public static string _143 = """
            Points:A(1.1,1.6) B(0,0) C(2,0) P(0,1) Q(2,3) M(1,0) N(1,1.5)
            三角形 ABC
            点在线上 A,PQ
            中点 M,BC
            中点 N,PQ
            垂足 P,BP,PQ
            垂足 Q,CQ,PQ
            垂足 N,MN,PQ
            Prove:线段长度相等 MP,MQ
            """;
    public static string _151 = """
            Point:A B C D E F G
            三角形 ABC
            三角形 FDC
            四边形 BDFE
            四边形 ADEF
            四边形 AGCF
            中点 D,AB
            中点 E,AC
            中点 G,BC
            点在线上 D,AB
            点在线上 E,AC
            线段平行 DF,BE
            线段平行 EF,AB
            Prove:线段长度相等 CF,AG
            """;
    public static string _155 = """
            Point:A B C F E D 
            三角形 ABC
            角大小 BAC,90
            角平分线 BAC,AF
            点在线上 F,BC
            中点 EBC
            直线垂直 AD,BC
            点在线上 D,BC
            Prove:角的值相等 EAF,FAD
            """;
    public static string _163 = """
            Point:B E  D A C 

            角平分线 ABC,BE
            角平分线 ADC,DE
            角的大小 DAB,45
            角的大小 BCD,55
            Prove:角的大小 BED,50
            """;
    public static string _172 = """
            Point:A B C D E 
            三角形 ABC
            AB=AC
            点在线上 D,BC
            点在线上 E,AD
            //角的大小比例 BED,CED,2
            角的大小相等 BED,BAC
            Prove:BD=2CD
            """;
    public static string _181 = """
            Point:A B C D E F 
            三角形 ABC
            角平分线 AD,BAC
            点在线上 D,BC
            点在线上 E,AB
            AE=AC
            EF//BC
            点在线上 F,AC
            Prove:
            角平分线 EC,DEF
            
            """;
    public static string _194 = """
            Point:A B C D E F M N 
            多边形 ABCD
            中点 EAC
            中点 FBD
            中点 MAB
            中点 NCD
            直线的交点 1,EF,MN
            Prove:
            EI=IF
            NI=IM
            """;
    public static string _195 = """
            Point:A B C D E F 
            正方形 ABCD
            点在线上 E,CD
            点在线上 F,BC
            CF=CE
            直线的交点 G,BE,DF
            Prove:
            BG L DF
            """;
    public static string _197 = """
            Point:A B C D E 
            多边形 ABCD
            AD // BC
            AB=AD+BC
            中点 ECD
            Prove:
            角平分线 AE,BAD
            角平分线 BE,ABC
            AEL BE
            
            """;
    public static string _213 = """
            Point:A B C D M 
            三角形 ABC
            //B=2C
            直线垂直 AD,BC
            点在线上 D,BC
            中点 MBC
            Prove:线段比值 AB,DM,2
            """;
    public static string _223 = """
            Point:A B C G H E F D 
            三角形 ABC
            点在线上 G,AC
            点在线上 H,AC
            //AG=GH=HC
            中点 E,AB
            中点 F,BC
            直线的交点 D,EG,FH
            Prove:平行四边形 ABCD
            """;

   

    public static string _224 = """
            Point:A B C D E F 
            三角形 ABC
            点在线上 G,AC
            点在线上 H,AC
            //AG=GH=HC
            中点 E,AB
            中点 F,BC
            直线的交点 D,EG,FH
            Prove:平行四边形 ABCD
            """;

}