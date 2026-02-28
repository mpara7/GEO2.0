using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Rules.Tradition.Determine
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 点关系判定 : PlaneRuleClass
    {
        public void 中点的判定(PointWithInPoints p,SegmentLengthEqual segmentLengthEqual )
        {
            var seg1 = GetSegment(p.EndPoint1,p.Point);
            var seg2 = GetSegment(p.EndPoint2, p.Point);
            if ((seg1 == segmentLengthEqual.Seg1&& seg2 == segmentLengthEqual.Seg2)||
                (seg1 == segmentLengthEqual.Seg2 && seg2 == segmentLengthEqual.Seg1)) 
            {
                Midpoint pred = new Midpoint(p.Point,p.EndPoint1,p.EndPoint2); 
                pred.AddCondition(p, segmentLengthEqual);
                pred.AddReason();
                AddProcessor.Add(pred);
            }
        }
        public void 中点的判定2(PointWithInPoints p, SegmentLengthRatio lengthRatio)
        {
            var ab = GetSegment(p.EndPoint1, p.Point);
            var bc = GetSegment(p.EndPoint2, p.Point);
            var ac = GetSegment(p.EndPoint1, p.EndPoint2);
            if ((lengthRatio.Seg1 == ab && lengthRatio.Seg2 == ac &&lengthRatio.Expr==Expr.AHalf) ||
                (lengthRatio.Seg1 == ac && lengthRatio.Seg2 == ab && lengthRatio.Expr == Expr.Two))
            {
                Midpoint pred = new Midpoint(p.Point, p.EndPoint1, p.EndPoint2);
                pred.AddCondition(p, lengthRatio);
                pred.AddReason();
                AddProcessor.Add(pred);
            }
            else if ((lengthRatio.Seg1 == bc && lengthRatio.Seg2 == ac && lengthRatio.Expr == Expr.AHalf) ||
                (lengthRatio.Seg1 == ac && lengthRatio.Seg2 == bc && lengthRatio.Expr == Expr.Two))
            {
                Midpoint pred = new Midpoint(p.Point, p.EndPoint1, p.EndPoint2);
                pred.AddCondition(p, lengthRatio);
                pred.AddReason();
                AddProcessor.Add(pred);
            }
        }
    }
}
