namespace GeoInferenceEngine.PlaneKnowledges.Rules;


[PlaneRuleType(PlaneRuleType.Internal)]

public class PointInternalRules : PlaneRuleClass
{
    public void RulePi001中点得点在两点之间(Midpoint midpoint)
    {
        PointWithInPoints pred = new PointWithInPoints((Point)midpoint[0], (Point)midpoint.EndPoint1, (Point)midpoint.EndPoint2);
        pred.AddReason();
        pred.AddCondition(midpoint);
        AddProcessor.Add(pred);
    }
    public void RulePi002三等分点得点在线段两点之间(TrisectionPoint trisectionPoint)
    {
        PointWithInPoints pred = new PointWithInPoints((Point)trisectionPoint[0], (Point)trisectionPoint[2], (Point)trisectionPoint[3]);
        PointWithInPoints pred1 = new PointWithInPoints((Point)trisectionPoint[1], (Point)trisectionPoint[2], (Point)trisectionPoint[3]);
        pred.AddReason();
        pred.AddCondition(trisectionPoint);
        AddProcessor.Add(pred);
        pred1.AddReason();
        pred1.AddCondition(trisectionPoint);
        AddProcessor.Add(pred1);
    }

    #region 交点
    public void RulePi003垂足得到线段交点(Pedal pedal)
    {
        LineIntersectionPoint pred = new LineIntersectionPoint((Point)pedal[0], (Line)pedal[1], (Line)pedal[2]);
        pred.AddReason();
        pred.AddCondition(pedal);
        AddProcessor.Add(pred);
    }
    public void RulePi004垂足得到垂直(Pedal pedal)
    {
        LinePerpendicular pred = new LinePerpendicular((Line)pedal[1], (Line)pedal[2]);
        pred.AddReason();
        pred.AddCondition(pedal);
        AddProcessor.Add(pred);
    }
    #endregion

}
