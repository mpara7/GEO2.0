using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models;
public enum PlaneRuleType
{
    /// <summary>
    /// 未定义的
    /// </summary>
    Undefinded,

    Constructive,//构造转约束
    Internal,//特殊图形与几何关系生成一般
    CoLine,//生成共线
    AutoGenerate,//生成
    /// <summary>
    /// 传统法
    /// </summary>
    Tradition,
}
/// <summary>
/// 特性-规则类型，用于表示规则应该在那个推理阶段使用
/// </summary>
/// <param name="ruleType"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PlaneRuleTypeAttribute : Attribute
{
    public PlaneRuleTypeAttribute(PlaneRuleType ruleType)
    {
        RuleType = ruleType;
    }
    public PlaneRuleType RuleType { get; init; }
}

public abstract class PlaneRuleClass
{
    [ZDI]
    protected KnowledgeBase KnowledgeGetter { get; set; }
    [ZDI]
    protected KnowledgeAddProcessor AddProcessor { get; set; }

    public (bool isMatch, Knowledge other1, Knowledge other2) FindOther
        (Knowledge pm11, Knowledge pm12, Knowledge pm21, Knowledge pm22, Knowledge pm31, Knowledge pm32)
    {
        if (pm11 == pm21 && pm12 == pm31)
        {
            return (true, pm22, pm32);
        }
        else if (pm11 == pm22 && pm12 == pm31)
        {
            return (true, pm21, pm32);
        }
        else if (pm11 == pm21 && pm12 == pm32)
        {
            return (true, pm22, pm31);
        }
        else if (pm11 == pm22 && pm12 == pm32)
        {
            return (true, pm21, pm31);
        }
        else return (false, null, null);
    }
    public List<Point> GetPoints(params Knowledge[] pms)
    {
        List<Point> points = new List<Point>();
        foreach (var pm in pms)
        {
            foreach (var item in pm.Properties)
            {
                if (item is Point p)
                {
                    if (points.IndexOf(p) == -1)//没有才加
                        points.Add(p);
                }
                else
                {
                    foreach (var pitem in item.Properties)//最多就两层，懒得写递归
                    {
                        if (pitem is Point pi)
                        {
                            if (points.IndexOf(pi) == -1)
                                points.Add(pi);
                        }
                    }
                }
            }
        }
        return points;
    }
    public (Knowledge common, Knowledge notcommon1, Knowledge notcommon2) FindCommon(Knowledge pm1, Knowledge pm2, Knowledge pm3, Knowledge pm4)
    {
        if (pm1 == pm3)
        {
            return (pm1, pm2, pm4);
        }
        else if (pm1 == pm4)
        {
            return (pm1, pm2, pm3);
        }
        else if (pm2 == pm3)
        {
            return (pm2, pm1, pm4);
        }
        else if (pm2 == pm4)
        {
            return (pm2, pm1, pm3);
        }
        else return (null, null, null);
    }
    public (Knowledge common, Knowledge notcommon1, Knowledge notcommon2) FindCommon(Knowledge pm1, Knowledge pm2)
    {
        if (pm1.Properties.Count == 2 && pm2.Properties.Count == 2)
            return FindCommon(pm1[0], pm1[1], pm2[0], pm2[1]);
        throw new Exception("错误使用FindCommon函数");
    }
    public (T common, T notcommon1, T notcommon2) FindCommon<T>(Knowledge pm1, Knowledge pm2) where T : Knowledge
    {
        if (pm1.Properties.Count == 2 && pm2.Properties.Count == 2)
        {
            var (c, nc1, nc2) = FindCommon(pm1[0], pm1[1], pm2[0], pm2[1]);
            return ((T)c, (T)nc1, (T)nc2);
        }

        throw new Exception("错误使用FindCommon函数");
    }
    public (List<Knowledge> intersection, List<Knowledge> left1, List<Knowledge> left2) FindIntersection(Knowledge pms1, Knowledge pms2)
    {
        return FindIntersection(pms1.Properties, pms2.Properties);
    }
    public (List<T> intersection, List<T> left1, List<T> left2) FindIntersection<T>(List<T> pms1, List<T> pms2) where T : Knowledge
    {
        List<T> intersection;
        List<T> left1;
        List<T> left2;
        intersection = pms1.Where(p => pms2.Contains(p)).ToList();
        left1 = pms1.Where(p => !pms2.Contains(p)).ToList();
        left2 = pms2.Where(p => !pms1.Contains(p)).ToList();
        return (intersection, left1, left2);
    }
    public (List<Knowledge> intersection, List<Knowledge> left1, List<Knowledge> left2) FindIntersection(List<Knowledge> pms1, List<Knowledge> pms2)
    {
        List<Knowledge> intersection;
        List<Knowledge> left1;
        List<Knowledge> left2;
        intersection = pms1.Where(p => pms2.Contains(p)).ToList();
        left1 = pms1.Where(p => !pms2.Contains(p)).ToList();
        left2 = pms2.Where(p => !pms1.Contains(p)).ToList();
        return (intersection, left1, left2);
    }
    public bool IsInQuadriliateral(Knowledge quadriliateral, Point p1, Point p2, Point p3)
    {
        if (quadriliateral.Properties.Count != 4)
            throw new Exception("错误使用IsInQuadriliateral函数");
        if (quadriliateral[1] == p1 && quadriliateral[0] == p2 && quadriliateral[3] == p3 ||
            quadriliateral[3] == p1 && quadriliateral[0] == p2 && quadriliateral[1] == p3)
        {
            return true;
        }
        else if (quadriliateral[0] == p1 && quadriliateral[1] == p2 && quadriliateral[2] == p3 ||
            quadriliateral[2] == p1 && quadriliateral[1] == p2 && quadriliateral[0] == p3)
        {
            return true;
        }
        else if (quadriliateral[1] == p1 && quadriliateral[2] == p2 && quadriliateral[3] == p3 ||
            quadriliateral[3] == p1 && quadriliateral[2] == p2 && quadriliateral[1] == p3)
        {
            return true;
        }
        else if (quadriliateral[2] == p1 && quadriliateral[3] == p2 && quadriliateral[0] == p3 ||
            quadriliateral[0] == p1 && quadriliateral[3] == p2 && quadriliateral[2] == p3)
        {
            return true;
        }
        else return false;

    }

    protected Segment GetSegment(Knowledge point1, Knowledge point2)
    {
        return KnowledgeGetter.GetSegment(point1, point2);
    }
    protected Segment GetSegment(Point point1,Point point2)
    {
        return KnowledgeGetter.GetSegment(point1,point2);
    }

    protected Line GetLine(Point point1, Point point2)
    {
        return KnowledgeGetter.GetLine(point1, point2);
    }
}
