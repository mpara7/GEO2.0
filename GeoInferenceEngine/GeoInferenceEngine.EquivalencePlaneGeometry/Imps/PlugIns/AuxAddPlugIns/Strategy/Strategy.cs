using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using System.Text;
using System;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.Strategy;

public abstract class Strategy: PlaneRuleClass
{
    [ZDI]
    protected KnowledgeBase KnowledgeGetter { get; set; }


    private static int index = 0;

    protected Segment GetSegment(Knowledge point1, Knowledge point2)
    {
        return KnowledgeGetter.GetSegment(point1, point2);
    }
    protected Segment GetSegment(Point point1, Point point2)
    {
        return KnowledgeGetter.GetSegment(point1, point2);
    }

    protected Line GetLine(Point point1, Point point2)
    {
        return KnowledgeGetter.GetLine(point1, point2);
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
        if(pm1 == pm2)
        {
            return (pm1, null, null);
        }
        if (pm1.Properties.Count == 2 && pm2.Properties.Count == 2)
            return FindCommon(pm1[0], pm1[1], pm2[0], pm2[1]);
        throw new Exception("错误使用FindCommon函数");
    }

    public  string GenerateRandomString()
    {

        return "O" + index++;
        
    }



}




