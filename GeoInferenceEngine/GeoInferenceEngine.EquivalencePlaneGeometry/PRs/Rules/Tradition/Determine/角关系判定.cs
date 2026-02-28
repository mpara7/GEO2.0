using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Rules.Tradition.Determine
{

    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 角关系判定 : PlaneRuleClass
    {
        public void RuleDA001角的值相等的两个角相等(AngleSize angleSize,AngleSize angleSize1)
        {
            if(angleSize.Expr.CompareTo(angleSize1.Expr) == ExprCompareResult.Equal)
            {
                AngleSizeEqual angleSizeEqual = new AngleSizeEqual((Angle)angleSize[0], (Angle)angleSize1[0]);
                angleSizeEqual.AddReason();
                angleSizeEqual.AddCondition(angleSize, angleSize1);
                AddProcessor.Add(angleSizeEqual);
            }
        }

        public void RuleDA002垂足两条线形成的角的大小是90度(Pedal pedal)
        {
            Point point = (Point)pedal[0];
            Line line1 = (Line)pedal[1];
            Line line2 = (Line)pedal[2];
            List<Point> points_line1 = new List<Point>(line1.Points);
            List<Point> points_line2 = new List<Point>(line2.Points);
            if (points_line1.Contains(point))
            {
                points_line1.Remove(point);
            }
            if (points_line2.Contains(point))
            {
                points_line2.Remove(point);
            }
            for (int i = 0; i < points_line1.Count; i++)
            {
                for(int j = 0; j < points_line2.Count; j++)
                {
                    AngleSize angleSize = new AngleSize(KnowledgeGetter.GetAngle(points_line1[i], point, points_line2[j]),90);
                    angleSize.AddReason();
                    angleSize.AddCondition(pedal);
                    AddProcessor.Add(angleSize);
                }
            }
        }

       
    }
}
